using UnityEngine;

public partial class UiManager : MonoBehaviour
{
    private void DisableActiveBuilding()
    {
        DisableActiveOutline();
        DisableActiveDisplayRange();
        DisableActiveDisplayBuildingInputOutputHandler();
        DisableActiveDisplayBuildingNameImgHandler();
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

    private void DisableEntireCanvas()
    {
        SetAllCanvasItemsToInactive(SelectedBuildingPanel);
        SetAllCanvasItemsToInactive(SelectedBuildingPanelSkillTree);

        SelectedBuildingPanel.SetActive(false);
        SelectedBuildingPanelSkillTree.SetActive(false);
    }

    private void SetAllCanvasItemsToInactive(GameObject selectedBuildingPanel)
    {
        foreach (Transform transform in selectedBuildingPanel.transform)
        {
            transform.gameObject.SetActive(false);
        }
    }
}