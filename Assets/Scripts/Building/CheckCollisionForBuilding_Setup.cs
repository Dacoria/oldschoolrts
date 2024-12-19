using UnityEngine;

public partial class CheckCollisionForBuilding : MonoBehaviourCI
{
    // LET OP: Dit script wordt 2x gebruikt (via bool IsRoadCollider):
    // 1x voor 'standaard check' of gebouw ander gebouw raakt (moet 1 leegruimte tussen zitten)
    // 1x voor 'weg check' (via los GO) of gebouw een weg/road raakt (geen leegruimte nodig) 
    // Road heeft zelf natuurlijk dit maar 1x

    // Via OnEnable wordt de road-variant aangemaakt (instellen settings daarvan gebeurt in de parent; niet road-variant)

    [HideInInspector] public bool IsRoadCollider;
    [HideInInspector][ComponentInject(Required.OPTIONAL)] public CheckResourceCollisionForBuilding CheckResourceCollisionForBuilding;

    [ComponentInject] private BoxCollider boxCollider;
    [ComponentInject] private BuildingBehaviour buildingBehaviour;
    
    private CheckCollisionForBuilding roadColliderCheckScript;

    private bool isBuildingBeingBuild;

    private int terrainLayer = 1 << Constants.LAYER_TERRAIN;
    private int roadLayer = 1 << Constants.LAYER_ROAD;
    private int farmFieldLayer = 1 << Constants.LAYER_FARM_FIELD;

    private void OnEnable()
    {       
        if (IsRoadCollider)
        {
            return;
        }

        isBuildingBeingBuild = IsBuildingBeingBuild();

        // Gebouw dat je gaat bouwen (dit GO) heeft geen trigger, de andere juist wel (gaat weer aan via OnDisable)
        boxCollider.isTrigger = false;         

        if (isBuildingBeingBuild)
        {
            InitRoadColliderGo();            
            boxCollider.size += new Vector3(1, 1, 1);
        }

        SetLayersOnColliders();
    }    

    private void OnDisable()
    {
        StopAllCoroutines(); // voor delays in updaten; deze wil je ook weghebben
        if (IsRoadCollider)
        {
            return;
        }

        if (IsBuildingBeingBuild())
        {
            boxCollider.size -= new Vector3(1, 1, 1);
        }

        Destroy(roadColliderCheckScript?.gameObject);
        boxCollider.excludeLayers = terrainLayer;
        boxCollider.isTrigger = true;
    }

    private void SetLayersOnColliders()
    {
        if (isBuildingBeingBuild)
        {
            boxCollider.excludeLayers = terrainLayer | roadLayer | farmFieldLayer;

            var roadCollider = roadColliderCheckScript.GetComponent<BoxCollider>();
            roadCollider.includeLayers = roadLayer | farmFieldLayer;
        }
        else
        {
            boxCollider.excludeLayers = terrainLayer;
        }
    }

    private bool IsBuildingBeingBuild()
    {
        return !IsRoadCollider &&
            GetComponentInChildren<GhostBuildingBehaviour>() != null &&
            !this.gameObject.IsFarmField() &&
            !this.gameObject.IsRoad();
    }

    private void InitRoadColliderGo()
    {
        var roadColliderGo = new GameObject { name = "RoadCollider" }; // direct initialized
        roadColliderGo.transform.parent = this.transform;

        roadColliderCheckScript = roadColliderGo.AddComponent<CheckCollisionForBuilding>();
        roadColliderCheckScript.IsRoadCollider = true;

        var roadColliderBoxCollider = roadColliderGo.AddComponent<BoxCollider>();
        roadColliderBoxCollider.size = boxCollider.size;
        roadColliderBoxCollider.center = boxCollider.center;
        roadColliderBoxCollider.isTrigger = false;

        var roadColliderRigidBody = roadColliderGo.AddComponent<Rigidbody>();
        roadColliderRigidBody.useGravity = false;
        roadColliderRigidBody.isKinematic = true;
    }   
}