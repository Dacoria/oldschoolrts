using UnityEngine;

public class HideShowBuildingProgressScript : BaseAEMonoCI
{
    private void Start()
    {
        gameObject.SetActive(KeyCodeStatusSettings.ToggleBuildingProgressDisplay_Active);
    }

    protected override void OnKeyCodeAction(KeyCodeAction keyCodeAction)
    {
        if(keyCodeAction.KeyCodeActionType == KeyCodeActionType.ToggleBuildingProgressDisplay)
        {
            gameObject.SetActive(keyCodeAction.Active);
        }
    }
}