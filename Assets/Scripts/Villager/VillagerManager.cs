using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VillagerManager : BaseEventCallback
{
    public static VillagerManager Instance;

    private List<IVillagerUnit> VillagerUnits;

    private List<SerfBehaviour> Serfs = new List<SerfBehaviour>();
    private List<BuilderBehaviour> Builders = new List<BuilderBehaviour>();
    private List<WorkerBuildingBehaviour> Workers = new List<WorkerBuildingBehaviour>();

    public List<SerfBehaviour> GetSerfs() => Serfs;
    public List<BuilderBehaviour> GetBuilders() => Builders;
    public List<WorkerBuildingBehaviour> GetWorkers() => Workers;

    private new void Awake()
    {
        base.Awake();
        Instance = this;
        RefreshVillagerUnits();
    }

    protected override void OnVillagerUnitCreated(VillagerUnitType villagerUnitType) => RefreshVillagerUnits();

    private void RefreshVillagerUnits()
    {
        VillagerUnits = FindObjectsOfType<MonoBehaviour>(true).OfType<IVillagerUnit>().ToList();

        Serfs.Clear();
        Builders.Clear();
        Workers.Clear();

        foreach (var villagerUnit in VillagerUnits)
        {
            switch(villagerUnit.GetVillagerUnitType())
            {
                case VillagerUnitType.Serf:
                    Serfs.Add((SerfBehaviour)villagerUnit); 
                    break;
                case VillagerUnitType.Builder:
                    Builders.Add((BuilderBehaviour)villagerUnit);
                    break;
                case VillagerUnitType.StoneMason:
                case VillagerUnitType.Forrester:
                case VillagerUnitType.Farmer:
                case VillagerUnitType.Fisherman:
                case VillagerUnitType.Hunter:
                case VillagerUnitType.Gatherer:
                    Workers.Add((WorkerBuildingBehaviour)villagerUnit);
                    break;
                default:
                    throw new System.Exception($"VillagerManager -> VillagerUnitType '{villagerUnit.GetVillagerUnitType()}' is niet ondersteund");
            }
        }
    }
}