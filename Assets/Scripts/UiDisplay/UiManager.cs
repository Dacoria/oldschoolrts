using UnityEngine;
using UnityEngine.EventSystems;

public class UiManager : MonoBehaviour
{
    public Camera Camera;

    public GameObject SelectedBuildingPanel;
    public GameObject SelectedBuildingPanelSkillTree;
    public GameObject QueueUiGo;
    public MonoBehaviour BuildingOverviewUI;

    public OutlineBehaviour ActiveOutlineBehaviour;
    public RangeDisplayBehaviour ActiveRangeDisplayBehaviour;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            DisableActiveOutline();
            DisableActiveDisplayRange();
            var isClickingUi = EventSystem.current.IsPointerOverGameObject();

            if (!isClickingUi)
            {
                Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
                var hits = Physics.RaycastAll(ray);
                ActOnRaycastHit(hits);
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            DisableActiveOutline();
            DisableActiveDisplayRange();
            SelectedBuildingPanel.SetActive(false);
            SelectedBuildingPanelSkillTree.SetActive(false);
        }
    }

    private void DisableActiveOutline()
    {
        if(ActiveOutlineBehaviour != null)
        {
            ActiveOutlineBehaviour.enabled = false;
        }
    }

    private void DisableActiveDisplayRange()
    {
        if (ActiveRangeDisplayBehaviour != null)
        {
            ActiveRangeDisplayBehaviour.gameObject.SetActive(false);
        }
    }

    private void EnableOutline(GameObject go)
    {
        ActiveOutlineBehaviour = go.GetComponent<OutlineBehaviour>();
        if(ActiveOutlineBehaviour != null)
        {
            ActiveOutlineBehaviour.enabled = true;
        }
        else
        {
            ActiveOutlineBehaviour = go.AddComponent<OutlineBehaviour>();
        }
    }    

    private void ActOnRaycastHit(RaycastHit[] hits)
    {
        MonoBehaviour toactivateGo = GetBuildingHitByRay(hits);

        if (toactivateGo != null)
        {
            EnableOutline(toactivateGo.gameObject);

            var resourceRangeBuilding = toactivateGo.transform.parent.GetComponentInChildren<RangeDisplayBehaviour>(true);
            if (resourceRangeBuilding != null)
            {
                ActiveRangeDisplayBehaviour = resourceRangeBuilding;
                resourceRangeBuilding.gameObject.SetActive(true);
            }
        }
        else
        {
            foreach (var hit in hits)
            {
                AE.LeftClickOnGo(hit.transform.gameObject); // nu gebruikt voor tooltips
            }
        }

        var uiBuildingHit = toactivateGo != null;
        ActivateUI(uiBuildingHit, toactivateGo);
    }

    private MonoBehaviour GetBuildingHitByRay(RaycastHit[] hits)
    {
        MonoBehaviour toactivateGo = null;
        foreach (var hit in hits)
        {
            var stockpileBehaviour = hit.transform.gameObject.GetComponentInChildren<StockpileBehaviour>();
            if (stockpileBehaviour != null)
            {
                var go = SelectedBuildingPanel.GetComponentInChildren<StockpileUiBehaviour>(true);
                go.CallingStockpile = stockpileBehaviour;
                toactivateGo = go;
                break;
            }

            var displayCardBuilding = hit.transform.gameObject.GetComponentInChildren<ICardBuilding>();
            if (displayCardBuilding != null)
            {
                var go = GetDisplayCardUiHandlerToActivate(hit.transform.gameObject);
                go.CallingBuilding = displayCardBuilding;
                toactivateGo = go;
                break;
            }
            var tavernBuilding = hit.transform.gameObject.GetComponentInChildren<TavernBehaviour>();
            if (tavernBuilding != null)
            {
                var go = SelectedBuildingPanel.GetComponentInChildren<UiTavernBehaviour>(true);
                go.CallingTavern = tavernBuilding;
                toactivateGo = go;
                break;
            }            
        }

        return toactivateGo;
    }

    private CardUiHandler GetDisplayCardUiHandlerToActivate(GameObject goClicked)
    {
        var buildingTypeOfGo = GetBuildingType(goClicked);

        var goDisplayUiHandlers = SelectedBuildingPanel.GetComponentsInChildren<CardUiHandler>(true);
        foreach (var goDisplayUiHandler in goDisplayUiHandlers)
        {
            if(goDisplayUiHandler.BuildingType == buildingTypeOfGo)
            {
                return goDisplayUiHandler;
            }
        }

        throw new System.Exception("Geen Building gevonden om te activerren in UI -> Building type " + buildingTypeOfGo);
    }

    private BuildingType GetBuildingType(GameObject gameObject)
    {
        var buildingBehaviour = gameObject.GetComponent<BuildingBehaviour>();
        if(buildingBehaviour == null)
        {
            buildingBehaviour = gameObject.GetComponentInChildren<BuildingBehaviour>();
            if (buildingBehaviour == null)
            {
                throw new System.Exception("Building heeft wel IDisplayCardBuilding maar geen BuildingBehaviour???? --> ERROR");
            }
        }

        return buildingBehaviour.BuildingType;
    }

    // via button click
    public void ActivateUi(MonoBehaviour mono)
    {
        if (mono.gameObject.name == "SkillTree")
        {
            // skilltree zit in een andere canvas
            ActivateUI(true, mono, SelectedBuildingPanelSkillTree);
        }
        else
        {
            ActivateUI(true, mono);
        }               
    }  

    public void ActivateUI(bool activateUiPanel, MonoBehaviour uiToActivate = null, GameObject selectedBuildingPanel = null)
    {
        if(selectedBuildingPanel == null)
        {
            selectedBuildingPanel = SelectedBuildingPanel;
        }

        SetAllCanvasItemsToInactive(SelectedBuildingPanel);
        SetAllCanvasItemsToInactive(SelectedBuildingPanelSkillTree);
        ActivateQueueIfNeeded(uiToActivate);

        SelectedBuildingPanel.SetActive(false);
        SelectedBuildingPanelSkillTree.SetActive(false);

        selectedBuildingPanel.SetActive(activateUiPanel);
        if (uiToActivate != null)
        {
            uiToActivate.gameObject.SetActive(true);
            uiToActivate.gameObject.transform.parent.gameObject.SetActive(true);
        }
    }

    private void SetAllCanvasItemsToInactive(GameObject selectedBuildingPanel)
    {
        foreach (Transform transform in selectedBuildingPanel.transform)
        {            
            transform.gameObject.SetActive(false);            
        }
    }

    private void ActivateQueueIfNeeded(MonoBehaviour uiToActivate)
    {
        if(uiToActivate is CardUiHandler)
        {
            var cardHandler = (CardUiHandler)uiToActivate;
            if(cardHandler?.CallingBuilding?.GetQueueForBuildingBehaviour() != null)
            {
                QueueUiGo.SetActive(true);
                return;
            }

        }

        QueueUiGo.SetActive(false);
    }
}

