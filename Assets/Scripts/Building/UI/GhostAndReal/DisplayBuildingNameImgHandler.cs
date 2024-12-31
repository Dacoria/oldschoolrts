using System.Linq;
using UnityEngine;

public class DisplayBuildingNameImgHandler : BaseAEMonoCI
{
    private SetTextOfBuilding text;
    private GameObject imageGo;

    new void OnEnable()
    {
        base.OnEnable();
        text = GetComponentInChildren<SetTextOfBuilding>();
        imageGo = GetComponentsInChildren<MeshRenderer>()?.Where(x => x.gameObject.name.StartsWith("Image")).FirstOrDefault()?.gameObject;

        text.gameObject.SetActive(KeyCodeStatusSettings.ToggleBuildingNameImgDisplay_Active);
        imageGo?.SetActive(KeyCodeStatusSettings.ToggleBuildingNameImgDisplay_Active);
    }  

    protected override void OnKeyCodeAction(KeyCodeAction action)
    {
        if(action.KeyCodeActionType == KeyCodeActionType.ToggleBuildingNameImgDisplay)
        {
            UpdateEnabledStatusOfDisplayObjects(KeyCodeStatusSettings.ToggleBuildingNameImgDisplay_Active);
        }
    }

    public void UpdateEnabledStatusOfDisplayObjects(bool show)
    {
        text?.gameObject?.SetActive(show);
        imageGo?.SetActive(show);
    }
}
