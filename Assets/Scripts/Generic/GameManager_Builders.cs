using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class GameManager : BaseAEMonoCI
{
    [HideInInspector] public List<BuildingPrefabItem> BuildingPrefabItems = new List<BuildingPrefabItem>();

    private static List<BuilderBehaviour> freeBuilders = new List<BuilderBehaviour>();
    private static SortedSet<BuilderRequest> BuilderRequests = new SortedSet<BuilderRequest>();

    public SortedSet<BuilderRequest> GetBuilderRequests() => BuilderRequests;

    private void InitCheckBuildingTypes()
    {
        BuilderRequests = new SortedSet<BuilderRequest>();

        foreach (BuildingType buildingType in Enum.GetValues(typeof(BuildingType)).Cast<BuildingType>().OrderBy(x => x.ToString()))
        {
            if(buildingType == BuildingType.NONE)
            {
                continue;
            }

            if (Load.GoMap.TryGetValue($"{buildingType.ToString()}Prefab" , out GameObject buildingPrefab))
            {
                if (Load.SpriteMap.TryGetValue($"{buildingType.ToString()}Image", out Sprite buildingSprite))
                {
                    var prefabBuilding = new BuildingPrefabItem
                    {
                        BuildingPrefab = buildingPrefab,
                        BuildingType = buildingType,
                        DisplayOffset = BuildingTypeOffsets.GetValueOrDefault(buildingType, Vector3.zero),
                        Icon = buildingSprite
                    };
                    BuildingPrefabItems.Add(prefabBuilding);
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
    }

    protected override void OnBuilderRequest(BuilderRequest builderRequest)
    {
        if (freeBuilders.Count > 0)
        {
            var builder = PopClosest(freeBuilders, builderRequest.Location);
            builder.AssignBuilderRequest(builderRequest);
        }
        else
        {
            BuilderRequests.Add(builderRequest);
        }
    }

    protected override void OnFreeBuilder(BuilderBehaviour builder)
    {
        var request = BuilderRequests.Pop();
        if (request != null)
        {
            builder.AssignBuilderRequest(request);
        }
        else
        {
            if(!IsFreeBuilder(builder))
            {
                freeBuilders.Add(builder);
            }
        }
    }

    public bool IsFreeBuilder(BuilderBehaviour builder) => freeBuilders.Any(x => x == builder);

    public bool TryRemoveBuilderFromFreeBuilderList(BuilderBehaviour builder)
    {
        if (IsFreeBuilder(builder))
        {
            return freeBuilders.Remove(builder);
        }

        return false;
    }

    private static Dictionary<BuildingType, Vector3> BuildingTypeOffsets => new Dictionary<BuildingType, Vector3>
    {
        {  BuildingType.BAKERY,         new Vector3(0.7f, 0, 0) },
        {  BuildingType.HOUSE,          new Vector3(0.7f, 0, 0) },
        {  BuildingType.LEATHERMAKER,   new Vector3(0.7f, 0, 0) },
        {  BuildingType.PIGFARM,        new Vector3(0.7f, 0, -1.1f) },
        {  BuildingType.SHEEPFARM,      new Vector3(0.7f, 0, -1.1f) },
        {  BuildingType.QUARRY,         new Vector3(0.0f, 0, -0.85f) },
        {  BuildingType.SCHOOL,         new Vector3(-0.8f, 0, -0.8f) },
        {  BuildingType.TAVERN,         new Vector3(1, -0.3f, -1.1f) },
        {  BuildingType.WHEATFARM,      new Vector3(-0.25f, 0, -0.85f) },
        {  BuildingType.FORRESTER,      new Vector3(0.5f, 0, 0) },
        {  BuildingType.BARRACKS,       new Vector3(1, 0, -1.2f) },
        {  BuildingType.WEAPONMAKER,    new Vector3(0.4f, 0, 0.9f) },
        {  BuildingType.LEATHERARMORY,  new Vector3(-1f, 0, 0) },
        {  BuildingType.THREADMAKER,    new Vector3(0, 0, -1.1f) },
        {  BuildingType.BUTCHER,        new Vector3(0, 0, -1.6f) },
        {  BuildingType.CLOTHMAKER,     new Vector3(0, 0, -2) },
        {  BuildingType.CHURCH,         new Vector3(0, 0, -3) },
    };
}