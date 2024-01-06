using System;

public struct AttackData
{
    public int damage;
    public int armorPierce;
    public int damageType;
    public float armorBonus;

    public AttackData(int damage, int armorPierce, float armorBonus = 1f)
    {
        this.damage = damage;
        this.armorPierce = armorPierce;
        this.damageType = 0; //ph
        this.armorBonus = armorBonus;
    }
}

public struct DamageData
{
    public int healthDamage;
    public int armorDamage;

    public DamageData(int healthDamage, int armorDamage)
    {
        this.healthDamage = healthDamage;
        this.armorDamage = armorDamage;
    }

    public static DamageData none = new DamageData(0, 0);

    public override bool Equals(object obj)
    {
        if (obj is DamageData otherDamage) 
        {
            return this.healthDamage == otherDamage.healthDamage && this.armorDamage == otherDamage.armorDamage;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(healthDamage, armorDamage);
    }

    public static bool operator ==(DamageData left, DamageData right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(DamageData left, DamageData right)
    {
        return !(left == right);
    }
}