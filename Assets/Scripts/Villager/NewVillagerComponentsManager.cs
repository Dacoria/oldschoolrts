public class NewVillagerComponentsManager : BaseAEMonoCI
{
    // niet via event - dat is te traag voor de awake
    public static void NewVillagerUnit(IVillagerUnit newVillagerUnit)
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
            // go.AddComponent<ResourceCarryingBehaviour>(); // Vanwege verschillende prefabs + settings -> zelf toevoegen
            // go.AddComponent<RetrieveResourceBehaviour>(); // Vanwege verschillende prefabs + settings -> zelf toevoegen
            go.AddComponent<GoingToWorkStation>();
            go.AddComponent<WarningDisplayAboveHeadBehaviour>();
        }        

        // meer specifieke dingen staan nog op GO zelf! (bv tool carrying)
    }


    // Alleen checks/validaties....
    protected override void OnNewVillagerUnit(IVillagerUnit newVillagerUnit)
    {
        var go = newVillagerUnit.GetGO();
        if (go.GetComponent<PopulationEligibleBehaviour>() == null)
        {
            throw new System.Exception($"Geen standaard behaviours gevonden op nieuwe unit --> Check awake van villager '{go.name}', type '{newVillagerUnit.GetVillagerUnitType()}'");
        }

        if(newVillagerUnit.IsVillagerWorker() && go.GetComponent<ResourceCarryingBehaviour>() == null)
        {
            throw new System.Exception($"Geen ResourceCarryingBehaviour gevonden op villager worker --> '{go.name}', type '{newVillagerUnit.GetVillagerUnitType()}'");
        }

        if (newVillagerUnit.IsVillagerWorker() && go.GetComponent<RetrieveResourceBehaviour>() == null)
        {
            throw new System.Exception($"Geen RetrieveResourceBehaviour gevonden op villager worker --> '{go.name}', type '{newVillagerUnit.GetVillagerUnitType()}'");
        }
    }
}