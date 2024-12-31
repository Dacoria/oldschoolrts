using Assets.CrossCutting;
using System.Collections.Generic;
using System.Linq;

public interface IResourcesToProduceSettings
{
    List<ItemProduceSetting> GetResourcesToProduceSettings();    
    ItemProduceSetting GetItemToProduceSettings();
    bool CanProduceResource() => GetItemToProduceSettings() != null;
    bool ConsumeRequiredResources(ItemProduceSetting itemProduceSetting);
    List<ItemOutput> GetAvailableItemsToProduce() => GetResourcesToProduceSettings().SelectMany(x => x.ItemsToProduce).ToList();
}