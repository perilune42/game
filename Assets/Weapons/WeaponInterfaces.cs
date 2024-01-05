using UnityEngine;

public interface IRanged
{
    public void DisplayRange();
    public void HideRange();
}

public interface ITargetsShip
{
    public AttackData GetAttack(Ship targetShip);
    public void ShootShip (Ship targetShip);
    public DamageData GetDamage(Ship targetShip);
    

}

public interface IUsesAmmo
{

}

public interface ILimitedUse
{
    public int GetRemainingUses();
}

public interface IHasCooldown
{
    public int GetCooldown();
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