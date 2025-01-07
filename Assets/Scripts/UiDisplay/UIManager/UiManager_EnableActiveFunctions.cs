using UnityEngine;

public partial class UiManager : MonoBehaviour
{
    [HideInInspector] public DisplayBuildingInputOutputHandler ActiveDisplayBuildingInputOutputHandler;

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