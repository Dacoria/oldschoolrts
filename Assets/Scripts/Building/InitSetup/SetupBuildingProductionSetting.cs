using System;
using System.Collections.Generic;
using System.Linq;

public static class SetupBuildingProductionSetting
{
    public static List<ProductionSetting> GetProductionSettings(this BuildingType type)
    {
        var category = type.GetCategory();
        switch (category)
        {
            case BuildingCategory.SelectProductsOverTime:
                return type.GetItemProductionSettings().ConvertAll<ProductionSetting>(x => x);
            case BuildingCategory.School:
                return VillagerPrefabs.Get().ConvertAll<ProductionSetting>(x => x);
            case BuildingCategory.Barracks:
                return BarrackUnitPrefabs.Get().ConvertAll<ProductionSetting>(x => x);
            default:
                throw new Exception();
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
            MaxBuffer = produceSetting.ItemsToProduce.Single().MaxBuffer, // TODO; kan meerdere items hebben (bv pig)
            Type = produceSetting.ItemsToProduce.Single().ItemType // TODO; kan meerdere items hebben (bv pig)
        };       
    }
}