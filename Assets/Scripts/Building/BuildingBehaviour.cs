using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingBehaviour : BaseAEMonoCI, IOrderDestination
{
    [HideInInspector][ComponentInject(Required.OPTIONAL)] public GhostBuildingBehaviour GhostBuildingBehaviour;

    public Purpose Purpose = Purpose.BUILDING;
    public GameObject Real;
    public bool DEBUG_RealImmediately;

    public BuildingType BuildingType;// wordt geset door builder bij bouwen
    public List<ItemAmountBuffer> RequiredItems => BuildingType.GetBuildCosts();

    [HideInInspector] public DateTime StartTimeBuildingTheBuilding; // voor weergave progressie bouwen 

    public GameObject GetGO() {try { return this.gameObject; } catch (Exception e) {return null; }} // voor stoppen unity.... TODO
    [ComponentInject(Required.OPTIONAL)] private IValidateOrder validateOrder;

    private void Start()
    {
        AE.NewBuilding?.Invoke(this);
    }

    private void EnableRealWithoutActivating()
    {
        var children = Real.GetComponentsInChildren<MonoBehaviour>();
        foreach (var monoBehaviour in children) monoBehaviour.enabled = false;

        GhostBuildingBehaviour.gameObject.SetActive(false);
        Real.SetActive(true);

        ActiveStartBuildingScripts();
    }

    private void ActivateReal()
    {
        Destroy(GhostBuildingBehaviour.gameObject);
        Destroy(GhostBuildingBehaviour.gameObject.GetComponentInChildren<RangeDisplayBehaviour>());
        Destroy(gameObject.GetComponent<CheckCollisionHandler>());

        Real.SetActive(true);

        var children = Real.GetComponentsInChildren<MonoBehaviour>();
        foreach (var monoBehaviour in children) monoBehaviour.enabled = true;

        var locOfResource = Real.GetComponent<ILocationOfResource>();
        if (locOfResource != null)
        {
            var rangedDisplay = Instantiate(MonoHelper.Instance.RangedDisplayPrefab, ((MonoBehaviour)locOfResource).transform);
            rangedDisplay.MaxRangeForResource = locOfResource.GetMaxRangeForResource();
            rangedDisplay.RangeType = locOfResource.GetRangeTypeToFindResource();
        }        
    }

    [HideInInspector] public BuildStatus CurrentBuildStatus;

    protected override void OnBuilderRequestStatusChanged(BuilderRequest builderRequest, BuildStatus previousStatus)
    {
        if (GhostBuildingBehaviour != null && builderRequest.GameObject == GhostBuildingBehaviour.transform.parent.gameObject)
        {
            CurrentBuildStatus = builderRequest.Status;

            if (builderRequest.Status == BuildStatus.CANCEL)
            {
                HandleCanceledBuilding(builderRequest, previousStatus);
            }
            else if (builderRequest.Status == BuildStatus.COMPLETED_PREPARING)
            {
                if(RequiredItems.Count == 0)
                {
                    ActivateReal();
                }
                else
                {
                    CreateSerfRequests();
                }
            }
            else if (builderRequest.Status == BuildStatus.BUILDING && previousStatus == BuildStatus.NEEDS_BUILDING)
            {
                EnableRealWithoutActivating();
                StartTimeBuildingTheBuilding = DateTime.Now;
            }
            else if (builderRequest.Status == BuildStatus.COMPLETED_BUILDING && previousStatus == BuildStatus.BUILDING)
            {
                ActivateReal();                
            }
            else if (builderRequest.Status == BuildStatus.PREPARING && previousStatus == BuildStatus.NEEDS_PREPARE)
            {
                StartTimeBuildingTheBuilding = DateTime.Now;
            }
        }
    }

    private void HandleCanceledBuilding(BuilderRequest builderRequest, BuildStatus previousStatus)
    {
        // genereer nieuw request, aangezien deze is gecanceled
        BuildStatus statusForNewReq;
        switch(previousStatus)
        {
            case BuildStatus.NEEDS_PREPARE:
            case BuildStatus.PREPARING:
            case BuildStatus.NONE:
                statusForNewReq = BuildStatus.NEEDS_PREPARE;
                break;
            case BuildStatus.BUILDING:
            case BuildStatus.NEEDS_BUILDING:
                statusForNewReq = BuildStatus.NEEDS_BUILDING;
                break;
            default:
                throw new Exception($"Geen buildstatus ('{previousStatus}') dat mogelijk is");
        }

        var newBuilderRequest = new BuilderRequest
        {
            Status = BuildStatus.NONE,
            Purpose = builderRequest.Purpose,
            GameObject = builderRequest.GameObject
        };
        AE.BuilderRequest?.Invoke(newBuilderRequest);
        newBuilderRequest.Status = statusForNewReq; // forceert change; maakt het makkelijker
    }

    protected override void OnOrderStatusChanged(SerfOrder serfOrder, Status prevStatus)
    {
        if(GhostBuildingBehaviour == null)
        {
            return; // al afgebouwd -> niet van toepassing
        }

        if (serfOrder.To != null && serfOrder.To.GameObject == this.gameObject)
        {
            if (serfOrder.Status == Status.SUCCESS)
            {
                TryCallBuilderToFinish();
            }
        }
    }

    private void CreateSerfRequests()
    {
        if (GhostBuildingBehaviour == null)
        {
            return; // al afgebouwd -> niet van toepassing
        }

        foreach (var requiredItem in RequiredItems)
        {
            var count = 0;
            while (count < requiredItem.Amount)
            {
                var serfRequest = new SerfRequest
                {
                    Purpose = Purpose,
                    ItemType = requiredItem.ItemType,
                    OrderDestination = this,
                    Direction = Direction.PULL,
                    IsOriginator = true
                };

                AE.SerfRequest?.Invoke(serfRequest);
                count++;
            }
        }
    }

    public void ActivateGhost()
    {
        if (GhostBuildingBehaviour == null)
        {
            return; // al afgebouwd -> niet van toepassing
        }
        GhostBuildingBehaviour.gameObject.SetActive(true);
        var children = GhostBuildingBehaviour.GetComponentsInChildren<MonoBehaviour>();
        foreach (var monoBehaviour in children) monoBehaviour.enabled = true;

        ActivateBuildersRequest();        
    }

    private void ActiveStartBuildingScripts()
    {
        var moveToSurfaceScript = Real.GetComponent<MoveObjectToSurface>();        
        if (moveToSurfaceScript != null)
        {
            moveToSurfaceScript.enabled = true;
        }
    }

    private void ActivateBuildersRequest()
    {
        if (DEBUG_RealImmediately)
        {
            // Forceer event met een change -> niet alleen voor Current BuildStatus, maar ook voor entrance exit
            var req = new BuilderRequest
            {
                Purpose = Purpose.BUILDING,
                Status = BuildStatus.NONE,
                GameObject = GhostBuildingBehaviour.transform.parent.gameObject
            };
            AE.BuilderRequest?.Invoke(req);
            req.Status = BuildStatus.COMPLETED_BUILDING;

            ActivateReal();            
        }     
    }

    public void TryCallBuilderToFinish()
    {
        if (AllRequiredItemsCompleted())
        {
            AE.BuilderRequest?.Invoke(new BuilderRequest
            {
                Purpose = Purpose,
                Status = BuildStatus.NEEDS_BUILDING,
                GameObject = GhostBuildingBehaviour.transform.parent.gameObject
            });
        }
    }

    public int GetItemCountDeliveredForBuilding(ItemType itemtype)
    {
        if (GhostBuildingBehaviour == null)
        {
            return -1; // al afgebouwd -> niet van toepassing
        }

        var allCompletedSerfOrdersForThisBuilding = GameManager.Instance.CompletedOrdersIncFailed.Where(x => x.Status == Status.SUCCESS && x.To.GameObject == this.gameObject).ToList();
        return allCompletedSerfOrdersForThisBuilding.Count(x => x.ItemType == itemtype);
    }

    private bool AllRequiredItemsCompleted()
    {
        if (GhostBuildingBehaviour == null)
        {
            return true; // al afgebouwd -> niet van toepassing
        }
        foreach (var requiredItem in RequiredItems)
        {
            var countItemsForItemTypeDelivered = GetItemCountDeliveredForBuilding(requiredItem.ItemType);
            if (countItemsForItemTypeDelivered < requiredItem.Amount)
            {
                return false;
            }
        }

        return true;
    }

    public bool CanProcessOrder(SerfOrder serfOrder)
    {
        if (validateOrder == null)
        {
            return true;
        }
        return validateOrder.CanProcessOrder(serfOrder);
    }
}