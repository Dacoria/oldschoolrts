using System;

[Serializable]
public class ItemOutput
{
    public ItemType ItemType;
    public int MaxBuffer = 5;
    public int ProducedPerProdCycle = 1;
}