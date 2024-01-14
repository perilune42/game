
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ShipStatus : MonoBehaviour
{
    public Ship ship;

    public List<IStat> stats = new List<IStat>();
    public StatInt thrust;
    public StatFloat mobility;
    public StatFloat accuracy;
    public int rotateSpeed = 0;

    public int health;
    public int maxHealth;

    public int armorPoints;
    public int maxArmorPoints;
    public int armorLevel;

    public bool isEvading = false;

    public List<IProjectile> incomingProjectiles = new List<IProjectile>();
    public List<StatusEffect> statusEffects = new List<StatusEffect>();


    void Awake()
    {
        ship = GetComponent<Ship>();

        thrust = new StatInt(ship.shipData.thrust);
        mobility = new StatFloat(ship.shipData.mobility);
        maxHealth = ship.shipData.health;
        health = maxHealth;
        maxArmorPoints = ship.shipData.armorPoints;
        armorLevel = ship.shipData.armorLevel;
        armorPoints = maxArmorPoints;

        stats.Add(thrust); //all stats go here
    }




    public bool IsUnderAttack()
    {
        return incomingProjectiles.Count > 0;
    }

    public void AddStatusEffect(StatusEffect newEffect)
    {
        foreach (StatusEffect effect in statusEffects)
        {
            if (effect.GetType() == newEffect.GetType())
            {
                effect.Merge(newEffect);
                Debug.Log("merged effects");
                return;
            }
        }
        statusEffects.Add(newEffect);
        newEffect.Apply(ship);
    }

    public void TickEffects()
    {
        foreach (IStat stat in stats)
        {
            stat.ClearModifiers();
        }
        foreach (StatusEffect effect in statusEffects)
        {
            effect.Tick();
        }
        statusEffects.RemoveAll(x => x.IsActive() == false);
    }

    public void DealDamage(DamageData damage, bool canCrit = true, float critOffset = 0f)
    {
        CritType crit = CritType.none;
        DamageData finalDamage = damage;
        if (damage == DamageData.none)
        {
            GameEvents.instance.HitShip(ship, HitType.Miss, damage, crit);
            return;
        }
        
        if (canCrit) crit = RollForCrit(damage, critOffset);
        if (crit != CritType.none)
        {
            switch (crit)
            {
                case CritType.doubleDamage:
                    finalDamage.healthDamage *= 2;
                    break;
                case CritType.stun:
                    AddStatusEffect(new StunEffect(2));
                    break;
                case CritType.slow:
                    AddStatusEffect(new SlowEffect(2));
                    break;
                case CritType.targeting:
                    AddStatusEffect(new ScrambleEffect(2)); //TODO: Not implemented accuracy stat yet
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

    CritType RollForCrit(DamageData damage, float critOffset)
    {
        float chanceToCrit = GetCritChance(damage, critOffset);
        if (Random.value > chanceToCrit) return CritType.none;
        CritType[] arr = { CritType.slow, CritType.stun, CritType.doubleDamage };
        int choice = Random.Range(0, arr.Length);
        return arr[choice];
    } 
    public float GetCritChance(DamageData damage, float critOffset)
    {
        float damagePercent = (float)damage.healthDamage / maxHealth;
        float chanceToCrit = Mathf.Clamp01(2.5f * damagePercent - 0.25f + critOffset);
        //critical chance formula, linear with endpoints at 10% damage and 50% damage
        return chanceToCrit;
    }

    public float GetCritChance(ITargetsShip weapon)
    {
        return GetCritChance(weapon.GetDamage(ship), weapon.GetCritOffset());
    }

    public DamageData CalculateDamage(AttackData attack)
    {
        return new DamageData(attack.damage - Mathf.Max(0, armorLevel - attack.armorPen), attack.damage);
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
