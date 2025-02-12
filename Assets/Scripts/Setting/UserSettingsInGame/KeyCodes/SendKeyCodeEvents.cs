using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class SendKeyCodeEvents : MonoBehaviour
{
    private List<KeyCodeAction> RegistedKeyCodesList;

    private void Start()
    {
        RegistedKeyCodesList = UIUserSettings.KeyCodeActionList.ToList();
    }

    void Update()
    {
        foreach(var keyCodeRegistredItem in RegistedKeyCodesList)
        {            
            if (Input.GetKeyDown(keyCodeRegistredItem.KeyCode))
            {
                DoKeyCodeAction(keyCodeRegistredItem);
            }
        }
    }

    private void DoKeyCodeAction(KeyCodeAction keyCodeAction)
    {
        switch (keyCodeAction.KeyCodeActionType)
        {
            case KeyCodeActionType.ToggleInputOutputDisplay:

                KeyCodeStatusSettings.ToggleInputOutputDisplay_Active = !KeyCodeStatusSettings.ToggleInputOutputDisplay_Active;
                keyCodeAction.Active = KeyCodeStatusSettings.ToggleInputOutputDisplay_Active;
                AE.KeyCodeAction?.Invoke(keyCodeAction);
                break;
            case KeyCodeActionType.ToggleBuildingProgressDisplay:

                KeyCodeStatusSettings.ToggleBuildingProgressDisplay_Active = !KeyCodeStatusSettings.ToggleBuildingProgressDisplay_Active;
                keyCodeAction.Active = KeyCodeStatusSettings.ToggleBuildingProgressDisplay_Active;
                AE.KeyCodeAction?.Invoke(keyCodeAction);
                break;
            case KeyCodeActionType.ToggleEntranceExitDisplay:

                KeyCodeStatusSettings.ToggleEntranceExitDisplay_Active = !KeyCodeStatusSettings.ToggleEntranceExitDisplay_Active;
                keyCodeAction.Active = KeyCodeStatusSettings.ToggleEntranceExitDisplay_Active;
                AE.KeyCodeAction?.Invoke(keyCodeAction);
                break;     
            default:
                throw new Exception($"Actie voor keyCodeActionType {keyCodeAction.KeyCodeActionType} is niet gedefinieerd");
        }    
    }
}