using UnityEngine;
using UnityEngine.AI;
using NavMeshSurface = Unity.AI.Navigation.NavMeshSurface;

public partial class GameManager : BaseAEMonoCI
{
    public GameObject RoadNavMeshSurface;

    public NavMeshAgent Agent; // voor path check

    private NavMeshSurface _roadNavMeshSurfaceComponent;
    private NavMeshSurface roadNavMeshSurfaceComponent
    {
        get {
            if (_roadNavMeshSurfaceComponent == null)
            {                
                _roadNavMeshSurfaceComponent = RoadNavMeshSurface.GetComponent<NavMeshSurface>();
            }
            return _roadNavMeshSurfaceComponent;
        }
        set => _roadNavMeshSurfaceComponent = value;
    }
      
    public void RegisterRoad(Transform transform)
    {
        transform.gameObject.tag = Constants.TAG_ROAD;
        transform.SetParent(RoadNavMeshSurface.transform);
        roadNavMeshSurfaceComponent.BuildNavMesh();
        ManagePossibleNewSerfOrders();
    }

    private bool HasPathForBuilding(StockpileBehaviour stockpile, Vector3 targetBuildingLoc, int areaMask, UnityEngine.AI.NavMeshPath path)
    {
        return IsFullPathAvailable(stockpile.Location, targetBuildingLoc, areaMask, path);
    }

    private bool IsFullPathAvailable(Vector3 loc1, Vector3 loc2, int areaMask, UnityEngine.AI.NavMeshPath path)
    {
        var hasPartialPath = UnityEngine.AI.NavMesh.CalculatePath(loc1, loc2, areaMask, path);
        return path.status == NavMeshPathStatus.PathComplete;
    }
}