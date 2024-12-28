using UnityEngine;

public class WorkerBuildingBehaviour : MonoBehaviourCI
{
    [ComponentInject] public BuildingBehaviour BuildingBehaviour;
    public IVillagerUnit Worker;  

    private void Start()
    {
        if (BuildingBehaviour.CurrentBuildStatus != BuildStatus.COMPLETED_BUILDING)
            throw new System.Exception($"Alleen WorkerBuildingBehaviour bij completed building, status nu: {BuildingBehaviour.CurrentBuildStatus} in {BuildingBehaviour.gameObject.name}, type: {BuildingBehaviour.BuildingType}");

        AE.BuildingNeedsWorker?.Invoke(this);
    }
}