using System;
using System.Collections.Generic;

namespace Assets.Army
{
    public static class DamageLookup
    {
        public static readonly Dictionary<AttackType, Dictionary<ArmorType, float>> LookUp =
            new Dictionary<AttackType, Dictionary<ArmorType, float>>
            {
                {
                    // Light attacks (arrows, daggers) deal normal damage to light armor, but fail to inflict any real damage any heavier armor
                    AttackType.LIGHT, new Dictionary<ArmorType, float>
                    {
                        {ArmorType.UNARMORED, 1.30f},
                        {ArmorType.LIGHT, 1.00f},
                        {ArmorType.MEDIUM, 0.75f},
                        {ArmorType.HEAVY, 0.60f},
                        {ArmorType.FORTIFIED, 0.35f}
                    }
                },
                {
                    // Medium attacks (battle axe, longsword, bolts) deal extra damage to medium armor, but have trouble penetrating heavy armor
                    AttackType.MEDIUM, new Dictionary<ArmorType, float>
                    {
                        {ArmorType.UNARMORED, 1.30f},
                        {ArmorType.LIGHT, 1.00f},
                        {ArmorType.MEDIUM, 1.30f},
                        {ArmorType.HEAVY, 0.75f},
                        {ArmorType.FORTIFIED, 0.35f}
                    }
                },
                {
                    // Heavy attacks (zweihander / great axe, etc) deal full damage to most armortypes and even deal additional damage to heavy armor
                    AttackType.HEAVY, new Dictionary<ArmorType, float>
                    {
                        {ArmorType.UNARMORED, 1.30f},
                        {ArmorType.LIGHT, 1.00f},
                        {ArmorType.MEDIUM, 1.00f},
                        {ArmorType.HEAVY, 1.30f},
                        {ArmorType.FORTIFIED, 0.60f}
                    }
                },
                {
                    // Siege is exceptionally strong against fortified positions, however lightly and medium armored units are more adapt at avoiding magic damage
                    AttackType.SIEGE, new Dictionary<ArmorType, float>
                    {
                        {ArmorType.UNARMORED, 1.30f},
                        {ArmorType.LIGHT, 0.65f},
                        {ArmorType.MEDIUM, 0.65f},
                        {ArmorType.HEAVY, 1.00f},
                        {ArmorType.FORTIFIED, 1.50f}
                    }
                },
                {
                    // Magic is exceptionally strong against knights and heavily armored units, however lightly armored units are more adapt at avoiding magic damage
                    AttackType.MAGIC, new Dictionary<ArmorType, float>
                    {
                        {ArmorType.UNARMORED, 1.30f},
                        {ArmorType.LIGHT, 0.75f},
                        {ArmorType.MEDIUM, 1.00f},
                        {ArmorType.HEAVY, 1.30f},
                        {ArmorType.FORTIFIED, 0.35f}
                    }
                },
                {
                    // True damage always deals full damage
                    AttackType.TRUE, new Dictionary<ArmorType, float>
                    {
                        {ArmorType.UNARMORED, 1.00f},
                        {ArmorType.LIGHT, 1.00f},
                        {ArmorType.MEDIUM, 1.00f},
                        {ArmorType.HEAVY, 1.00f},
                        {ArmorType.FORTIFIED, 1.00f}
                    }
                }
            };

        public static float CalculateAbsoluteDamage(Offence offence, Defence defence)
        {
            float effectiveArmor = 0;
            if (offence.AttackType != AttackType.TRUE)
            {
                var armorMultiplier = 1 - offence.BonusArmorPenetration;
                effectiveArmor =
                    Math.Max(defence.ArmorValue * armorMultiplier - offence.FlatArmorPenetration, 0);
            }

            var damageTypeModifier = LookUp[offence.AttackType][defence.ArmorType];
            var effectiveDamage = offence.Damage * damageTypeModifier;

            var damageMultiplier = 100 / (100 + effectiveArmor);

            return (float)Math.Round(effectiveDamage * damageMultiplier, 1);
        }
    }
}