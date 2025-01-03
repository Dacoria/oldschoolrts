using UnityEngine;

public class KeyCodeAction
{
    public KeyCodeAction(KeyCode keyCode, KeyCodeActionType keyCodeActionType)
    {
        KeyCode = keyCode;
        KeyCodeActionType = keyCodeActionType;
    }

    public KeyCodeActionType KeyCodeActionType { get;set; }
    public KeyCode KeyCode { get; set; }
    public bool Active { get; set; }

}