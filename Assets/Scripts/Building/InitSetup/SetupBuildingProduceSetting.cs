using System;
using System.Collections.Generic;
using System.Linq;

public static class SetupBuildingProduceSetting
{
    private static Dictionary<BuildingType, List<ItemProduceSetting>> cache = new Dictionary<BuildingType, List<ItemProduceSetting>>();

    public static List<ItemProduceSetting> GetItemProduceSettings(this BuildingType type)
    {
        if (!cache.ContainsKey(type))
        {
            cache.Add(type, GetItemProduceSettingsNoCache(type));
        }
        return cache[type];
    }

    public static bool IgnoreMaxItemBuffer(this BuildingType type)
    {
        if(type == BuildingType.BARRACKS)
        {
            return true;
        }
        return false;
    }

    public static List<ItemAmountBuffer> GetItemsConsumedToProduceAll(this BuildingType type)
    {
        var buildingProducingType = type.GetBuildingProducingType();

        switch (buildingProducingType)
        {
            case BuildingProducingType.Nothing:
                return new List<ItemAmountBuffer>();
            case BuildingProducingType.Items:
                return type.GetItemProduceSettings().SelectMany(x => x.ItemsConsumedToProduce).ToList();
            case BuildingProducingType.Villagers:
            case BuildingProducingType.BattleUnits:
                return type.GetProductionSettings().SelectMany(x => x.ItemsConsumedToProduce).ToList();
            default:
                throw new Exception();
        }
    }

    private static List<ItemProduceSetting> GetItemProduceSettingsNoCache(BuildingType type)
    {
        var category = type.GetCategory();

        switch (category)
        {
            case BuildingCategory.Manual:
                return GetSetupManualProduceSetting(type);
            case BuildingCategory.Mine:
                return GetSetupMineProduceSetting(type);
            case BuildingCategory.OneProductOverTime:
                return new List<ItemProduceSetting> { type.GetBuildingOverTimeSetup().ConvertToProduceSettings() };
            case BuildingCategory.SelectProductsOverTime:
                return ProdCardItemSettings.GetValueOrDefault(type, new List<BuildingInOutSetup>()).Select(x => x.ConvertToProduceSettings()).ToList();
            //case BuildingCategory.School:
            //case BuildingCategory.Unknown:
            //case BuildingCategory.Barracks:
            //case BuildingCategory.Population:
            //    return new List<ItemProduceSetting>(); // geen item productie
            default:
                throw new Exception();
        }
    }

    private static List<ItemProduceSetting> GetSetupManualProduceSetting(BuildingType type)
    {
        var itemToProduce = new ItemOutput
        {
            MaxBuffer = 6,
            ProducedPerProdCycle = 1,
            ItemType = type.GetMaterialResourceType().GetItemType()
        };
        return new List<ItemProduceSetting> { new ItemProduceSetting { ItemsToProduce = new List<ItemOutput> { itemToProduce }, ItemsConsumedToProduce = new List<ItemAmountBuffer>() } };
    }

    private static List<ItemProduceSetting> GetSetupMineProduceSetting(BuildingType type)
    {
        var itemToProduce = new ItemOutput
        {
            MaxBuffer = 6,
            ProducedPerProdCycle = 1,
            ItemType = type.GetMaterialResourceType().GetItemType()
        };
        return new List<ItemProduceSetting> { new ItemProduceSetting { ItemsToProduce = new List<ItemOutput> { itemToProduce }, ItemsConsumedToProduce = new List<ItemAmountBuffer>() } };
    }

    public static MaterialResourceType GetMaterialResourceType(this BuildingType type)
    {
        return MaterialRscForBuilding.GetValueOrDefault(type, MaterialResourceType.NONE);
    }

