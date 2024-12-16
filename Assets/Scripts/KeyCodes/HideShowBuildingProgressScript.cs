using UnityEngine;

public class HideShowBuildingProgressScript : MonoBehaviour
{
    private void Start()
    {
        ActionEvents.KeyCodeAction += KeyCodeActionChanged;
        gameObject.SetActive(KeyCodeStatusSettings.ToggleBuildingProgressDisplay_Active);
    }

    private void KeyCodeActionChanged(KeyCodeAction keyCodeAction)
    {
        if(keyCodeAction.KeyCodeActionType == KeyCodeActionType.ToggleBuildingProgressDisplay)
        {
            gameObject.SetActive(keyCodeAction.Active);
        }
    }
}
