using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PopulationManager : BaseAEMonoCI
{
    public static int PopulationLimit = 7;
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
        if(Enum.TryParse<VillagerUnitType>(typesToProduce.First().ToString(), out var villagerType))
        {            
            PopulationBeingCreated++;
        }
    }

    protected override void OnNewVillagerUnit(IVillagerUnit newVillagerUnit)
    {
        PopulationBeingCreated = Math.Max(0, PopulationBeingCreated - 1);
        CurrentPopulation++;
    }

    public static bool HasPopulationRoom => CurrentPopulation + PopulationBeingCreated < PopulationLimit;
}
