using UnityEngine;

public class HarvestableMaterialScript : MonoBehaviour, IRetrieveResourceFromObject
{
    public MaterialResourceType MaterialType;

    public int InitialMaterialCount;
    public int MaterialCount; // voor evt resetten materials
    public bool ResourceIsBeingRetrieved { private set; get; }
    private int MaterialNumberRequestedToHarvest;

    public bool DestroyOnNoMaterial = true;

    void Start()
    {
        if (MaterialType == MaterialResourceType.NONE) { throw new System.Exception("MaterialType is verplicht"); }
        InitialMaterialCount = MaterialCount;
    }

    private void NoMoreMaterialAction()
    {
        if(MaterialType == MaterialResourceType.WOODLOG)
        {
            transform.gameObject.AddComponent<TreeFallingScript>();
            return;
        }

        if(!DestroyOnNoMaterial)
        {
            ResourceIsBeingRetrieved = false;
            return;
        }

        this.gameObject.AddComponent<MoveObjectDownSurfaceAndDestroy>();

        if(
            MaterialType == MaterialResourceType.COALORE ||
            MaterialType == MaterialResourceType.IRONORE
        )           
        {
            gameObject.GetComponent<MoveObjectDownSurfaceAndDestroy>().AlsoDestroyParent = false;
        }
    }

    public HarvestMaterialResource ResourceIsRetrieved()
    {
        if(MaterialCount <= 0)
        {
            NoMoreMaterialAction();
            return null;
        }
        if (MaterialCount > MaterialNumberRequestedToHarvest)
        {
            MaterialCount -= MaterialNumberRequestedToHarvest;
            ResourceIsBeingRetrieved = false;
            return new HarvestMaterialResource(MaterialType, MaterialNumberRequestedToHarvest);
        }

        var materialsLeft = MaterialCount;
        MaterialCount = 0;
        NoMoreMaterialAction();        
        return new HarvestMaterialResource(MaterialType, materialsLeft);
    }

    public bool CanRetrieveResource()
    {
        return !ResourceIsBeingRetrieved && MaterialCount > 0;
    }

    public void StartRetrievingResource(int materialNumberRequestedToHarvest = 1)
    {
        MaterialNumberRequestedToHarvest = materialNumberRequestedToHarvest;
        ResourceIsBeingRetrieved = true;
    }
}