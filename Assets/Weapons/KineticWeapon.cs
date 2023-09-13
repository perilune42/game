using UnityEngine;

public class KineticWeapon : Weapon
{

    public KineticWeaponSO weaponData;

    public override void Init()
    {
        type = TargetType.Kinetic;
        weaponName = weaponData.weaponName;
    }
    public void ShootShip(Ship targetShip)
    {
        
        float distance = HexCoordinates.Distance(ship.pos, targetShip.pos);
        Debug.Log(distance);
        KineticProjectile projectile = new KineticProjectile(this, targetShip);
        if (distance <= weaponData.idealRange) 
        {
            if (projectile.RollForHit()) projectile.Hit();
        }
        else
        {
            targetShip.shipStatus.AddProjectile(projectile);
        }
    }


}
