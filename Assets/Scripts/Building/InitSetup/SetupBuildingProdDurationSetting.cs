using System;
using System.Collections.Generic;
using System.Linq;

public static class SetupBuildingProdDurationSetting
{
    private static Dictionary<BuildingType, ProduceDurations> cache = new Dictionary<BuildingType, ProduceDurations>();

    public static ProduceDurations GetProductionDurationSettings(this BuildingType type)
    {
        if (!cache.ContainsKey(type))
        {
            cache.Add(type, GetProductionDurationSettingsNoCache(type));
        }
        return cache[type];
    }

    private static ProduceDurations GetProductionDurationSettingsNoCache(BuildingType type)
    {
        var category = type.GetCategory();

        switch (category)
        {
            case BuildingCategory.Manual:
                return new ProduceDurations
                {
                    TimeToProduceResourceInSeconds = 0, // manual - gaat via poppetje
                    TimeToWaitAfterProducingInSeconds = 2
                };
            case BuildingCategory.Mine:
                return new ProduceDurations
                {
                    TimeToProduceResourceInSeconds = 20,
                    TimeToWaitAfterProducingInSeconds = 3
                };
            case BuildingCategory.OneProductOverTime:
                if (type == BuildingType.SHEEPFARM || type == BuildingType.PIGFARM)
                {
                    return new ProduceDurations
                    {
                        TimeToProduceResourceInSeconds = 60,
                        TimeToWaitAfterProducingInSeconds = 5
                    };
                }

                return new ProduceDurations
                {
                    TimeToProduceResourceInSeconds = 30,
                    TimeToWaitAfterProducingInSeconds = 5
                };
            case BuildingCategory.SelectProductsOverTime:
                return new ProduceDurations
                {
                    TimeToProduceResourceInSeconds = 40,
                    TimeToWaitAfterProducingInSeconds = 5
                };
            case BuildingCategory.School:
            case BuildingCategory.Unknown:
            case BuildingCategory.Barracks:
            case BuildingCategory.Population:
                return new ProduceDurations
                {
                    TimeToProduceResourceInSeconds = 0,
                    TimeToWaitAfterProducingInSeconds = 0
                };
            default:
                throw new Exception();
        }
    }
}