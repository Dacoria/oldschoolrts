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
    GameObject GetGameObject();
    UIItemProcessing GetCurrentItemProcessed();
    void AddItems(Enum type, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            AddItem(type);
        }
    }
    void DecreaseItems(Enum type, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            DecreaseItem(type);
        }
    }
}