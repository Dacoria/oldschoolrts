using System;
using UnityEngine;

public interface ICardBuilding
{
    int GetCount(Enum type);
    void AddItem(Enum type);
    void DecreaseItem(Enum type);
    float GetProductionTime(Enum type);
    ProductionSetting GetCardDisplaySetting(Enum type);
    bool CanProces(Enum type);
    QueueForBuildingBehaviour GetQueueForBuildingBehaviour();
    GameObject GetGameObject();
    UIItemProcessing GetCurrentItemProcessed();
}