using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class UIUpdateBuildRoadsFromEditorStatus : MonoBehaviour
{
    public Text BuildRoadsFromEditorText;    

    private void OnGUI()
    {
        if(EditorSettings.BuildRoadOnMouseActivity_Active)
        {
            BuildRoadsFromEditorText.text = "Build Editor Active -> Road";
        }
        else if (EditorSettings.BuildBuildingOnMouseActivity_Active)
        {
            var buildingToBuild = EditorSettings.SelectedBuildingType.Value.ToString().Capitalize();
            BuildRoadsFromEditorText.text = "Build Editor Active -> " + buildingToBuild;
        }
        else
        {
            BuildRoadsFromEditorText.text = "";
        }
    }
}
