using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProductionSetting
{
    public List<ItemAmountBuffer> ItemsConsumedToProduce;
    public new abstract Enum GetType();
    public abstract Sprite GetIcon();
}