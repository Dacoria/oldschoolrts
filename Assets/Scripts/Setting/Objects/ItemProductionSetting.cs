﻿using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class ItemProductionSetting : ProductionSetting
{
    public ItemType Type;

    public int MaxBuffer = 5;
    public override Enum GetType() => Type;
    public override Sprite GetIcon() => ResourcePrefabs.Get().First(x => x.ItemType == Type).Icon;
}