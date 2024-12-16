using UnityEngine;

public class HideShowInputOutputScript : MonoBehaviour
{
    private void Start()
    {
        ActionEvents.KeyCodeAction += KeyCodeActionChanged;
        gameObject.SetActive(KeyCodeStatusSettings.ToggleInputOutputDisplay_Active);
    }

    private void KeyCodeActionChanged(KeyCodeAction keyCodeAction)
    {
        if(keyCodeAction.KeyCodeActionType == KeyCodeActionType.ToggleInputOutputDisplay)
        {
            gameObject.SetActive(keyCodeAction.Active);
        }
    }
}
