using System;

namespace Assets.Army
{
    [Serializable]
    public struct Offence
    {
        public float AttackHitRate;
        public AttackType AttackType;

        public float BaseBonusArmorPenetrationPercentage;
        public float BaseDamage;
        public float BaseFlatArmorPenetration;

        public float Damage => BaseDamage; // + buffs

        // Flat penetration is calculated after bonus penetration, and reduces armor by that absolute number
        public float FlatArmorPenetration => BaseFlatArmorPenetration; // + buffs any absolute number
        
        // Bonus armor penetration is calculated before flat penetration, and reduces armor by that percentage// Bonus armor penetration is calculated before flat penetration, and reduces armor by that percentage
        public float BonusArmorPenetration => BaseBonusArmorPenetrationPercentage; // + buffs, between 0 and 1
        


        //buffs

        //debuffs
    }

    public enum AttackType
    {
        LIGHT,
        MEDIUM,
        HEAVY,
        SIEGE,
        MAGIC,
        TRUE
    }
}