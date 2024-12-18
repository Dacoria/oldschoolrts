public class RetrieveResourceDummy : IRetrieveResourceFromObject
{
    private int MaterialNumberRequestedToHarvest;
    public bool CanRetrieveResource()
    {
        return true;
    }

    public HarvestMaterialResource ResourceIsRetrieved()
    {
        return new HarvestMaterialResource(MaterialResourceType.NONE, MaterialNumberRequestedToHarvest);
    }

    public void StartRetrievingResource(int materialNumberRequestedToHarvest = 1)
    {
        MaterialNumberRequestedToHarvest = materialNumberRequestedToHarvest;
    }
}