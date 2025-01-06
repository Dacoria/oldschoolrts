using System.Collections.Generic;
using System.Linq;

public class BuildingForVillagerManager : BaseAEMonoCI
{
    public List<WorkerBuildingBehaviour> WorkerBuildingBehavioursNotCoupled = new List<WorkerBuildingBehaviour>();
    public static BuildingForVillagerManager Instance;

    private new void Awake()
    {
        base.Awake();
        Instance = this;
    }

    protected override void OnBuildingNeedsWorker(WorkerBuildingBehaviour behaviour)
    {
        WorkerBuildingBehavioursNotCoupled.Add(behaviour);        
    }

    public WorkerBuildingBehaviour TryCoupleBuildingWithVillager(IVillagerUnit villager, List<BuildingType> buildingTypesToMatch)
    {
        var matchedBuilding = WorkerBuildingBehavioursNotCoupled.FirstOrDefault(x => buildingTypesToMatch.Any(y => x.BuildingBehaviour.BuildingType == y));
        if(matchedBuilding != null)
        {
            matchedBuilding.Worker = villager;
            WorkerBuildingBehavioursNotCoupled.Remove(matchedBuilding);
            return matchedBuilding;
        }

        return null;
    }
}