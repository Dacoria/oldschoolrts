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
    public DisplayBuildingInputOutputHandler ActiveDisplayBuildingInputOutputHandler;
    public DisplayBuildingNameImgHandler ActiveDisplayBuildingNameImgHandler;

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            DisableActiveBuilding();
            SelectedBuildingPanel.SetActive(false);
            SelectedBuildingPanelSkillTree.SetActive(false);
        }
        if (Input.GetMouseButtonDown(0))
        {
            DisableActiveBuilding();
            var isClickingUi = EventSystem.current.IsPointerOverGameObject();

            if (!isClickingUi)
            {
                Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
                var hits = Physics.RaycastAll(ray);
                ActOnRaycastHit(hits);
            }
        }        
    }

    private void DisableActiveBuilding()
    {
        DisableActiveOutline();
        DisableActiveDisplayRange();
        DisableActiveDisplayBuildingInputOutputHandler();
        DisableActiveDisplayBuildingNameImgHandler();        
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

    private void DisableActiveDisplayBuildingInputOutputHandler()
    {
        if (ActiveDisplayBuildingInputOutputHandler != null)
        {
            ActiveDisplayBuildingInputOutputHandler.UpdateEnabledStatusOfDisplayObjects(KeyCodeStatusSettings.ToggleInputOutputDisplay_Active);
            ActiveDisplayBuildingInputOutputHandler = null;
        }
    }

    private void DisableActiveDisplayBuildingNameImgHandler()
    {
        if (ActiveDisplayBuildingNameImgHandler != null)
        {
            ActiveDisplayBuildingNameImgHandler.UpdateEnabledStatusOfDisplayObjects(KeyCodeStatusSettings.ToggleBuildingNameImgDisplay_Active);
            ActiveDisplayBuildingNameImgHandler = null;
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
        (MonoBehaviour toactivateUI, GameObject buildingHit) = GetBuildingHitByRay(hits);

        if (buildingHit != null)
        {
            EnableOutline(buildingHit);

            var resourceRangeBuilding1 = buildingHit.transform.GetComponentInChildren<RangeDisplayBehaviour>(true);
            var resourceRangeBuilding2 = buildingHit.transform.GetComponent<RangeDisplayBehaviour>();
            var resourceRangeBuilding = resourceRangeBuilding1 != null ? resourceRangeBuilding1 : resourceRangeBuilding2;

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
                AE.LeftClickOnGo?.Invoke(hit.transform.gameObject); // nu gebruikt voor tooltips
            }
        }

        var shouldActivateUIPart = toactivateUI != null;
        ActivateUI(shouldActivateUIPart, toactivateUI);
    }

    private (MonoBehaviour, GameObject) GetBuildingHitByRay(RaycastHit[] hits)
    {
        MonoBehaviour toactivateGo = null;
        GameObject goBuildingHit = null;

        foreach (var hit in hits)
        {
            // Altijd input altijd; bij overige: BREAK
            var displayBuildingInputOutputHandler = hit.transform.gameObject.GetComponentInChildren<DisplayBuildingInputOutputHandler>();
            if (displayBuildingInputOutputHandler != null && !displayBuildingInputOutputHandler.gameObject.IsRoad())
            {
                goBuildingHit = displayBuildingInputOutputHandler.gameObject;
                ActiveDisplayBuildingInputOutputHandler = displayBuildingInputOutputHandler;
                ActiveDisplayBuildingInputOutputHandler.UpdateEnabledStatusOfDisplayObjects(true);

                var displayBuildingNameImgHandler = goBuildingHit.GetComponent<DisplayBuildingNameImgHandler>();
                if (displayBuildingNameImgHandler != null)
                {
                    ActiveDisplayBuildingNameImgHandler = displayBuildingNameImgHandler;
                    ActiveDisplayBuildingNameImgHandler.UpdateEnabledStatusOfDisplayObjects(true);
                }
            }

            var stockpileBehaviour = hit.transform.gameObject.GetComponentInChildren<StockpileBehaviour>();
            if (stockpileBehaviour != null)
            {
                goBuildingHit = stockpileBehaviour.gameObject;
                var go = SelectedBuildingPanel.GetComponentInChildren<StockpileUiBehaviour>(true);
                go.CallingStockpile = stockpileBehaviour;
                toactivateGo = go;
                break;
            }

            var displayCardBuilding = hit.transform.gameObject.GetComponentInChildren<ICardBuilding>();
            if (displayCardBuilding != null)
            {
                goBuildingHit = displayCardBuilding.GetGameObject();
                var go = GetDisplayCardUiHandlerToActivate(hit.transform.gameObject);
                go.CallingBuilding = displayCardBuilding;
                toactivateGo = go;
                break;
            }

            var tavernBuilding = hit.transform.gameObject.GetComponentInChildren<TavernBehaviour>();
            if (tavernBuilding != null)
            {
                goBuildingHit = tavernBuilding.gameObject;
                var go = SelectedBuildingPanel.GetComponentInChildren<UiTavernBehaviour>(true);
                go.CallingTavern = tavernBuilding;
                toactivateGo = go;
                break;
            }
        }

        return (toactivateGo, goBuildingHit);
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

        throw new System.Exception($"Geen Building gevonden om te activerren in UI -> Building type: {buildingTypeOfGo}");
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
            var queue = cardHandler?.CallingBuilding?.GetGameObject()?.GetComponent<QueueForBuildingBehaviour>();
            if (queue != null)
            {
                QueueUiGo.GetComponentInChildren<UiQueueHandler>(true).CallingQueueForBuildingBehaviour = queue;
                QueueUiGo.SetActive(true);
                return;
            }
        }

        QueueUiGo.SetActive(false);
    }
}