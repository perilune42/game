using UnityEngine;

public interface IRanged
{
    public void DisplayRange();
    public void HideRange();
}

public interface ITargetsShip
{
    public AttackData GetAttack(Ship targetShip = null);
    public void ShootShip (Ship targetShip);
    public DamageData GetDamage(Ship targetShip);
    public float GetCritOffset();

}

public interface IHasHitChance
{
    public float ChanceToHit(Ship target, float distance);
    public float ChanceToHit(Ship target, float distance, bool isEvading);
}

public interface IUsesAmmo
{
    public int GetAmmoCount();
    public int GetAmmoCapacity();
}

public interface ILimitedUse
{
    public int GetRemainingUses();
}

public interface IHasCooldown
{
    public int GetCooldown();
}

public interface IShootsProjectile
{
    public int GetProjectileCount();
}

public interface IProjectile
{
    public float ChanceToHit();
    public bool RollForHit();
    public DamageData GetDamage();
    public Ship GetTarget();
    public string GetName();
    public void Destroy();
}

public interface IInverseSquare
{

}