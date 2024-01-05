using UnityEngine;
using System.Collections.Generic;
using System.Net.NetworkInformation;

public class KineticProjectile: MonoBehaviour
{
    public DamageData damage;
    public float accuracy;
    public float distance;
    public Ship target;
    KineticWeapon weapon;

    public void Init (KineticWeapon origin, Ship target) 
    {
        this.weapon = origin;
        damage = origin.GetSingleDamage(target);
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

    public DamageData GetDamage()
    {
        return weapon.GetSingleDamage(target);
    }

    public Ship GetTarget()
    {
        return target;
    }

    public string GetName()
    {
        return this.name;
    }

    public void Destoy()
    {
        GameObject.Destroy(gameObject);
    }

    private void Hit()
    {
        damage = GetDamage();
        target.shipStatus.DealDamage(damage);
        GameEvents.instance.HitShip(target, HitType.Hit, damage.healthDamage);
    }

    private void Miss()
    {
        GameEvents.instance.HitShip(target, HitType.Miss, 0);
    }
}