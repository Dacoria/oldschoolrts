using System.Collections.Generic;
using UnityEngine;

public class UIUserSettings : MonoBehaviour
{
    public static bool ShowExternalUIElementsActiveBuilding = false; // voor nu: alleen via code aan te passen; niet UI

    public static List<KeyCodeAction> KeyCodeActionList =
    new List<KeyCodeAction>
    {
            new KeyCodeAction(KeyCode.U, KeyCodeActionType.ToggleInputOutputDisplay),
            new KeyCodeAction(KeyCode.I, KeyCodeActionType.ToggleBuildingProgressDisplay),
            new KeyCodeAction(KeyCode.O, KeyCodeActionType.ToggleEntranceExitDisplay),
    };

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

    // CHECKBOX UI
    public void ToggleInstaFreeUnits()
    {
        BattleManager.ToggleInstaFreeUnits_Active = !BattleManager.ToggleInstaFreeUnits_Active;
        BuildBuildingsByUser.Instance.ClearSelectedGameObjectToBuild();// leegmaken selectie bij UI Click --> TODO: Automatisch maken!
    }

    // CHECKBOX UI
    public void ToggleInstaFreeVillagers()
    {
        VillagerManager.ToggleInstaFreeVillagers_Active = !VillagerManager.ToggleInstaFreeVillagers_Active;
        BuildBuildingsByUser.Instance.ClearSelectedGameObjectToBuild();// leegmaken selectie bij UI Click --> TODO: Automatisch maken!
    }
}