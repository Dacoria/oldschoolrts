using System;
using System.Linq;
using UnityEngine;

public class BuildingBehaviour : BaseAEMonoCI
{
    //private int completed = 0;

    [HideInInspector]
    [ComponentInject] public GhostBuildingBehaviour GhostBuildingBehaviour;

    [ComponentInject] public CheckCollisionForBuilding colScript;

    public Purpose Purpose = Purpose.BUILDING;
    public GameObject Real;
    public bool DEBUG_RealImmediately;
    public float TimeToBuildRealInSeconds = 5; //kan overschreven worden per prefab
    public float TimeToPrepareBuildingInSeconds = 3; //kan overschreven worden per prefab

    //[HideInInspector]
    public BuildingType BuildingType;// wordt geset door builder bij bouwen   

    public ItemAmountBuffer[] RequiredItems;

    [HideInInspector]
    public DateTime StartTimeBuildingTheBuilding; // voor weergave progressie bouwen 

    public void Start()
    {
        CheckDisableCollisionScript();
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
        GhostBuildingBehaviour.gameObject.SetActive(false);
        Destroy(GhostBuildingBehaviour.gameObject.GetComponentInChildren<RangeDisplayBehaviour>());

        Destroy(gameObject.GetComponent<CheckCollisionForBuilding>());

        Real.SetActive(true); // staat al aan; zekerheidje

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

    [HideInInspector]
    public BuildStatus CurrentBuildStatus;


    protected override void OnBuilderRequestStatusChanged(BuilderRequest builderRequest, BuildStatus previousStatus)
    {
        if (builderRequest.GameObject == GhostBuildingBehaviour.transform.parent.gameObject)
        {
            CurrentBuildStatus = builderRequest.Status;

            if (builderRequest.Status == BuildStatus.CANCEL)
            {
                HandleCanceledBuilding(builderRequest, previousStatus);
            }
            else if (builderRequest.Status == BuildStatus.COMPLETED_PREPARING)
            {
                if(RequiredItems.Length == 0)
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

    protected override void OnOrderStatusChanged(SerfOrder serfOrder)
    {
        if (serfOrder.To != null && serfOrder.To.GameObject == GhostBuildingBehaviour.gameObject)
        {
            if (serfOrder.Status == Status.SUCCESS)
            {
                TryCallBuilderToFinish();
            }
        }
    }

    private void CreateSerfRequests()
    {
        foreach (var requiredItem in RequiredItems)
        {
            var count = 0;
            while (count < requiredItem.Amount)
            {
                var serfRequest = new SerfRequest
                {
                    Purpose = Purpose,
                    ItemType = requiredItem.ItemType,
                    GameObject = GhostBuildingBehaviour.gameObject,
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
        GhostBuildingBehaviour.gameObject.SetActive(true);
        var children = GhostBuildingBehaviour.GetComponentsInChildren<MonoBehaviour>();
        foreach (var monoBehaviour in children) monoBehaviour.enabled = true;

        CheckDisableCollisionScript();
        ActivateBuildersRequest();        
    }

    private void ActiveStartBuildingScripts()
    {
        var moveToSurfaceScript = Real.GetComponent<MoveObjectToSurface>();        
        if (moveToSurfaceScript != null)
        {
            moveToSurfaceScript.enabled = true;
        }

        var buildProgressTextScript = Real.GetComponent<BuildProgressTextScript>();
        if (buildProgressTextScript != null)
        {
            buildProgressTextScript.enabled = true;
        }
    }

    private void CheckDisableCollisionScript()
    {
        if(colScript != null)
        {
            if(Real.activeSelf || GhostBuildingBehaviour.isActiveAndEnabled)
            {
                colScript.enabled = false; // alleen collisions (rode kleur) bij template mode, niet Gost of Real
            }
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
        var allCompletedSerfOrdersForThisBuilding = GameManager.Instance.CompletedOrders.Where(x => x.To.GameObject == GhostBuildingBehaviour.gameObject).ToList();
        return allCompletedSerfOrdersForThisBuilding.Count(x => x.ItemType == itemtype);
    }

    private bool AllRequiredItemsCompleted()
    {
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
}