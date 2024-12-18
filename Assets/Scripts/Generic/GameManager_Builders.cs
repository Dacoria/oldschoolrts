using System;
using System.Collections.Generic;
using System.Linq;

public partial class GameManager : BaseAEMonoCI
{
    public List<BuildingPrefabItem> BuildingPrefabItems;

    private static List<BuilderBehaviour> freeBuilders = new List<BuilderBehaviour>();
    private static SortedSet<BuilderRequest> BuilderRequests = new SortedSet<BuilderRequest>();

    public SortedSet<BuilderRequest> GetBuilderRequests() => BuilderRequests;

    private void InitCheckBuildingTypes()
    {
        foreach (BuildingType buildingType in Enum.GetValues(typeof(BuildingType)))
        {
            var countItemTypeInDict = BuildingPrefabItems.Count(x => x.BuildingType == buildingType);
            if (buildingType != BuildingType.NONE && countItemTypeInDict != 1)
            {
                throw new Exception("BuildingType " + buildingType + " komt " + countItemTypeInDict + "x voor ipv 1 -- > Zie Grass -> BuildingPrefabItems");
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

    public bool IsFreeBuilder(BuilderBehaviour builder)
    {
        return freeBuilders.Any(x => x == builder);
    }

    public bool TryRemoveBuilderFromFreeBuilderList(BuilderBehaviour builder)
    {
        if (IsFreeBuilder(builder))
        {
            return freeBuilders.Remove(builder);
        }

        return false;
    }
}