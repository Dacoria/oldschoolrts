using System.Collections.Generic;
using UnityEngine;

public static class GOExtensions
{    
    public static Vector3 EntranceExit(this GameObject go)
    {
        if (go == null)
        {
            return new Vector3(0, 0, 0);
        }

        Transform root = null;
        if (go.transform.parent != null)
        {
            root = go.transform.parent;
        } else
        {
            root = go.transform;
        }
        foreach (Transform child in root)
        {
            if (child.tag == Constants.TAG_ENTRANCE_EXIT)
            {
                return child.position;
            }
        }

        return go.transform.position;
    }

    public static bool IsRoad(this GameObject go)
    {
        return RootNameGoStartsWith(go, "Road");
    }

    public static bool IsFarmField(this GameObject go)
    {
        return RootNameGoStartsWith(go, "FarmField");
    }

    private static bool RootNameGoStartsWith(GameObject go, string startsWithText)
    {
        if(go == null)
        {
            return false;
        }

        var currGo = go;
        do
        {
            currGo = currGo.transform.parent != null ? currGo.transform.parent.gameObject : currGo;

        } while (currGo.transform.parent != null);

        return currGo.name.StartsWith(startsWithText);
    }

    public static List<KeyCodeAction> KeyCodeActionList =
    new List<KeyCodeAction>
    {
            new KeyCodeAction(KeyCode.U, KeyCodeActionType.ToggleInputOutputDisplay),
            new KeyCodeAction(KeyCode.I, KeyCodeActionType.ToggleBuildingProgressDisplay),
            new KeyCodeAction(KeyCode.O, KeyCodeActionType.ToggleEntranceExitDisplay),
    };

    public static void SetDirectChildrenActive(this GameObject go) => SetDirectChildrenSetActive(go, isActive: true);
    public static void SetDirectChildrenInactive(this GameObject go) => SetDirectChildrenSetActive(go, isActive: false);
    public static void SetDirectChildrenSetActive(this GameObject go, bool isActive)
    {
        for (var i = 0; i < go.transform.childCount; i++)
        {
            var child = go.transform.GetChild(i);
            child.gameObject.SetActive(isActive);
        }
    }
}