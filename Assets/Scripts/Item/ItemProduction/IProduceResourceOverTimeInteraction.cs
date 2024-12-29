using Assets.CrossCutting;
public interface IProduceResourceOverTimeInteraction
{
    void StartedProducing(ItemProduceSetting itemProduceSetting);
    void FinishedProducing(ItemProduceSetting itemProduceSetting);
    void FinishedWaitingAfterProducing(ItemProduceSetting itemProduceSetting);
    float GetTimeToProduceResourceInSeconds();
    float GetTimeToWaitAfterProducingInSeconds();
    bool CanProduceResource();
}