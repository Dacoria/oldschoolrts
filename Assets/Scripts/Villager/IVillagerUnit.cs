using UnityEngine;

public interface IVillagerUnit
{
    VillagerUnitType GetVillagerUnitType();
    GameObject GetGO();
    bool IsVillagerWorker();
}