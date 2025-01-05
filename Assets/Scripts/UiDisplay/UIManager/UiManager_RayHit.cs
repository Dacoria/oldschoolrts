using System.Linq;
using UnityEngine;

public partial class UiManager : MonoBehaviour
{
    private bool ActOnRaycastHit(RaycastHit[] hits)
    {
        (BuildingBehaviour buildingBehaviourHit, MonoBehaviour uiToActivate) = GetBuildingHitByRay(hits);

        if (buildingBehaviourHit != null && uiToActivate != null)
        {
            if (UIUserSettings.ShowExternalUIElementsActiveBuilding)            
                EnableExternalUIElementsActiveBuilding(buildingBehaviourHit.gameObject);
            
            EnableOutline(buildingBehaviourHit.gameObject);
            ActiveRangedDisplay(buildingBehaviourHit.gameObject);
            ActivateUI(uiToActivate);
            return true;
        }
        else
        {            
            DisableEntireCanvas();
            foreach (var hit in hits)
            {
                AE.LeftClickOnGo?.Invoke(hit.transform.gameObject); // nu gebruikt voor tooltips
            }

            return false;
        }
    }

    private (BuildingBehaviour, MonoBehaviour) GetBuildingHitByRay(RaycastHit[] hits)
    {
        foreach (var hit in hits)
        {
            var buildingBehaviourHit = hit.transform.gameObject.GetComponentInChildren<BuildingBehaviour>();
            if (buildingBehaviourHit == null)
                continue;

            switch (buildingBehaviourHit.CurrentBuildStatus)
            {
                case BuildStatus.NEEDS_PLACEMENT:
                case BuildStatus.NEEDS_PREPARE:
                case BuildStatus.PREPARING:
                case BuildStatus.COMPLETED_PREPARING:
                case BuildStatus.NEEDS_BUILDING:
                case BuildStatus.BUILDING:
                case BuildStatus.CANCEL:
                    return GetUIBuildingBeingBuild(buildingBehaviourHit);
                case BuildStatus.COMPLETED_BUILDING:
                    return GetUIFinishedBuilding(buildingBehaviourHit);
                case BuildStatus.NONE:
                    return (buildingBehaviourHit, null);
                default:
                    throw new System.Exception();
            }
        }

        return (null, null);
    }

    private (BuildingBehaviour, MonoBehaviour) GetUIFinishedBuilding(BuildingBehaviour buildingBehaviourHit)
    {
        var category = buildingBehaviourHit.BuildingType.GetCategory();
        switch (category)
        {
            case BuildingCategory.Manual:
            case BuildingCategory.OneProductOverTime:
            case BuildingCategory.Mine:
                var ui0 = SelectedBuildingPanel.GetComponentsInChildren<CardOneProdUiHandler>(true).First();
                ui0.CallingBuilding = buildingBehaviourHit.GetComponentInChildren<ICardOneProdBuilding>(); // niet inactieve ophalen
                if (ui0.CallingBuilding != null)
                    return (buildingBehaviourHit, ui0);
                else
                    return (buildingBehaviourHit, null);

            case BuildingCategory.SelectProductsOverTime:
            case BuildingCategory.School:
            case BuildingCategory.Barracks:
                var ui1 = SelectedBuildingPanel.GetComponentsInChildren<CardSelectProdUiHandler>(true).First(x => x.BuildingCategory == category);
                ui1.CallingBuilding = buildingBehaviourHit.GetComponentInChildren<ICardSelectProdBuilding>(); // niet inactieve ophalen
                if (ui1.CallingBuilding != null)
                    return (buildingBehaviourHit, ui1);
                else
                    return (buildingBehaviourHit, null);

            case BuildingCategory.Stockpile:
                var ui2 = SelectedBuildingPanel.GetComponentsInChildren<StockpileUiBehaviour>(true).First();
                ui2.CallingStockpile = buildingBehaviourHit.GetComponentInChildren<StockpileBehaviour>(); // niet inactieve ophalen
                if (ui2.CallingStockpile != null)
                    return (buildingBehaviourHit, ui2);
                else
                    return (buildingBehaviourHit, null);

            case BuildingCategory.Tavern:
                var ui3 = SelectedBuildingPanel.GetComponentsInChildren<UiTavernBehaviour>(true).First();
                ui3.CallingTavern = buildingBehaviourHit.GetComponentInChildren<TavernBehaviour>(); // niet inactieve ophalen
                if (ui3.CallingTavern != null)
                    return (buildingBehaviourHit, SelectedBuildingPanel.GetComponentsInChildren<UiTavernBehaviour>(true).First());
                else
                    return (buildingBehaviourHit, null);
            default:
                return (buildingBehaviourHit, null);
        }
    }

    private (BuildingBehaviour, MonoBehaviour) GetUIBuildingBeingBuild(BuildingBehaviour buildingBehaviourHit)
    {
        var ui = SelectedBuildingPanel.GetComponentsInChildren<CardsBuildingBeingBuildUiBehaviour>(true).First();
        ui.CallingGhostBuildingBehaviour = buildingBehaviourHit.GetComponentInChildren<GhostBuildingBehaviour>();
        if (ui.CallingGhostBuildingBehaviour != null)
            return (buildingBehaviourHit, ui);
        else
            return (buildingBehaviourHit, null);
    }

    public void ActivateUI(MonoBehaviour ui)
    {
        DisableEntireCanvas();
        SelectedBuildingPanel.SetActive(true);

        if (ui != null)
        {
            ActivateQueueIfNeeded(ui);
            ui.gameObject.SetActive(true);
            ui.gameObject.transform?.parent?.gameObject.SetActive(true);
        }
    }
}