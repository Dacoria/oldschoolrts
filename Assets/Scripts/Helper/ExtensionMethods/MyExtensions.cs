using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class MyExtensions
{
    public static int ToAreaMask(this Purpose purpose)
    {
        switch (purpose)
        {
            case Purpose.ROAD:
                return 1 << Constants.LAYER_DEFAULT;
            default:
                return 1 << Constants.LAYER_TERRAIN;
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

    public static T PopClosest<T>(this List<T> behaviours, Vector3 objLocation) where T : MonoBehaviour
    {
        var closestBehaviour = behaviours.OrderBy(x => (x.transform.position - objLocation).sqrMagnitude).First();
        behaviours.Remove(closestBehaviour);
        return closestBehaviour;
    }

    public static T Next<T>(this T src) where T : struct
    {
        if (!typeof(T).IsEnum) throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(T).FullName));

        T[] Arr = (T[])Enum.GetValues(src.GetType());
        int j = Array.IndexOf<T>(Arr, src) + 1;
        return (Arr.Length == j) ? Arr[0] : Arr[j];
    }    

    public static T DeepClone<T>(this T obj)
    {
        using (var ms = new MemoryStream())
        {
            var formatter = new BinaryFormatter();
            formatter.Serialize(ms, obj);
            ms.Position = 0;

            return (T)formatter.Deserialize(ms);
        }
    }   
}