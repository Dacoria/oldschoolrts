using System.Collections.Generic;

public static class SetupBuildingBuildCostsSetting
{
    private static Dictionary<BuildingType, List<ItemAmountBuffer>> cache = new Dictionary<BuildingType, List<ItemAmountBuffer>>();

    public static List<ItemAmountBuffer> GetBuildCosts(this BuildingType type)
    {
        if (!cache.ContainsKey(type))
        {
            cache.Add(type, GetBuildCostsNoCache(type));
        }
        return cache[type];
    }

    private static List<ItemAmountBuffer> GetBuildCostsNoCache(BuildingType type)
    {
        return type.GetBuildingCostsSetup().ConvertToBuildCosts();
    }

    private static BuildingCostsSetup GetBuildingCostsSetup(this BuildingType buildingType)
    {
        //var defaultCosts = new BuildingCostsSetup(@in: ItemType.PLANKS, inAmount: 3, in2: ItemType.STONE, inAmount2: 2);
        var defaultCosts = new BuildingCostsSetup(@in: ItemType.STONE, inAmount: 1); // ROAD = NONE
        return BuildCostsSettings.GetValueOrDefault(buildingType, defaultCosts);
    }

    private static List<ItemAmountBuffer> ConvertToBuildCosts(this BuildingCostsSetup setup)
    {
        var result = new List<ItemAmountBuffer>();
        if (setup == null)
        {
            return result;
        }

        if (setup.In != ItemType.NONE)
        {
            result.Add(new ItemAmountBuffer { ItemType = setup.In, Amount = setup.InAmount, MaxBuffer = setup.InMaxBuffer });
        }
        if (setup.In2 != ItemType.NONE)
        {
            result.Add(new ItemAmountBuffer { ItemType = setup.In2, Amount = setup.InAmount2, MaxBuffer = setup.InMaxBuffer2 });
        }
        if (setup.In3 != ItemType.NONE)
        {
            result.Add(new ItemAmountBuffer { ItemType = setup.In3, Amount = setup.InAmount3, MaxBuffer = setup.InMaxBuffer3 });
        }        

        return result;
    }

