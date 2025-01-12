using System.Collections.Generic;
using System.Linq;

public static class SetupBuildingBuildDurationSetting
{
    private static Dictionary<BuildingType, BuildDurations> cache = new Dictionary<BuildingType, BuildDurations>();

    public static BuildDurations GetBuildDurationSettings(this BuildingType type)
    {
        if (!cache.ContainsKey(type))
        {
            cache.Add(type, GetBuildDurationSettingsNoCache(type));
        }
        return cache[type];
    }

    private static BuildDurations GetBuildDurationSettingsNoCache(BuildingType type)
    {
        var buildCosts = type.GetBuildCosts();
        return new BuildDurations
        {
            TimeToPrepareBuildingInSeconds = buildCosts.Sum(x => x.Amount) * 2,
            TimeToBuildRealInSeconds = buildCosts.Sum(x => x.Amount) * 3,
        };
    }
}