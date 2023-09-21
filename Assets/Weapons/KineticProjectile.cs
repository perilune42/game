using UnityEngine;
using System.Collections.Generic;

public class KineticProjectile: MonoBehaviour
{
    public int damage;
    public float accuracy;
    public float distance;
    Ship target;

    public void Init (KineticWeapon origin, Ship target) 
    {
        damage = origin.damage;
        accuracy = origin.accuracy;
        distance = HexCoordinates.Distance(origin.ship.pos, target.pos);
        this.target = target;
    }
    
    public float ChanceToHit() //modifier 
    {
        return accuracy;
    }
    public bool RollForHit() 
    {
        bool hit = Random.value <= ChanceToHit();
        if (hit) Hit();
        return hit;
    }

    public void Hit()
    {
        target.shipStatus.Damage(damage); 
        
        
    }
}