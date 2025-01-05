public static class Constants
{
    //layers (collision + nav mesh)
    public static int LAYER_DEFAULT = 0;
    public static int LAYER_TERRAIN = 3;
    public static int LAYER_UI = 5;
    public static int LAYER_RTS_UNIT = 6;
    public static int LAYER_WALL_LAYER = 7;
    public static int LAYER_ROAD = 8;
    public static int LAYER_FARM_FIELD = 9;

    //tags
    public static string TAG_ROAD = "Road";
    public static string TAG_ENTRANCE_EXIT = "EntranceExit";
    public static string TAG_BUILDING = "Building";
    public static string TAG_WHEATFIELD = "WheatField";
    public static string TAG_TAVERN = "Tavern";
    public static string TAG_RESOURCE = "Resource";
    public static string TAG_UNIT = "Unit";
    public static string TAG_COLLIDE = "Collide";

    //animation trigger
    public static string ANIM_BOOL_IS_ATTACKING = "IS_ATTACKING";
    public static string ANIM_BOOL_IS_WALKING = "IS_WALKING";
    public static string ANIM_BOOL_IS_WORKING_2 = "IS_WORKING_2";
    public static string ANIM_BOOL_IS_WORKING = "IS_WORKING";
    public static string ANIM_BOOL_IS_IDLE = "IS_IDLE";
    public static string ANIM_TRIGGER_DIE = "Die";

    // Load paths
    public static string LOAD_PATH_GO_UI_PREFAB = "FinalPrefab/UiPrefabs";
    public static string LOAD_PATH_GO_BUILDINGS = "FinalPrefab/Buildings";
    public static string LOAD_PATH_GO_UNITS = "FinalPrefab/Units";
    public static string LOAD_PATH_GO_RSC_TO_CARRY = "ResourcesToCarry";

    public static string LOAD_PATH_SPRITE_BUILDINGS = "Images/BuildingImages";
    public static string LOAD_PATH_SPRITE_VILLAGERS = "Images/VillagerImages";
    public static string LOAD_PATH_SPRITE_MILITARY = "Images/MilitaryImages";
    public static string LOAD_PATH_SPRITE_RESOURCES = "Images/ResourceImages";

    public static string PATH_ROAD_NAV_MESH = "FinalPrefab/Road/RoadNavMeshSurfacePrefab";

    // Area Mask
    public static string AREA_MASK_EVERYTHING = "Everything";
    public static string AREA_MASK_WALKABLE = "Walkable";
    public static string AREA_MASK_ROAD = "Road";

    // GO Objects to find in Scene
    public static string GO_SCENE_MAIN = "Grass";
    public static string GO_SCENE_VILLAGERS = "Villagers";
    public static string GO_SCENE_BATTLE_UNITS = "BattleUnits";
    public static string GO_SCENE_SQUADS = "Squads";

    // Objects to find in Prefabs
    //UI
    public static string GO_PREFAB_UI_GO_TO_TAVERN_DISPLAY = "GoToTavernBubble";
    public static string GO_PREFAB_UI_PRODUCE_ITEM_RIGHT_ARROW = "ProduceItemRightArrow";
    public static string GO_PREFAB_UI_RANGE_DISPLAY = "RangeDisplay";
    public static string GO_PREFAB_UI_SERF_PROCESSING_DISPLAY = "SerfProcessingDisplayGo";
    public static string GO_PREFAB_UI_UNIT_DESTINATION_DISPLAY = "UnitDestinationSphere";
    public static string GO_PREFAB_UI_UNIT_SELECTION_DISPLAY = "UnitSelectionSphere";

    // RscToCarry
    public static string GO_PREFAB_RSC_TO_CARRY_CUBE_UNKNOWN = "CubeUnknownBeingCarried";
    
    //Buildings
    public static string GO_PREFAB_BUILDINGS_ROAD = "Road";
}