using Assets.CrossCutting;
using System.Collections.Generic;

public interface IProduceResourceOverTime
{
    void StartProducing(ItemProduceSetting itemProduceSetting);
    void FinishProducing(ItemProduceSetting itemProduceSetting);
    float GetTimeToProduceResourceInSeconds();
    float GetTimeToWaitAfterProducingInSeconds();
    bool CanProduceResource();
}