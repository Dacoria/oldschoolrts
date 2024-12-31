using System.Collections.Generic;

public enum MaterialResourceType
{
    NONE,
    STONE,
    WOODLOG,
    WHEAT,
    GOLDORE,
    COALORE,
    IRONORE,
    FISH,
    WILDANIMAL,
    REDBERRIES
}
public static class MaterialHelper
{
    private static Dictionary<MaterialResourceType, ItemType> ItemForMaterial = new Dictionary<MaterialResourceType, ItemType>
    {
        { MaterialResourceType.STONE,       ItemType.STONE },
        { MaterialResourceType.WOODLOG,     ItemType.LUMBER },
        { MaterialResourceType.COALORE,     ItemType.COALORE },
        { MaterialResourceType.GOLDORE,     ItemType.GOLDORE },
        { MaterialResourceType.IRONORE,     ItemType.IRONORE },
        { MaterialResourceType.REDBERRIES,  ItemType.REDBERRIES },
        { MaterialResourceType.WILDANIMAL,  ItemType.WILDMEAT },
        { MaterialResourceType.FISH,        ItemType.FISH },
        { MaterialResourceType.WHEAT,       ItemType.WHEAT },
    };

    public static ItemType GetItemType(this MaterialResourceType type)
    {
        return ItemForMaterial[type];
    }
}