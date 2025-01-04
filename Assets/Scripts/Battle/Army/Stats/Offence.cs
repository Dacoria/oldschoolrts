using System;

[Serializable]
public struct Offence
{
    public float AttackHitRate;
    public AttackType AttackType;
    public float BaseDamage;
    public float Damage => BaseDamage;        
}

public enum AttackType
{
    PIERCING,
    MELEE,
    MAGIC,
}
