using System;
using System.Collections.Generic;

public static class DamageLookup
{
    public static readonly Dictionary<AttackType, Dictionary<ArmorType, float>> LookUp =
        new Dictionary<AttackType, Dictionary<ArmorType, float>>
        {
            {
                AttackType.PIERCING, new Dictionary<ArmorType, float>
                {
                    {ArmorType.UNARMORED, 1.00f},
                    {ArmorType.LIGHT, 0.75f},
                    {ArmorType.HEAVY, 0.50f},
                }
            },
            {
                AttackType.MELEE, new Dictionary<ArmorType, float>
                {
                    {ArmorType.UNARMORED, 1.00f},
                    {ArmorType.LIGHT, 0.75f},
                    {ArmorType.HEAVY, 0.75f},
                }
            },                
            {
                AttackType.MAGIC, new Dictionary<ArmorType, float>
                {
                    {ArmorType.UNARMORED, 0.75f},
                    {ArmorType.LIGHT, 0.5f},
                    {ArmorType.HEAVY, 1.0f},
                }
            }                
        };

    public static float CalculateAbsoluteDamage(Offence offence, Defence defence)
    {
        var damageTypeModifier = LookUp[offence.AttackType][defence.ArmorType];
        var effectiveDamage = offence.Damage * damageTypeModifier;
        return (float)Math.Round(effectiveDamage, 1);
    }
}