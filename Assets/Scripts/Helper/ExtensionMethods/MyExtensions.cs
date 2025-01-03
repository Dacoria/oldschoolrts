using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Assets;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public static class MyExtensions
{
    public static int ToAreaMask(this Purpose purpose)
    {
        switch (purpose)
        {
            case Purpose.ROAD:
                return 1 << 0;
            default:
                return 1 << 3;
        }
    }

    public static T Pop<T>(this SortedSet<T> sortedSet)
    {
        var obj = sortedSet.Min();
        if (obj != null)
        {
            sortedSet.Remove(obj);
        }

        return obj;
    }

    public static string TitleCase(this string text)
    {
        var textInfo = new CultureInfo("en-US", false).TextInfo;
        var test = textInfo.ToTitleCase(text.ToLower());
        return textInfo.ToTitleCase(text.ToLower());
    }

    public static bool StoppedAtDestination(this NavMeshAgent myNavMeshAgent, float additionalStoppingDistance = 0)
    {
        return myNavMeshAgent.isActiveAndEnabled &&
                !myNavMeshAgent.pathPending &&
                myNavMeshAgent.pathStatus != NavMeshPathStatus.PathInvalid &&
                myNavMeshAgent.remainingDistance <= myNavMeshAgent.stoppingDistance + additionalStoppingDistance &&
                myNavMeshAgent.velocity.sqrMagnitude == 0f;
    }

    public static bool IsSameXAndZ(this Vector3 a, Vector3 b)
    {
        return a.x == b.x && a.z == b.z;
    }

    public static bool IsSameVector3(this Vector3 a, Vector3 b)
    {
        var sqrDiff = Vector3.SqrMagnitude(a - b);
        return sqrDiff < 0.001;
    }

    public static bool IsEmptyVector(this Vector3 a)
    {
        return a.x == 0 && a.y == 0 && a.z == 0;
    }
    public static bool IsAlmostEmptyVector(this Vector3 a)
    {
        return Mathf.Abs(a.x) <= 0.1 && Mathf.Abs(a.y) <= 0.1 && Mathf.Abs(a.z) <= 0.1;
    }

    public static Vector3 MultiplyVector(this Vector3 a, Vector3 b)
    {
        return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
    }

    public static Vector3 Absolute(this Vector3 a)
    {
        return new Vector3(math.abs(a.x), math.abs(a.y), math.abs(a.z));
    }

    public static Bounds Copy(this Bounds b)
    {
        return new Bounds(new Vector3(b.center.x, b.center.y, b.center.z), new Vector3(b.size.x, b.size.y, b.size.z));
    }

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
            new KeyCodeAction(KeyCode.P, KeyCodeActionType.ToggleBuildingNameImgDisplay)
    };    

    public static Vector2 GetRandomVector(int minRange, int maxRange)
    {
        var randomX = UnityEngine.Random.Range(minRange, maxRange);
        var randomZ = UnityEngine.Random.Range(minRange, maxRange);

        var positiveNegativeX = UnityEngine.Random.Range(0, 2) == 1 ? 1 : -1;
        var positiveNegativeZ = UnityEngine.Random.Range(0, 2) == 1 ? 1 : -1;

        var x = Mathf.RoundToInt(randomX * positiveNegativeX);
        var z = Mathf.RoundToInt(randomZ * positiveNegativeZ);

        return new Vector2(x, z);
    }       

    public static string Capitalize(this string input, bool everyWord = true, string delimiter = "_")
    {
        if (everyWord)
        {
            var words = input.Split(delimiter);
            var result = words[0].Capitalize(false); ;
            for (int i = 1; i < words.Length; i++)
            {
                result += delimiter;
                result += words[i].Capitalize(false);                
                
            }

            return result;
        }
        else
        {
            return string.Concat(input[0].ToString().ToUpper(), input.ToLower().Substring(1, input.Length - 1));
        }        
    }

    public static T Next<T>(this T src) where T : struct
    {
        if (!typeof(T).IsEnum) throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(T).FullName));

        T[] Arr = (T[])Enum.GetValues(src.GetType());
        int j = Array.IndexOf<T>(Arr, src) + 1;
        return (Arr.Length == j) ? Arr[0] : Arr[j];
    }

    public static T PopClosest<T>(this List<T> behaviours, Vector3 objLocation) where T : MonoBehaviour
    {
        var closestBehaviour = behaviours.OrderBy(x => (x.transform.position - objLocation).sqrMagnitude).First();
        behaviours.Remove(closestBehaviour);
        return closestBehaviour;
    }
}