    private static Dictionary<BuildingType, MaterialResourceType> MaterialRscForBuilding = new Dictionary<BuildingType, MaterialResourceType>
    {
        { BuildingType.QUARRY,          MaterialResourceType.STONE },
        { BuildingType.FORESTHUT,       MaterialResourceType.WOODLOG },
        { BuildingType.COALMINE,        MaterialResourceType.COALORE },
        { BuildingType.GOLDMINE,        MaterialResourceType.GOLDORE },
        { BuildingType.IRONMINE,        MaterialResourceType.IRONORE },
        { BuildingType.BERRYGATHERSPOT, MaterialResourceType.REDBERRIES },
        { BuildingType.HUNTERSHUT,      MaterialResourceType.WILDANIMAL },
        { BuildingType.FISHINGLODGE,    MaterialResourceType.FISH },
        { BuildingType.WHEATFARM,       MaterialResourceType.WHEAT },
    };

    private static BuildingInOutSetup GetBuildingOverTimeSetup(this BuildingType buildingType)
    {
        return ProdOverTimeItemSettings.GetValueOrDefault(buildingType, null);
    }

    private static ItemProduceSetting ConvertToProduceSettings(this BuildingInOutSetup setup)
    {
        var result = new ItemProduceSetting { 
            ItemsConsumedToProduce = new List<ItemAmountBuffer>(), 
            ItemsToProduce = new List<ItemOutput>()
        };
        if (setup == null)
        {
            return result;
        }

        if(setup.In != ItemType.NONE)
        {
            result.ItemsConsumedToProduce.Add(new ItemAmountBuffer { ItemType = setup.In, Amount = setup.InAmount, MaxBuffer = setup.InMaxBuffer });
        }
        if (setup.In2 != ItemType.NONE)
        {
            result.ItemsConsumedToProduce.Add(new ItemAmountBuffer { ItemType = setup.In2, Amount = setup.InAmount2, MaxBuffer = setup.InMaxBuffer2 });
        }
        if (setup.In3 != ItemType.NONE)
        {
            result.ItemsConsumedToProduce.Add(new ItemAmountBuffer { ItemType = setup.In3, Amount = setup.InAmount3, MaxBuffer = setup.InMaxBuffer3 });
        }
        if (setup.Out != ItemType.NONE)
        {
            result.ItemsToProduce.Add(new ItemOutput { ItemType = setup.Out, ProducedPerProdCycle = setup.OutAmount, MaxBuffer = setup.OutMaxBuffer });
        }
        if (setup.Out2 != ItemType.NONE)
        {
            result.ItemsToProduce.Add(new ItemOutput { ItemType = setup.Out2, ProducedPerProdCycle = setup.OutAmount2, MaxBuffer = setup.OutMaxBuffer2 });
        }

        return result;
    }  

    private static Dictionary<BuildingType, BuildingInOutSetup> ProdOverTimeItemSettings = new Dictionary<BuildingType, BuildingInOutSetup>
    {
        { BuildingType.BAKERY, new BuildingInOutSetup(      @in: ItemType.FLOUR,                        @out: ItemType.BREAD,      outAmount: 2) },
        { BuildingType.BUTCHER, new BuildingInOutSetup(     @in:ItemType.PIGMEAT,                       @out: ItemType.SAUSAGE,    outAmount: 3) },
        { BuildingType.CLOTHMAKER, new BuildingInOutSetup(  @in:ItemType.THREAD,                        @out: ItemType.CLOTH) },
        { BuildingType.GOLDSMELTER, new BuildingInOutSetup( @in:ItemType.GOLDORE,                       @out: ItemType.GOLDBAR,    outAmount: 2) },
        { BuildingType.IRONSMELTER, new BuildingInOutSetup( @in:ItemType.IRONORE,                       @out: ItemType.IRONBAR) },
        { BuildingType.LEATHERMAKER, new BuildingInOutSetup(@in:ItemType.PIGHIDE,                       @out: ItemType.LEATHER,    outAmount: 2) },
        { BuildingType.MILL, new BuildingInOutSetup(        @in:ItemType.WHEAT,                         @out: ItemType.FLOUR) },
        { BuildingType.PIGFARM, new BuildingInOutSetup(     @in:ItemType.WHEAT,   inAmount: 4,          @out: ItemType.PIGHIDE,    out2: ItemType.PIGMEAT) },
        { BuildingType.SAWMILL, new BuildingInOutSetup(     @in:ItemType.LUMBER,                        @out: ItemType.PLANKS,     outAmount: 2) },
        { BuildingType.SHEEPFARM, new BuildingInOutSetup(   @in:ItemType.WHEAT, inAmount: 2, in2: ItemType.WATER, inAmount2: 2,  @out: ItemType.WOOL) },
        { BuildingType.THREADMAKER, new BuildingInOutSetup( @in:ItemType.WOOL,                          @out: ItemType.THREAD,     outAmount : 2) },
        { BuildingType.WATERWELL, new BuildingInOutSetup(                                               @out: ItemType.WATER) },
    };

