using Assets.CrossCutting;
using System;
using System.Collections.Generic;

using UnityEngine;

[Serializable]
public class VillagerUnitSetting : ProductionSetting
{
    public VillagerUnitType Type;
    public GameObject ResourcePrefab;
    public Sprite Icon;
    public override Sprite GetIcon() => Icon;
    public override Enum GetType() => Type;
}