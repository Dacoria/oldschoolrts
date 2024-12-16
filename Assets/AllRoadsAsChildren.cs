using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllRoadsAsChildren : MonoBehaviour
{
    void Awake()
    {
        var allRoads = GameObject.FindGameObjectsWithTag(StaticHelper.TAG_ROAD);
        foreach (var road in allRoads)
        {
            road.transform.parent = transform;
        }
    }

}
