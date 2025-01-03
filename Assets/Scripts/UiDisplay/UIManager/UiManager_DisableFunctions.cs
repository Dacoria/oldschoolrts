using UnityEngine;

public partial class UiManager : MonoBehaviour
{
    private void DisableActiveBuilding()
    {
        DisableActiveOutline();
        DisableActiveDisplayRange();
        DisableActiveDisplayBuildingInputOutputHandler();
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

    public void DisableEntireCanvas()
    {
        SetAllCanvasItemsToInactive(SelectedBuildingPanel);
        SelectedBuildingPanel.SetActive(false);
    }

    private void SetAllCanvasItemsToInactive(GameObject selectedBuildingPanel)
    {
        foreach (Transform transform in selectedBuildingPanel.transform)
        {
            transform.gameObject.SetActive(false);
        }
    }
}