using System.Linq;
using UnityEngine;

public class DisplayBuildingNameImgHandler : BaseAEMonoCI
{
    private SetTextOfBuilding text;
    private GameObject imageGo;

    void Start()
    {
        text = GetComponentInChildren<SetTextOfBuilding>();
        imageGo = GetComponentsInChildren<MeshRenderer>().Where(x => x.gameObject.name.StartsWith("Image")).First().gameObject; // lelijk; maar minste werk :P

        text.gameObject.SetActive(KeyCodeStatusSettings.ToggleBuildingNameImgDisplay_Active);
        imageGo.SetActive(KeyCodeStatusSettings.ToggleBuildingNameImgDisplay_Active);
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
        text.gameObject.SetActive(show);
        imageGo.SetActive(show);
    }
}
