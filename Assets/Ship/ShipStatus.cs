using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipStatus : MonoBehaviour
{
    public Ship ship;

    public int health;
    public int maxHealth;

    public bool isEvading = false;

    public List<KineticProjectile> incomingProjectiles = new List<KineticProjectile>();
    void Awake()
    {
        ship = GetComponent<Ship>();
        maxHealth = ship.shipData.health;
        health = maxHealth;
    }

    public void Damage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            health = 0;

            ship.Destroy();
        }
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
