using UnityEngine;

public partial class UiManager : MonoBehaviour
{
    private void EnableExternalUIElementsActiveBuilding(GameObject go)
    {
        var displayBuildingInputOutputHandler = go.GetComponentInChildren<DisplayBuildingInputOutputHandler>();
        if (displayBuildingInputOutputHandler != null && !displayBuildingInputOutputHandler.gameObject.IsRoad())
        {
            ActiveDisplayBuildingInputOutputHandler = displayBuildingInputOutputHandler;
            ActiveDisplayBuildingInputOutputHandler.UpdateEnabledStatusOfDisplayObjects(true);           
        }
    }
}