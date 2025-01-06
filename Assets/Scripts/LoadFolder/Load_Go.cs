using System.Collections.Generic;
using UnityEngine;

public static partial class Load
{
    private static Dictionary<string, GameObject> _goMapUnits;
    public static Dictionary<string, GameObject> GoMapUnits => LoadHelper.GetOrCreateCache(_goMapUnits, Constants.LOAD_PATH_GO_UNITS);

    private static Dictionary<string, GameObject> _goMapUI;
    public static Dictionary<string, GameObject> GoMapUI => LoadHelper.GetOrCreateCache(_goMapUI, Constants.LOAD_PATH_GO_UI_PREFAB);

    private static Dictionary<string, GameObject> _goMapRscToCarry;
    public static Dictionary<string, GameObject> GoMapRscToCarry => LoadHelper.GetOrCreateCache(_goMapRscToCarry, Constants.LOAD_PATH_GO_RSC_TO_CARRY);

    private static Dictionary<string, GameObject> _goMapBuildings;
    public static Dictionary<string, GameObject> GoMapBuildings => LoadHelper.GetOrCreateCache(_goMapBuildings, Constants.LOAD_PATH_GO_BUILDINGS);
}