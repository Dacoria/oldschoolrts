using UnityEngine;

public class HideShowInputOutputScript : BaseAEMono
{
    private void Start()
    {
        gameObject.SetActive(KeyCodeStatusSettings.ToggleInputOutputDisplay_Active);
    }

    protected override void OnKeyCodeAction(KeyCodeAction keyCodeAction)
    {
        if(keyCodeAction.KeyCodeActionType == KeyCodeActionType.ToggleInputOutputDisplay)
        {
            gameObject.SetActive(keyCodeAction.Active);
        }
    }
}
