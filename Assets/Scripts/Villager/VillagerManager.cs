using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VillagerManager : BaseAEMonoCI
{
    public static VillagerManager Instance;

    private List<IVillagerUnit> VillagerUnits;

    private List<SerfBehaviour> Serfs = new List<SerfBehaviour>();
    private List<BuilderBehaviour> Builders = new List<BuilderBehaviour>();
    private List<WorkManager> Workers = new List<WorkManager>();

    public List<SerfBehaviour> GetSerfs() => Serfs;
    public List<BuilderBehaviour> GetBuilders() => Builders;
    public List<WorkManager> GetWorkers() => Workers;

    private new void Awake()
    {
        base.Awake();
        Instance = this;
        RefreshVillagerUnits();
    }

    protected override void OnNewVillagerUnit(IVillagerUnit newVillagerUnit) => RefreshVillagerUnits();

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
                    Workers.Add((WorkManager)villagerUnit);
                    break;
                default:
                    throw new System.Exception($"VillagerManager -> VillagerUnitType '{villagerUnit.GetVillagerUnitType()}' is niet ondersteund");
            }
        }
    }

    public VillagerUnitType DetermineVillagerUnitType(GameObject go)
    {
        var typeInSameGo = go.GetComponent<IVillagerUnit>();
        if(typeInSameGo != null)
        {
            return typeInSameGo.GetVillagerUnitType();
        }

        var typeInChildrenOfGo = go.GetComponentInChildren<IVillagerUnit>();
        if (typeInChildrenOfGo != null)
        {
            return typeInChildrenOfGo.GetVillagerUnitType();
        }
        else
        {
            throw new System.Exception($"VillagerManager -> No IVillagerUnit found in go: {go.name}");
        }
    }
}