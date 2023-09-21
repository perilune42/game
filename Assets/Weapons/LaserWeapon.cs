using UnityEngine;

public class LaserWeapon : Weapon
{

    public LaserWeaponSO weaponData;
    public int damage, fallOffRange;
    public override void Init()
    {
        type = TargetType.Direct;
        weaponName = weaponData.weaponName;
        damage = weaponData.damage;
        fallOffRange = weaponData.fallOffRange;
    }

    public void ShootShip(Ship targetShip)
    {
        targetShip.shipStatus.Damage(weaponData.damage);
    }


}