    private static Dictionary<BuildingType, BuildingCostsSetup> BuildCostsSettings = new Dictionary<BuildingType, BuildingCostsSetup>
    {
        { BuildingType.BAKERY, new BuildingCostsSetup(         @in:ItemType.PLANKS, inAmount: 4, in2: ItemType.STONE, inAmount2: 3, in3: ItemType.NONE,    inAmount3: 0) },
        { BuildingType.BARRACKS, new BuildingCostsSetup(       @in:ItemType.PLANKS, inAmount: 6, in2: ItemType.STONE, inAmount2: 4, in3: ItemType.IRONBAR, inAmount3: 2) },
        { BuildingType.BERRYGATHERSPOT, new BuildingCostsSetup(@in:ItemType.PLANKS, inAmount: 2, in2: ItemType.NONE,  inAmount2: 0, in3: ItemType.NONE,    inAmount3: 0) },
        { BuildingType.BLACKSMITH, new BuildingCostsSetup(     @in:ItemType.PLANKS, inAmount: 4, in2: ItemType.STONE, inAmount2: 3, in3: ItemType.IRONBAR, inAmount3: 1) },
        { BuildingType.BUTCHER, new BuildingCostsSetup(        @in:ItemType.PLANKS, inAmount: 4, in2: ItemType.STONE, inAmount2: 3, in3: ItemType.NONE,    inAmount3: 0) },
        { BuildingType.CHURCH, new BuildingCostsSetup(         @in:ItemType.PLANKS, inAmount: 10,in2: ItemType.STONE, inAmount2: 6, in3: ItemType.GOLDBAR, inAmount3: 4) },
        { BuildingType.CLOTHARMORMAKER, new BuildingCostsSetup(@in:ItemType.PLANKS, inAmount: 4, in2: ItemType.STONE, inAmount2: 3, in3: ItemType.NONE,    inAmount3: 0) },
        { BuildingType.CLOTHMAKER, new BuildingCostsSetup(     @in:ItemType.PLANKS, inAmount: 3, in2: ItemType.STONE, inAmount2: 2, in3: ItemType.NONE,    inAmount3: 0) },
        { BuildingType.COALMINE, new BuildingCostsSetup(       @in:ItemType.PLANKS, inAmount: 2, in2: ItemType.STONE, inAmount2: 2, in3: ItemType.NONE,    inAmount3: 0) },
        { BuildingType.FISHINGLODGE, new BuildingCostsSetup(   @in:ItemType.PLANKS, inAmount: 4, in2: ItemType.STONE, inAmount2: 2, in3: ItemType.NONE,    inAmount3: 0) },
        { BuildingType.FORESTHUT, new BuildingCostsSetup(      @in:ItemType.PLANKS, inAmount: 3, in2: ItemType.STONE, inAmount2: 1, in3: ItemType.NONE,    inAmount3: 0) },
        { BuildingType.GOLDMINE, new BuildingCostsSetup(       @in:ItemType.PLANKS, inAmount: 2, in2: ItemType.STONE, inAmount2: 2, in3: ItemType.NONE,    inAmount3: 0) },
        { BuildingType.GOLDSMELTER, new BuildingCostsSetup(    @in:ItemType.PLANKS, inAmount: 4, in2: ItemType.STONE, inAmount2: 3, in3: ItemType.NONE,    inAmount3: 0) },
        { BuildingType.HOUSE, new BuildingCostsSetup(          @in:ItemType.PLANKS, inAmount: 3, in2: ItemType.STONE, inAmount2: 2, in3: ItemType.NONE,    inAmount3: 0) },
        { BuildingType.HUNTERSHUT, new BuildingCostsSetup(     @in:ItemType.PLANKS, inAmount: 3, in2: ItemType.STONE, inAmount2: 1, in3: ItemType.NONE,    inAmount3: 0) },
        { BuildingType.IRONMINE, new BuildingCostsSetup(       @in:ItemType.PLANKS, inAmount: 2, in2: ItemType.STONE, inAmount2: 2, in3: ItemType.NONE,    inAmount3: 0) },
        { BuildingType.LEATHERARMORY, new BuildingCostsSetup(  @in:ItemType.PLANKS, inAmount: 4, in2: ItemType.STONE, inAmount2: 3, in3: ItemType.LEATHER, inAmount3: 1) },
        { BuildingType.LEATHERMAKER, new BuildingCostsSetup(   @in:ItemType.PLANKS, inAmount: 4, in2: ItemType.STONE, inAmount2: 3, in3: ItemType.NONE,    inAmount3: 0) },
        { BuildingType.MILL, new BuildingCostsSetup(           @in:ItemType.PLANKS, inAmount: 4, in2: ItemType.STONE, inAmount2: 3, in3: ItemType.NONE,    inAmount3: 0) },
        { BuildingType.PIGFARM, new BuildingCostsSetup(        @in:ItemType.PLANKS, inAmount: 4, in2: ItemType.STONE, inAmount2: 3, in3: ItemType.NONE,    inAmount3: 0) },
        { BuildingType.QUARRY, new BuildingCostsSetup(         @in:ItemType.PLANKS, inAmount: 2, in2: ItemType.STONE, inAmount2: 2, in3: ItemType.NONE,    inAmount3: 0) },
        { BuildingType.SAWMILL, new BuildingCostsSetup(        @in:ItemType.PLANKS, inAmount: 4, in2: ItemType.STONE, inAmount2: 3, in3: ItemType.NONE,    inAmount3: 0) },
        { BuildingType.SCHOOL, new BuildingCostsSetup(         @in:ItemType.PLANKS, inAmount: 5, in2: ItemType.STONE, inAmount2: 5, in3: ItemType.NONE,    inAmount3: 0) },
        { BuildingType.SHEEPFARM, new BuildingCostsSetup(      @in:ItemType.PLANKS, inAmount: 4, in2: ItemType.STONE, inAmount2: 3, in3: ItemType.NONE,    inAmount3: 0) },
        { BuildingType.TAVERN, new BuildingCostsSetup(         @in:ItemType.PLANKS, inAmount: 6, in2: ItemType.STONE, inAmount2: 5, in3: ItemType.NONE,    inAmount3: 0) },
        { BuildingType.THREADMAKER, new BuildingCostsSetup(    @in:ItemType.PLANKS, inAmount: 4, in2: ItemType.STONE, inAmount2: 3, in3: ItemType.NONE,    inAmount3: 0) },
        { BuildingType.WALL, new BuildingCostsSetup(           @in:ItemType.NONE  , inAmount: 0, in2: ItemType.STONE, inAmount2: 1, in3: ItemType.NONE,    inAmount3: 0) },
        { BuildingType.WAREHOUSE, new BuildingCostsSetup(      @in:ItemType.PLANKS, inAmount: 6, in2: ItemType.STONE, inAmount2: 6, in3: ItemType.NONE,    inAmount3: 0) },
        { BuildingType.WATERWELL, new BuildingCostsSetup(      @in:ItemType.PLANKS, inAmount: 1, in2: ItemType.STONE, inAmount2: 2, in3: ItemType.NONE,    inAmount3: 0) },
        { BuildingType.WEAPONMAKER, new BuildingCostsSetup(    @in:ItemType.PLANKS, inAmount: 5, in2: ItemType.STONE, inAmount2: 3, in3: ItemType.NONE,    inAmount3: 0) },
        { BuildingType.WHEATFARM, new BuildingCostsSetup(      @in:ItemType.PLANKS, inAmount: 4, in2: ItemType.STONE, inAmount2: 3, in3: ItemType.NONE,    inAmount3: 0) },
        { BuildingType.WIZARDHOUSE, new BuildingCostsSetup(    @in:ItemType.PLANKS, inAmount: 8, in2: ItemType.STONE, inAmount2: 6, in3: ItemType.GOLDBAR, inAmount3: 2) },

    };
}