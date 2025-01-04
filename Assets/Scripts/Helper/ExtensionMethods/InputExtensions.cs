using System.Collections.Generic;
using UnityEngine;
public static class InputExtensions
{
    private static Dictionary<KeyCode, int> keyCodeDict = new Dictionary<KeyCode, int> {
        {KeyCode.Alpha0, 0},
        {KeyCode.Alpha1, 1},
        {KeyCode.Alpha2, 2},
        {KeyCode.Alpha3, 3},
        {KeyCode.Alpha4, 4},
        {KeyCode.Alpha5, 5},
        {KeyCode.Alpha6, 6},
        {KeyCode.Alpha7, 7},
        {KeyCode.Alpha8, 8},
        {KeyCode.Alpha9, 9},
    };

    public static bool TryGetNumberDown(out int number)
    {
        foreach(var keyCode in keyCodeDict.Keys)
        {
            if(Input.GetKeyDown(keyCode))
            {
                return keyCodeDict.TryGetValue(keyCode, out number);
            }    
        }

        number = -1;
        return false;
    }
}