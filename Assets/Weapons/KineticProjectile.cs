using UnityEngine;
using System.Collections.Generic;

public class KineticProjectile: MonoBehaviour
{
    public DamageData damage;
    public float accuracy;
    public float distance;
    Ship target;
    KineticWeapon weapon;

    public void Init (KineticWeapon origin, Ship target) 
    {
        this.weapon = origin;
        damage = origin.GetDamage(target);
        accuracy = origin.accuracy;
        distance = HexCoordinates.Distance(origin.ship.pos, target.pos);
        this.target = target;
        
    }
    
    public float ChanceToHit() //modifier 
    {
        return weapon.ChanceToHit(target, distance);
    }
    public bool RollForHit() 
    {
        bool hit = Random.value <= ChanceToHit();
        if (hit) Hit();
        else Miss();
        return hit;
    }

    private void Hit()
    {
        target.shipStatus.DealDamage(damage);
        GameEvents.instance.HitShip(target, HitType.Hit, damage.healthDamage);
    }

    private void Miss()
    {
        GameEvents.instance.HitShip(target, HitType.Miss, 0);
    }
}