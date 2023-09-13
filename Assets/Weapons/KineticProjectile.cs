using UnityEngine;
using System.Collections.Generic;

public class KineticProjectile
{
    public int damage;
    public float accuracy;
    public float distance;
    Ship target;

    public KineticProjectile (KineticWeapon origin, Ship target) 
    {
        damage = origin.weaponData.damage;
        accuracy = origin.weaponData.accuracy;
        distance = HexCoordinates.Distance(origin.ship.pos, target.pos);
        this.target = target;
    }
    
    public float ChanceToHit() //modifier 
    {
        return accuracy;
    }
    public bool RollForHit() 
    {
        return Random.value <= ChanceToHit();
    }

    public void Hit()
    {
        target.shipStatus.Damage(damage);
        //Destroyself
    }
}