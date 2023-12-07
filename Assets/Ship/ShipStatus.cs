using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipStatus : MonoBehaviour
{
    public Ship ship;

    public int health;
    public int maxHealth;

    public int armorPoints;
    public int maxArmorPoints;
    public int armorLevel;

    public bool isEvading = false;

    public List<KineticProjectile> incomingProjectiles = new List<KineticProjectile>();
    void Awake()
    {
        ship = GetComponent<Ship>();
        maxHealth = ship.shipData.health;
        health = maxHealth;
        maxArmorPoints = ship.shipData.armorPoints;
        armorLevel = ship.shipData.armorLevel;
        armorPoints = maxArmorPoints;
    }

    public void DealDamage(DamageData damage)
    {
        health -= damage.healthDamage;
        armorPoints = Mathf.Max(0, armorPoints - damage.armorDamage);
        if (health <= 0)
        {
            health = 0;

            ship.Destroy();
        }
    }

    public DamageData CalculateDamage(AttackData attack)
    {
        return new DamageData(attack.damage - Mathf.Max(0, armorLevel - attack.armorPierce), attack.damage);
    }

    public void AddProjectile(KineticProjectile projectile)
    {
        incomingProjectiles.Add(projectile);
    }

    public void RollProjectiles()
    {
        foreach (KineticProjectile projectile in incomingProjectiles) {
            bool hit = projectile.RollForHit();
            GameEvents.instance.UpdateProjDisplay(projectile, hit);
        }
        incomingProjectiles.Clear();
    }

    
}
