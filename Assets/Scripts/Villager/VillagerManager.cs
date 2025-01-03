using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VillagerManager : BaseAEMonoCI
{
    public static VillagerManager Instance;

    public static bool ToggleInstaFreeVillagers_Active = false;

    private List<IVillagerUnit> villagerUnits;

    private List<SerfBehaviour> serfs = new List<SerfBehaviour>();
    private List<BuilderBehaviour> suilders = new List<BuilderBehaviour>();
    private List<WorkManager> workers = new List<WorkManager>();

    public List<SerfBehaviour> GetSerfs() => serfs;
    public List<BuilderBehaviour> GetBuilders() => suilders;
    public List<WorkManager> GetWorkers() => workers;
    public List<IVillagerUnit> GetVillagers() => villagerUnits;

    private GameObject VillagersParentGo;

    private new void Awake()
    {
        base.Awake();
        Instance = this;
        VillagersParentGo = GameObject.Find("Villagers");
        RefreshVillagerUnits();
    }

    protected override void OnNewVillagerUnit(IVillagerUnit newVillagerUnit)
    {
        RefreshVillagerUnits();
    }    

    private void RefreshVillagerUnits()
    {
        villagerUnits = FindObjectsOfType<MonoBehaviour>(true).OfType<IVillagerUnit>().ToList();

        serfs.Clear();
        suilders.Clear();
        workers.Clear();

        foreach (var villagerUnit in villagerUnits)
        {
            ((MonoBehaviour)villagerUnit).transform.parent = VillagersParentGo.transform;

            var type = villagerUnit.GetVillagerUnitType();
            
            if(type == VillagerUnitType.Serf)
                serfs.Add((SerfBehaviour)villagerUnit);

            else if(type == VillagerUnitType.Builder)
                suilders.Add((BuilderBehaviour)villagerUnit);

            else if(villagerUnit.IsVillagerWorker())
                workers.Add((WorkManager)villagerUnit);

            else
                throw new System.Exception($"VillagerManager -> VillagerUnitType '{villagerUnit.GetVillagerUnitType()}' is niet ondersteund");
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
        
        throw new System.Exception($"VillagerManager -> No IVillagerUnit found in go: {go.name}");
    }
}