using Assets.CrossCutting;
using System.Collections.Generic;

public interface IResourcesToProduce
{
    bool CanProduceResource();
    List<ItemOutput> GetAvailableItemsToProduce();
    ItemProduceSetting GetItemToProduce();
    bool ConsumeRequiredResources(ItemProduceSetting itemProduceSetting);
}