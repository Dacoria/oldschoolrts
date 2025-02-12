using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public static class BuildingPrefabs
{
    private static List<BuildingPrefabItem> cache;
    public static List<BuildingPrefabItem> Get() 
    {        
        if (cache == null)
        {
            cache = GenerateBuildingPrefabItems();
        }
        return cache;        
    }

    private static List<BuildingPrefabItem> GenerateBuildingPrefabItems()
    {
        var result = new List<BuildingPrefabItem>();

        foreach (BuildingType buildingType in Enum.GetValues(typeof(BuildingType)).Cast<BuildingType>().OrderBy(x => x.ToString()))
        {
            if (buildingType == BuildingType.NONE)
            {
                continue;
            }

            if (Load.GoMapBuildings.TryGetValue($"{buildingType.ToString()}Prefab", out GameObject buildingPrefab))
            {
                if (Load.SpriteMapBuildings.TryGetValue($"{buildingType.ToString()}Image", out Sprite buildingSprite))
                {
                    var prefabBuilding = new BuildingPrefabItem
                    {
                        BuildingPrefab = buildingPrefab,
                        BuildingType = buildingType,
                        DisplayOffset = BuildingTypeOffsets.GetValueOrDefault(buildingType, Vector3.zero),
                        Icon = buildingSprite
                    };
                    result.Add(prefabBuilding);

                    CheckProperPrefabSetup(buildingType, buildingPrefab);
                }
                else
                {
                    throw new Exception($"BuildingType {buildingType} heeft geen sprite image :O");
                }
            }
            else
            {
                throw new Exception($"BuildingType {buildingType} heeft geen prefab GO :O");
            }
        }

        return result;
    }

    private static void CheckProperPrefabSetup(BuildingType buildingType, GameObject buildingPrefab)
    {
        if (buildingPrefab.GetComponentInChildren<GhostBuildingBehaviour>() == null)
        {
            throw new Exception($"{buildingType.ToString()} Prefab --> Ghost GO staat uit! Enable plz");
        }
        if (buildingPrefab.GetComponentInChildren<MoveObjectToSurface>() != null)
        {
            throw new Exception($"{buildingType.ToString()} Prefab --> Real GO staat aan! Disable plz");
        }
        if (buildingPrefab.GetComponentInChildren<MoveObjectToSurface>(true) == null)
        {
            throw new Exception($"{buildingType.ToString()} Prefab --> Real GO bevat geen 'MoveObjectToSurface' --> Toevoegen!");
        }
        if(buildingPrefab.EntranceExit() == new Vector3(0, 0, 0))
        {
            throw new Exception($"{buildingType.ToString()} Prefab --> Bevat geen Entrance Exit Go (check GO + tag) --> Toevoegen!");
        }
    }

    private static Dictionary<BuildingType, Vector3> BuildingTypeOffsets => new Dictionary<BuildingType, Vector3>
    {
        {  BuildingType.BAKERY,         new Vector3(0.7f,     0,      0 ) },
        {  BuildingType.HOUSE,          new Vector3(0.7f,     0,      0 ) },
        {  BuildingType.LEATHERMAKER,   new Vector3(0.7f,     0,      0 ) },
        {  BuildingType.PIGFARM,        new Vector3(0.7f,     0,  -1.1f ) },
        {  BuildingType.SHEEPFARM,      new Vector3(0.7f,     0,  -1.1f ) },
        {  BuildingType.QUARRY,         new Vector3(0.0f,     0, -0.85f ) },
        {  BuildingType.SCHOOL,         new Vector3(-0.8f,    0,  -0.8f ) },
        {  BuildingType.TAVERN,         new Vector3(1,     -0.3f, -1.1f ) },
        {  BuildingType.WHEATFARM,      new Vector3(2.2f,     0, -0.85f ) },
        {  BuildingType.SAWMILL,        new Vector3(0.5f,     0,    -1f ) },
        {  BuildingType.BARRACKS,       new Vector3(0.8f,     0,  -0.4f ) },
        {  BuildingType.WEAPONMAKER,    new Vector3(0.4f,     0,   0.9f ) },
        {  BuildingType.FORESTHUT,      new Vector3(0.5f,     0,      0 ) },
        {  BuildingType.LEATHERARMORY,  new Vector3(-1f,      0,      0 ) },
        {  BuildingType.THREADMAKER,    new Vector3(0,        0,  -1.1f ) },
        {  BuildingType.BUTCHER,        new Vector3(0,        0,  -1.6f ) },
        {  BuildingType.CLOTHMAKER,     new Vector3(0,        0,    -2f ) },
        {  BuildingType.CHURCH,         new Vector3(0,        0,    -3f ) },
    };
}