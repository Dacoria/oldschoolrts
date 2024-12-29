using Assets.CrossCutting;
public interface IProduceResourceOverTimeDurations
{
    float TimeToProduceResourceInSeconds { get; }
    float TimeToWaitAfterProducingInSeconds { get; }
}