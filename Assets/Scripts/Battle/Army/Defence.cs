using System;

namespace Assets.Army
{
    [Serializable]
    public struct Defence
    {
        public float ArmorValue;
        public ArmorType ArmorType;
    }

    public enum ArmorType
    {
        UNARMORED,
        LIGHT,
        HEAVY,
    }
}