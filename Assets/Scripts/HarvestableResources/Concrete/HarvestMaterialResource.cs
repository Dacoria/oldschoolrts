public class HarvestMaterialResource
{
    public MaterialResourceType Type;
    public int MaterialCount;

    public HarvestMaterialResource(MaterialResourceType type, int materialCount)
    {
        Type = type;
        MaterialCount = materialCount;
    }
}