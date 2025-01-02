using System.Collections.Generic;
using System.Linq;

public enum BuildingCategory
{
    Unknown,
    Manual,
    Magic,
    Mine,
    OneProductOverTime,
    SelectProductsOverTime,
    School,
    Barracks,
    Population,
    Stockpile,
    Tavern
}

public static class BuildingCategoryHelper
{
    private static Dictionary<BuildingType, BuildingCategory> cache = new Dictionary<BuildingType, BuildingCategory>();

    public static BuildingCategory GetCategory(this BuildingType type)
    {
        if (!cache.ContainsKey(type))
        {
            cache.Add(type, GetCategoryNoCache(type));
        }
        return cache[type];
    }

    private static BuildingCategory GetCategoryNoCache(BuildingType type)
    {
        if (type == BuildingType.NONE)
            return BuildingCategory.Unknown;

        if (type == BuildingType.WIZARDHOUSE)
            return BuildingCategory.Magic;

        var buildingPrefab = BuildingPrefabs.Get().FirstOrDefault(x => x.BuildingType == type).BuildingPrefab;

        if (buildingPrefab.GetComponentInChildren<ProduceResourceMiningBehaviour>(true) != null)
            return BuildingCategory.Mine;

        if (buildingPrefab.GetComponentInChildren<ProduceResourceManualBehaviour>(true) != null)
            return BuildingCategory.Manual;

        if (buildingPrefab.GetComponentInChildren<ProduceResourceOverTimeBehaviour>(true) != null)
            return BuildingCategory.OneProductOverTime;

        if (buildingPrefab.GetComponentInChildren<ProduceResourceCardBehaviour>(true) != null)
            return BuildingCategory.SelectProductsOverTime;

        if (buildingPrefab.GetComponentInChildren<BarracksBehaviour>(true) != null)
            return BuildingCategory.Barracks;

        if (buildingPrefab.GetComponentInChildren<SchoolBehaviour>(true) != null)
            return BuildingCategory.School;

        if (buildingPrefab.GetComponentInChildren<IncreaseMaxPopulationBehaviour>(true) != null)
            return BuildingCategory.Population;        

        if (buildingPrefab.GetComponentInChildren<StockpileBehaviour>(true) != null)
            return BuildingCategory.Stockpile;

        if (buildingPrefab.GetComponentInChildren<TavernBehaviour>(true) != null)
            return BuildingCategory.Tavern;


        throw new System.Exception();
    }
}