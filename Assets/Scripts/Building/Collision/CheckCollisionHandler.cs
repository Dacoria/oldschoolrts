using UnityEngine;

public class CheckCollisionHandler : BaseAEMonoCI
{
    [ComponentInject] private BuildingBehaviour buildingBehaviour;

    private int terrainLayer = 1 << Constants.LAYER_TERRAIN;
    private int roadLayer = 1 << Constants.LAYER_ROAD;
    private int farmFieldLayer = 1 << Constants.LAYER_FARM_FIELD;

    private bool isBuildingToBuild;
    private CheckCollision mainCollision;
    private CheckCollision roadCollision;
    private Vector3 placingBuildingSizeIncreaseCollider = new Vector3(1, 1, 1);

    private new void Awake()
    {
        base.Awake();
        test = this.gameObject;
    }

    private GameObject test;

    private void Start()
    {        
        isBuildingToBuild = IsBuildingBeingBuild();

        if (isBuildingToBuild)
        {
            CreateResourceCollisionIfNeeded();
            CreateBuildingAndRoadCollisions();            
        }
        else
        {
            CreateStandardCollision();
        }
    }

    public bool IsColliding()
    {
        if (isBuildingToBuild)
        {
            return mainCollision.IsColliding || roadCollision.IsColliding;
        }
        else
        {            
            return mainCollision.IsColliding;
        }
    }

    private void CreateResourceCollisionIfNeeded()
    {
        var miningResourceBehaviour = gameObject.GetComponentInChildren<ProduceResourceMiningBehaviour>(true);
        if (miningResourceBehaviour != null)
        {
            var checkResourceCollisionForBuilding = gameObject.AddComponent<CheckResourceCollisionForBuilding>();
            checkResourceCollisionForBuilding.MaterialResourceTypeToCheck = buildingBehaviour.BuildingType.GetMaterialResourceType();
        }
    }

    private void CreateBuildingAndRoadCollisions()
    {
        // main collider
        mainCollision = gameObject.AddComponent<CheckCollision>();               

        // road collider - losse GO
        var roadColliderGo = new GameObject("RoadOrFarmFieldCollider"); // direct initialized
        roadColliderGo.transform.parent = this.transform;
        roadColliderGo.transform.localPosition = Vector3.zero;

        roadCollision = roadColliderGo.AddComponent<CheckCollision>();

        var mainCollisionBoxCollider = mainCollision.GetComponent<BoxCollider>();

        var roadColliderBoxCollider = roadColliderGo.AddComponent<BoxCollider>();
        roadColliderBoxCollider.size = mainCollisionBoxCollider.size;
        roadColliderBoxCollider.center = mainCollisionBoxCollider.center;

        var roadColliderRigidBody = roadColliderGo.AddComponent<Rigidbody>();
        roadColliderRigidBody.useGravity = false;
        roadColliderRigidBody.isKinematic = true;

        // init (nadertijd; omdat maten van de main door road overgenomen moeten worden)
        mainCollision.Init(exclLayers: terrainLayer | roadLayer | farmFieldLayer, v3SizeChangeWhenEnabled: placingBuildingSizeIncreaseCollider);
        roadCollision.Init(inclLayers: roadLayer | farmFieldLayer);
    }

    private void CreateStandardCollision()
    {
        mainCollision = gameObject.AddComponent<CheckCollision>();
        mainCollision.Init(exclLayers: terrainLayer);
    }

    private bool IsBuildingBeingBuild()
    {
        return GetComponentInChildren<GhostBuildingBehaviour>() != null &&
            !this.gameObject.IsFarmField() &&
            !this.gameObject.IsRoad();
    }

    protected override void OnBuilderRequestStatusChanged(BuilderRequest builderRequest, BuildStatus buildStatus)
    {
        if (builderRequest.GameObject == this.gameObject && buildStatus != BuildStatus.NONE)
        {
            // gebouw is geplaatst -> collisioncheck niet meer nodig
            Destroy(mainCollision);
            Destroy(roadCollision?.gameObject); // aangezien dit een losse Go is -> hele GO verwijderen
            Destroy(this);
        }
    }
}