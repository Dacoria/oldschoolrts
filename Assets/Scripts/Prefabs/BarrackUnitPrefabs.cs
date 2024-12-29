using System;
using System.Collections.Generic;
using Assets;
using UnityEngine;

public static class BarrackUnitPrefabs
{
    private static List<BarracksUnitSetting> _prefabs;
    public static List<BarracksUnitSetting> Get() 
    {        
        if (_prefabs == null)
        {
            _prefabs = GenerateBarracksUnitPrefabs();
        }
        return _prefabs;        
    }

    private static List<BarracksUnitSetting> GenerateBarracksUnitPrefabs()
    {
        var result = new List<BarracksUnitSetting>
        {
            GetSwordFighter(),
            GetArcher(),
            GetMage()
        };

        return result;
    }

    private static BarracksUnitSetting GetStartSettings(BarracksUnitType type)
    {
        if (Load.GoMap.TryGetValue($"{type.ToString()}Prefab", out GameObject prefab))
        {
            if (Load.SpriteMap.TryGetValue($"{type.ToString()}Image", out Sprite sprite))
            {
                return new BarracksUnitSetting
                {
                    Icon = sprite,
                    Type = type,
                    ResourcePrefab = prefab
                };
            }
            else
            {
                throw new Exception($"BarracksUnitType {type} heeft geen Image :O");
            }
        }
        else
        {
            throw new Exception($"BarracksUnitType {type} heeft geen Go :O");
        }
    }

    private static BarracksUnitSetting GetSwordFighter()
    {
        var result = GetStartSettings(BarracksUnitType.SWORDFIGHTER);
        result.ItemsConsumedToProduce = new List<ItemAmountBuffer>
            {
                new ItemAmountBuffer{ Amount = 1, ItemType = ItemType.IRONSWORD, MaxBuffer = 10},
                new ItemAmountBuffer{ Amount = 1, ItemType = ItemType.IRONHELM, MaxBuffer = 10},
                new ItemAmountBuffer{ Amount = 1, ItemType = ItemType.IRONSHIELD, MaxBuffer = 10},
                new ItemAmountBuffer{ Amount = 1, ItemType = ItemType.IRONARMOR, MaxBuffer = 10},
            };
        result.UnitStats = new UnitStatsSetting
        {
            Offence = new Assets.Army.Offence
            {
                AttackHitRate = 1,
                AttackType = Assets.Army.AttackType.MEDIUM,
                BaseBonusArmorPenetrationPercentage = 10,
                BaseDamage = 10,
                BaseFlatArmorPenetration = 5
            },
            Defence = new Assets.Army.Defence
            {
                ArmorType = Assets.Army.ArmorType.HEAVY,
                ArmorValue = 10,
                Dodge = 10
            },
            Health = 100,
            RangeToAttack = 2,
            RangeToAttractEnemies = 10,
            Speed = 1
        };

        return result;        
    }

    private static BarracksUnitSetting GetMage()
    {
        var result = GetStartSettings(BarracksUnitType.MAGE);
        result.ItemsConsumedToProduce = new List<ItemAmountBuffer>
            {
                new ItemAmountBuffer{ Amount = 1, ItemType = ItemType.CLOTHARMOR, MaxBuffer = 10},
                new ItemAmountBuffer{ Amount = 1, ItemType = ItemType.CLOTHPANTS, MaxBuffer = 10},
                new ItemAmountBuffer{ Amount = 1, ItemType = ItemType.CLOTHBOOTS, MaxBuffer = 10},
                new ItemAmountBuffer{ Amount = 1, ItemType = ItemType.CLOTHHELMET, MaxBuffer = 10},
                new ItemAmountBuffer{ Amount = 1, ItemType = ItemType.WOODENMAGESTAFF, MaxBuffer = 10},
            };
        result.UnitStats = new UnitStatsSetting
        {
            Offence = new Assets.Army.Offence
            {
                AttackHitRate = 1,
                AttackType = Assets.Army.AttackType.MAGIC,
                BaseBonusArmorPenetrationPercentage = 10,
                BaseDamage = 10,
                BaseFlatArmorPenetration = 5
            },
            Defence = new Assets.Army.Defence
            {
                ArmorType = Assets.Army.ArmorType.LIGHT,
                ArmorValue = 10,
                Dodge = 10
            },
            Health = 80,
            RangeToAttack = 10,
            RangeToAttractEnemies = 10,
            Speed = 1
        };
        return result;
    }

    private static BarracksUnitSetting GetArcher()
    {
        var result = GetStartSettings(BarracksUnitType.ARCHER);
        result.ItemsConsumedToProduce = new List<ItemAmountBuffer>
            {
                new ItemAmountBuffer{ Amount = 1, ItemType = ItemType.LEATHERARMOR, MaxBuffer = 10},
                new ItemAmountBuffer{ Amount = 1, ItemType = ItemType.LEATHERBOOTS, MaxBuffer = 10},
                new ItemAmountBuffer{ Amount = 1, ItemType = ItemType.LEATHERPANTS, MaxBuffer = 10},
                new ItemAmountBuffer{ Amount = 1, ItemType = ItemType.WOODENBOW, MaxBuffer = 10},
            };
        result.UnitStats = new UnitStatsSetting
        {
            Offence = new Assets.Army.Offence
            {
                AttackHitRate = 1,
                AttackType = Assets.Army.AttackType.LIGHT,
                BaseBonusArmorPenetrationPercentage = 10,
                BaseDamage = 10,
                BaseFlatArmorPenetration = 5
            },
            Defence = new Assets.Army.Defence
            {
                ArmorType = Assets.Army.ArmorType.MEDIUM,
                ArmorValue = 10,
                Dodge = 10
            },
            Health = 70,
            RangeToAttack = 12,
            RangeToAttractEnemies = 8,
            Speed = 1
        };
        return result;
    }
}