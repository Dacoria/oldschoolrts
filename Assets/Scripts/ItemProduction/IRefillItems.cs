using Assets;
using Assets.CrossCutting;
using System.Collections.Generic;

public interface IRefillItems
{
    bool AlwaysRefillItemsIgnoreBuffer();
    List<ItemProduceSetting> GetItemProduceSettings();
}