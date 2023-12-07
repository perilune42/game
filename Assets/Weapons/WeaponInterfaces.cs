interface IRanged
{
    public void DisplayRange();
    public void HideRange();
}

interface ITargetsShip
{
    public AttackData GetAttack(Ship targetShip);
    public void ShootShip (Ship targetShip);
    public DamageData GetDamage(Ship targetShip);
    

}

interface IUsesAmmo
{

}

interface IHasCooldown
{
    public int GetCooldown();
}