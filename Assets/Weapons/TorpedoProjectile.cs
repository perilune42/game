using UnityEngine;
using System.Collections.Generic;
using System.Net.NetworkInformation;

public class TorpedoProjectile: MonoBehaviour, IProjectile
{
    public DamageData damage;
    public float accuracy;
    public float distance;
    public Ship target;
    TorpedoWeapon weapon;

    public void Init (TorpedoWeapon origin, Ship target) 
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

    DamageData GetPartialDamage()
    {
        return weapon.GetPartialDamage(target);
    }

    public Ship GetTarget()
    {
        return target;
    }

    public string GetName()
    {
        return this.name;
    }

    public void Destroy()
    {
        GameObject.Destroy(gameObject);
    }


    private void Hit()
    {
        damage = GetDamage();
        target.shipStatus.DealDamage(damage);
    }

    private void Miss()
    {
        damage = GetPartialDamage();
        target.shipStatus.DealDamage(damage);
    }
}