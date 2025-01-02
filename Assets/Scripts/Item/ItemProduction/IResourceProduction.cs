using System.Collections.Generic;
using System.Linq;

public interface IResourceProduction
{
    bool CanProduce(ItemProduceSetting itemProduceSetting);      
}