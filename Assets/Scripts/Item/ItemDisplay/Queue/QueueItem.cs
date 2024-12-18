using System;

public class QueueItem
{
    public Enum Type;
    public DateTime? StartTimeBeingBuild;
    public bool IsBeingBuild => StartTimeBeingBuild.HasValue;

}