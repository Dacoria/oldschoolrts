using System.Collections.Generic;
using System.Linq;

public partial class GameManager : BaseAEMonoCI
{ 
    private static List<BuilderBehaviour> freeBuilders = new List<BuilderBehaviour>();
    private static SortedSet<BuilderRequest> BuilderRequests = new SortedSet<BuilderRequest>();

    public SortedSet<BuilderRequest> GetBuilderRequests() => BuilderRequests;

    protected override void OnBuilderRequest(BuilderRequest builderRequest)
    {
        if (freeBuilders.Count > 0)
        {
            var builder = freeBuilders.PopClosest(builderRequest.Location);
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
}