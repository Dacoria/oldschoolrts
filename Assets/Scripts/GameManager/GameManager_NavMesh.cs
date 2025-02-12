﻿using UnityEngine;
using UnityEngine.AI;
using NavMeshSurface = Unity.AI.Navigation.NavMeshSurface;

public partial class GameManager : BaseAEMonoCI
{
    public GameObject RoadNavMeshSurface;

    public NavMeshAgent Agent; // voor path check
      
    public void RegisterRoad(Transform transform)
    {
        transform.gameObject.tag = Constants.TAG_ROAD;
        transform.SetParent(RoadNavMeshSurface.transform);
        RoadNavMeshSurface.GetComponent<NavMeshSurface>().BuildNavMesh();
        ManagePossibleNewSerfOrders();
    }

    private bool HasPathForBuilding(StockpileBehaviour stockpile, Vector3 targetBuildingLoc, int areaMask, UnityEngine.AI.NavMeshPath path)
    {
        return IsFullPathAvailable(stockpile.Location, targetBuildingLoc, areaMask, path);
    }

    private bool IsFullPathAvailable(Vector3 loc1, Vector3 loc2, int areaMask, UnityEngine.AI.NavMeshPath path)
    {
        var hasPartialPath = NavMesh.CalculatePath(loc1, loc2, areaMask, path);
        return path.status == NavMeshPathStatus.PathComplete;
    }
}