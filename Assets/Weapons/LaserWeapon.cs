using UnityEngine;

public class LaserWeapon : Weapon
{

    public LaserWeaponSO weaponData;
    public override void Init()
    {
        type = TargetType.Direct;
        weaponName = weaponData.weaponName;
    }

    public void ShootShip(Ship targetShip)
    {
        targetShip.shipStatus.Damage(weaponData.damage);
    }


}