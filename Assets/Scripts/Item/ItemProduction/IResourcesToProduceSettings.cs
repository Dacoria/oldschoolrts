using Assets.CrossCutting;
using System.Collections.Generic;
using System.Linq;

public interface IResourcesToProduceSettings
{
    List<ItemProduceSetting> GetResourcesToProduceSettings();
    bool CanProduceResource();
    ItemProduceSetting GetItemToProduceSettings();
    bool ConsumeRequiredResources(ItemProduceSetting itemProduceSetting);
    List<ItemOutput> GetAvailableItemsToProduce() => GetResourcesToProduceSettings().SelectMany(x => x.ItemsToProduce).ToList();
}