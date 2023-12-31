using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ShipStatus : MonoBehaviour
{
    public Ship ship;

    public int health;
    public int maxHealth;

    public int armorPoints;
    public int maxArmorPoints;
    public int armorLevel;

    public bool isEvading = false;

    public List<IProjectile> incomingProjectiles = new List<IProjectile>();
    public List<IStatusEffect> statusEffects = new List<IStatusEffect>();


    void Awake()
    {
        ship = GetComponent<Ship>();
        maxHealth = ship.shipData.health;
        health = maxHealth;
        maxArmorPoints = ship.shipData.armorPoints;
        armorLevel = ship.shipData.armorLevel;
        armorPoints = maxArmorPoints;
    }

    public bool IsUnderAttack()
    {
        return incomingProjectiles.Count > 0;
    }

    public void AddStatusEffect(IStatusEffect effect)
    {
        statusEffects.Add(effect);
    }

    public void TickEffects()
    {
        foreach (IStatusEffect effect in statusEffects)
        {
            effect.Tick();
        }
        statusEffects.RemoveAll(x => x.IsActive() == false);
    }

    public void DealDamage(DamageData damage, bool canCrit = true)
    {
        CritType crit = CritType.none;
        DamageData finalDamage = damage;
        if (damage == DamageData.none)
        {
            GameEvents.instance.HitShip(ship, HitType.Miss, damage, crit);
            return;
        }
        
        if (canCrit) crit = RollForCrit(damage);
        if (crit != CritType.none)
        {
            switch (crit)
            {
                case CritType.doubleDamage:
                    finalDamage.healthDamage *= 2;
                    break;
                case CritType.stun:
                    this.AddStatusEffect(new StunEffect(ship, 2));
                    break;
            }
        }

        GameEvents.instance.HitShip(ship, HitType.Hit, finalDamage, crit);
        health -= finalDamage.healthDamage;
        armorPoints = Mathf.Max(0, armorPoints - finalDamage.armorDamage);
        if (health <= 0)
        {
            health = 0;

            ship.Destroy();
        }
        
    }

    CritType RollForCrit(DamageData damage)
    {
        float chanceToCrit = GetCritChance(damage);
        if (Random.value > chanceToCrit) return CritType.none;
        return CritType.stun; //temp
    }

    public float GetCritChance(DamageData damage)
    {
        float damagePercent = (float)damage.healthDamage / maxHealth;
        float chanceToCrit = Mathf.Clamp01(2.5f * damagePercent - 0.25f);
        return chanceToCrit;
    }

    public DamageData CalculateDamage(AttackData attack)
    {
        return new DamageData(attack.damage - Mathf.Max(0, armorLevel - attack.armorPierce), attack.damage);
    }

    public void AddProjectile(IProjectile projectile)
    {
        incomingProjectiles.Add(projectile);
    }

    public void RollProjectiles()
    {
        foreach (IProjectile projectile in incomingProjectiles) {
            bool hit = projectile.RollForHit();
            GameEvents.instance.UpdateProjDisplay(projectile, hit);
        }
        incomingProjectiles.Clear();
    }

    
}
