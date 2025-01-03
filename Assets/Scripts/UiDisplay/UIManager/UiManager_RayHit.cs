using System.Linq;
using UnityEngine;

public partial class UiManager : MonoBehaviour
{
    private void ActOnRaycastHit(RaycastHit[] hits)
    {
        (BuildingBehaviour buildingBehaviourHit, MonoBehaviour uiToActivate) = GetBuildingHitByRay(hits);

        if (buildingBehaviourHit != null)
        {
            EnableOutline(buildingBehaviourHit.gameObject);
            ActiveRangedDisplay(buildingBehaviourHit.gameObject);
            ActivateUI(uiToActivate);
        }
        else
        {
            DisableEntireCanvas();
            foreach (var hit in hits)
            {
                AE.LeftClickOnGo?.Invoke(hit.transform.gameObject); // nu gebruikt voor tooltips
            }
        }
    }

    private (BuildingBehaviour, MonoBehaviour) GetBuildingHitByRay(RaycastHit[] hits)
    {
        foreach (var hit in hits)
        {
            var buildingBehaviourHit = hit.transform.gameObject.GetComponentInChildren<BuildingBehaviour>();
            if (buildingBehaviourHit == null)
            {
                continue;
            }

            EnableUIActiveBuilding(buildingBehaviourHit.gameObject);
            var category = buildingBehaviourHit.BuildingType.GetCategory();
            switch (category)
            {
                case BuildingCategory.Manual:
                case BuildingCategory.OneProductOverTime:
                case BuildingCategory.Mine:
                    var ui0 = SelectedBuildingPanel.GetComponentsInChildren<CardOneProdUiHandler>(true).First();
                    ui0.CallingBuilding = buildingBehaviourHit.GetComponentInChildren<ICardOneProdBuilding>(); // niet inactieve ophalen
                    if(ui0.CallingBuilding != null)

                        return (buildingBehaviourHit, ui0);
                    else
                        return (null, null);

                case BuildingCategory.SelectProductsOverTime:
                case BuildingCategory.School:
                case BuildingCategory.Barracks:
                    var ui1 = SelectedBuildingPanel.GetComponentsInChildren<CardSelectProdUiHandler>(true).First(x => x.BuildingCategory == category);
                    ui1.CallingBuilding = buildingBehaviourHit.GetComponentInChildren<ICardSelectProdBuilding>(); // niet inactieve ophalen
                    if (ui1.CallingBuilding != null)
                        return (buildingBehaviourHit, ui1);
                    else
                        return (null, null);

                case BuildingCategory.Stockpile:
                    var ui2 = SelectedBuildingPanel.GetComponentsInChildren<StockpileUiBehaviour>(true).First();
                    ui2.CallingStockpile = buildingBehaviourHit.GetComponentInChildren<StockpileBehaviour>(); // niet inactieve ophalen
                    if (ui2.CallingStockpile != null)
                        return (buildingBehaviourHit, ui2);
                    else
                        return (null, null);

                case BuildingCategory.Tavern:
                    var ui3 = SelectedBuildingPanel.GetComponentsInChildren<UiTavernBehaviour>(true).First();
                    ui3.CallingTavern = buildingBehaviourHit.GetComponentInChildren<TavernBehaviour>(); // niet inactieve ophalen
                    if (ui3.CallingTavern != null)
                        return (buildingBehaviourHit, SelectedBuildingPanel.GetComponentsInChildren<UiTavernBehaviour>(true).First());
                    else
                        return (null, null);
            }
        }

        return (null, null);
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