    private static Dictionary<BuildingType, List<BuildingInOutSetup>> ProdCardItemSettings = new Dictionary<BuildingType, List<BuildingInOutSetup>>
    {
        { BuildingType.BLACKSMITH, new List<BuildingInOutSetup>
        {
            new BuildingInOutSetup(@in:ItemType.IRONBAR,                in2: ItemType.COALORE, @out: ItemType.IRONCROSSBOW),
            new BuildingInOutSetup(@in:ItemType.IRONBAR,                in2: ItemType.COALORE, @out: ItemType.IRONHELM),
            new BuildingInOutSetup(@in:ItemType.IRONBAR, inAmount: 2,   in2: ItemType.COALORE, @out: ItemType.IRONARMOR),
            new BuildingInOutSetup(@in:ItemType.IRONBAR,                in2: ItemType.COALORE, @out: ItemType.IRONSWORD),
            new BuildingInOutSetup(@in:ItemType.IRONBAR,                                       @out: ItemType.IRONSHIELD),
        }},
        { BuildingType.CLOTHARMORMAKER, new List<BuildingInOutSetup>
        {
            new BuildingInOutSetup(@in:ItemType.CLOTH,   inAmount: 2,   in2: ItemType.THREAD,  @out: ItemType.CLOTHARMOR),
            new BuildingInOutSetup(@in:ItemType.CLOTH,                  in2: ItemType.THREAD,  @out: ItemType.CLOTHPANTS),
            new BuildingInOutSetup(@in:ItemType.CLOTH,                  in2: ItemType.THREAD,  @out: ItemType.CLOTHBOOTS),
            new BuildingInOutSetup(@in:ItemType.CLOTH,                  in2: ItemType.THREAD,  @out: ItemType.CLOTHHELMET),
        }},
        { BuildingType.LEATHERARMORY, new List<BuildingInOutSetup>
        {
            new BuildingInOutSetup(@in:ItemType.LEATHER, inAmount: 2,   in2: ItemType.THREAD,  @out: ItemType.LEATHERARMOR),
            new BuildingInOutSetup(@in:ItemType.LEATHER,                in2: ItemType.THREAD,  @out: ItemType.LEATHERHELMET),
            new BuildingInOutSetup(@in:ItemType.LEATHER,                in2: ItemType.THREAD,  @out: ItemType.LEATHERPANTS),
            new BuildingInOutSetup(@in:ItemType.LEATHER,                in2: ItemType.THREAD,  @out: ItemType.LEATHERBOOTS),
        }},
        { BuildingType.WEAPONMAKER, new List<BuildingInOutSetup>
        {
            new BuildingInOutSetup(@in:ItemType.PLANKS, inAmount: 2,                           @out: ItemType.WOODENBOW),
            new BuildingInOutSetup(@in:ItemType.PLANKS, inAmount: 2,                           @out: ItemType.WOODENAXE),
            new BuildingInOutSetup(@in:ItemType.PLANKS, inAmount: 2,                           @out: ItemType.WOODENLANCE),
            new BuildingInOutSetup(@in:ItemType.PLANKS, inAmount: 2,                           @out: ItemType.WOODENMAGESTAFF),
            new BuildingInOutSetup(@in:ItemType.PLANKS, inAmount: 1,                           @out: ItemType.WOODENSHIELD),
        }},
    }; 
}