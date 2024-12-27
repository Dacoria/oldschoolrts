using UnityEngine;

public interface IVillagerUnit
{
    VillagerUnitType GetVillagerUnitType();
    GameObject GetGO();
    public bool IsVillagerWorker() => GetVillagerUnitType() != VillagerUnitType.Serf && GetVillagerUnitType() != VillagerUnitType.Builder;
}