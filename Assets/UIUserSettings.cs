using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIUserSettings : MonoBehaviour
{
    // CHECKBOX UI
    public void ToggleInstaBuild()
    {
        BuildBuildingsByUser.Instance.InstaBuild = !BuildBuildingsByUser.Instance.InstaBuild;
        BuildBuildingsByUser.Instance.ClearSelectedGameObjectToBuild(); // leegmaken selectie bij UI Click --> TODO: Automatisch maken!
    }

    // CHECKBOX UI
    public void ToggleGetsHungry()
    {
        FoodConsumptionSettings.ToggleUseFoodConsumption_Active = !FoodConsumptionSettings.ToggleUseFoodConsumption_Active;
        BuildBuildingsByUser.Instance.ClearSelectedGameObjectToBuild();// leegmaken selectie bij UI Click --> TODO: Automatisch maken!
    }
}
