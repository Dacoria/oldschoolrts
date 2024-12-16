using System;

namespace Assets.Army
{
    [Serializable]
    public struct Defence
    {
        public float Dodge;
        public float ArmorValue;
        public ArmorType ArmorType;

        // buffs

        // debuffs
    }

    public enum ArmorType
    {
        UNARMORED,
        LIGHT,
        MEDIUM,
        HEAVY,
        FORTIFIED
    }
}