public class RetrieveResourceDummy : IRetrieveResourceFromObject
{
    private int MaterialNumberRequestedToHarvest; // via UI ingesteld
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