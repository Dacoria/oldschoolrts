using System;
using System.Linq;
using UnityEngine;

public interface ICardBuilding
{
    void AddType(Enum type);
    int GetCount(Enum type) { return 0; } // alleen zonder queue interessant. TODO Lostrekken
    void DecreaseType(Enum type) { } // alleen zonder queue interessant. TODO Lostrekken
    
    bool CanProces(Enum type);
    GameObject GetGameObject();
    BuildingType GetBuildingType();
    TypeProcessing GetCurrentTypeProcessed();

    ProductionSetting GetCardDisplaySetting(Enum type) => GetBuildingType().GetProductionSettings().Single(x => (BarracksUnitType)x.GetType() == (BarracksUnitType)type);
    float GetProductionTime() => GetBuildingType().GetProductionDurationSettings().TimeToProduceResourceInSeconds;

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