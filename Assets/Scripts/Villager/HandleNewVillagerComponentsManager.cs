public class HandleNewVillagerComponentsManager : BaseAEMonoCI
{
    protected override void OnNewVillagerUnit(IVillagerUnit newVillagerUnit)
    {
        var go = newVillagerUnit.GetGO();
        var type = newVillagerUnit.GetVillagerUnitType();

        //Shared
        go.AddComponent<PopulationEligibleBehaviour>();
        go.AddComponent<FoodConsumptionBehaviour>();
        go.AddComponent<HealthBehaviour>();
        go.AddComponent<NavMeshStuckFixer>();

        // tooltips
        if (type == VillagerUnitType.Serf)
            go.AddComponent<TooltipSerf>();

        else if(type == VillagerUnitType.Builder)
            go.AddComponent<TooltipBuilder>();

        else if(newVillagerUnit.IsVillagerWorker())
            go.AddComponent<TooltipWorker>();

        else
            throw new System.Exception($"Type villager '{type}' onbekend");

        // serf stuff
        if (type == VillagerUnitType.Serf)
        {
            go.AddComponent<SerfResourceCarryingBehaviour>();
        }

        // Worker stuff
        if (newVillagerUnit.IsVillagerWorker())
        {
            go.AddComponent<WorkManager>();
            go.AddComponent<ResourceCarryingBehaviour>();
            go.AddComponent<GoingToWorkStation>();
            go.AddComponent<RetrieveResourceBehaviour>();
        }        

        // meer specifieke dingen staan nog op GO zelf! (bv tool carrying)
    }
}