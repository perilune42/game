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
}