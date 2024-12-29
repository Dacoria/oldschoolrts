using Assets.CrossCutting;
public interface IProduceResourceOverTimeDurations
{
    public float TimeToProduceResourceInSeconds { get; }
    public float TimeToWaitAfterProducingInSeconds { get; }
}