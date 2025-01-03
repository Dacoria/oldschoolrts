using UnityEngine;

public partial class UiManager : MonoBehaviour
{
    private void EnableUIActiveBuilding(GameObject go)
    {
        var displayBuildingInputOutputHandler = go.GetComponentInChildren<DisplayBuildingInputOutputHandler>();
        if (displayBuildingInputOutputHandler != null && !displayBuildingInputOutputHandler.gameObject.IsRoad())
        {
            ActiveDisplayBuildingInputOutputHandler = displayBuildingInputOutputHandler;
            ActiveDisplayBuildingInputOutputHandler.UpdateEnabledStatusOfDisplayObjects(true);

            var displayBuildingNameImgHandler = go.GetComponent<DisplayBuildingNameImgHandler>();
            if (displayBuildingNameImgHandler != null)
            {
                ActiveDisplayBuildingNameImgHandler = displayBuildingNameImgHandler;
                ActiveDisplayBuildingNameImgHandler.UpdateEnabledStatusOfDisplayObjects(true);
            }
        }
    }
}