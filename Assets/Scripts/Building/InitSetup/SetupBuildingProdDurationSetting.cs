using System;
using System.Collections.Generic;
using System.Linq;

public static class SetupBuildingProdDurationSetting
{
    private static Dictionary<BuildingType, ProduceResourceDurations> cacheItemProduce = new Dictionary<BuildingType, ProduceResourceDurations>();

    public static ProduceResourceDurations GetProductionDurationSettings(this BuildingType type)
    {
        if (!cacheItemProduce.ContainsKey(type))
        {
            cacheItemProduce.Add(type, GetProductionDurationSettingsNoCache(type));
        }
        return cacheItemProduce[type];
    }

    private static ProduceResourceDurations GetProductionDurationSettingsNoCache(BuildingType type)
    {
        var buildingPrefab = BuildingPrefabs.Get().Single(x => x.BuildingType == type).BuildingPrefab;

        var mine = buildingPrefab.GetComponentInChildren<ProduceResourceMiningBehaviour>(true);
        if (mine != null)
        {
            return new ProduceResourceDurations
            {
                TimeToProduceResourceInSeconds = 20,
                TimeToWaitAfterProducingInSeconds = 3
            };
        }

        var manual = buildingPrefab.GetComponentInChildren<ProduceResourceManualBehaviour>(true);
        if (manual != null)
        {
            return new ProduceResourceDurations
            {
                TimeToProduceResourceInSeconds = 0, // manual - gaat via poppetje
                TimeToWaitAfterProducingInSeconds = 2
            };
        }

        var overtime = buildingPrefab.GetComponentInChildren<ProduceResourceOverTimeBehaviour>(true);
        if (overtime != null)
        {
            if(type == BuildingType.SHEEPFARM || type == BuildingType.PIGFARM)
            {
                return new ProduceResourceDurations
                {
                    TimeToProduceResourceInSeconds = 60,
                    TimeToWaitAfterProducingInSeconds = 5
                };
            }

            return new ProduceResourceDurations
            {
                TimeToProduceResourceInSeconds = 30,
                TimeToWaitAfterProducingInSeconds = 5
            };
        }

        var card = buildingPrefab.GetComponentInChildren<CardItemsProduceBehaviour>(true);
        if (card != null)
        {
            return new ProduceResourceDurations
            {
                TimeToProduceResourceInSeconds = 40,
                TimeToWaitAfterProducingInSeconds = 5
            };
        }

        return new ProduceResourceDurations
        {
            TimeToProduceResourceInSeconds = 15,
            TimeToWaitAfterProducingInSeconds = 2
        };
    }
}