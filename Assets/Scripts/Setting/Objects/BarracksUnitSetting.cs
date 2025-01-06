using System;
using UnityEngine;

[Serializable]
public class BarracksUnitSetting : ProductionSetting
{
    public BarracksUnitType Type;
    public GameObject ResourcePrefab;
    public Sprite Icon;
    public override Sprite GetIcon() => Icon;
    public override Enum GetType() => Type;
    public UnitStatsSetting UnitStats;
}