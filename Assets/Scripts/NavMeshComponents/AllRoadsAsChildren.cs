using UnityEngine;

public class AllRoadsAsChildren : MonoBehaviour
{
    void Awake()
    {
        var allRoads = GameObject.FindGameObjectsWithTag(Constants.TAG_ROAD);
        foreach (var road in allRoads)
        {
            road.transform.parent = transform;
        }
    }
}