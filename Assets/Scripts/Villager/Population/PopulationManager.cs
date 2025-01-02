using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PopulationManager : BaseAEMonoCI
{
    public static int PopulationLimit = 10;
    public static int CurrentPopulation = 0;
    public static int PopulationBeingCreated = 0;

    public static PopulationManager Instance;
    private new void Awake()
    {
        base.Awake();
        Instance = this;        
    }

    protected override void OnStartedProducingAction(BuildingBehaviour building, List<Enum> typesToProduce)
    {
        if(Enum.IsDefined(typeof(VillagerUnitType), typesToProduce.First()))
        {
            PopulationBeingCreated++;
        }
    }

    protected override void OnFinishedProducingAction(BuildingBehaviour building, List<Enum> typesToProduce)
    {
        if (Enum.IsDefined(typeof(VillagerUnitType), typesToProduce.First()))
        {
            PopulationBeingCreated--;
        }
    }

    public static bool HasPopulationRoom => CurrentPopulation + PopulationBeingCreated < PopulationLimit;
}
