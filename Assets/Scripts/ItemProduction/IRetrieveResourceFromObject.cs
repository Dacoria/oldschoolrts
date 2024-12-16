public interface IRetrieveResourceFromObject
{
    HarvestMaterialResource ResourceIsRetrieved();
    bool CanRetrieveResource();
    void StartRetrievingResource(int materialNumberRequestedToHarvest = 1);
}