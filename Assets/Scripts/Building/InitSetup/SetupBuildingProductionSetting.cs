using System;
using System.Collections.Generic;
using System.Linq;

public static class SetupBuildingProductionSetting
{
    public static List<ProductionSetting> GetProductionSettings(this BuildingType type)
    {
        var building = BuildingPrefabs.Get().Single(x => x.BuildingType == type);
        switch (type)
        {
            case BuildingType.BLACKSMITH:
            case BuildingType.WEAPONMAKER:
            case BuildingType.LEATHERARMORY:
            case BuildingType.CLOTHARMORMAKER:
                return type.GetItemProductionSettings().ConvertAll<ProductionSetting>(x => x);
            case BuildingType.BARRACKS:
                return BarrackUnitPrefabs.Get().ConvertAll<ProductionSetting>(x => x);
            case BuildingType.SCHOOL:
                return VillagerPrefabs.Get().ConvertAll<ProductionSetting>(x => x);

            default:
                throw new Exception($"BuildingType for type.ToString() not specified");
        }
    }

    public static List<ItemProductionSetting> GetItemProductionSettings(this BuildingType type)
    {
        return type.GetItemProduceSettings().ConvertAll(x => x.ConvertToProductionSettings());
    }

    private static ItemProductionSetting ConvertToProductionSettings(this ItemProduceSetting produceSetting)
    {
        if(produceSetting == null)
        {
            return null;
        }

        return new ItemProductionSetting
        {
            ItemsConsumedToProduce = produceSetting.ItemsConsumedToProduce,
            MaxBuffer = produceSetting.ItemsToProduce.Single().MaxBuffer,
            Type = produceSetting.ItemsToProduce.Single().ItemType
        };       
    }   
}