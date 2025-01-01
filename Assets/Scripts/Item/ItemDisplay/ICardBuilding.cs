using System;
using UnityEngine;

public interface ICardBuilding
{
    int GetCount(Enum type);
    void AddType(Enum type);
    void DecreaseType(Enum type);
    float GetProductionTime(Enum type);
    ProductionSetting GetCardDisplaySetting(Enum type);
    bool CanProces(Enum type);
    GameObject GetGameObject();
    UIItemProcessing GetCurrentItemProcessed();


    void AddTypes(Enum type, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            AddType(type);
        }
    }
    void DecreaseTypes(Enum type, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            DecreaseType(type);
        }
    }
}