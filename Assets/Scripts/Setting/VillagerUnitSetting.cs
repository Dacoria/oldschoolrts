using System;
using UnityEngine;

[Serializable]
public class VillagerUnitSetting : ProductionSetting
{
    public VillagerUnitType Type;
    public IVillagerUnit VillagerBehaviour;
    public Sprite Icon;
    public override Sprite GetIcon() => Icon;
    public override Enum GetType() => Type;
}