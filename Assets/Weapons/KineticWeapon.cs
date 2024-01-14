using System.Linq;
using UnityEngine;


[RequireComponent(typeof(ProjectileRenderer))]
public class KineticWeapon : Weapon, ITargetsShip, IRanged, IUsesAmmo, IHasCooldown, IHasHitChance, IShootsProjectile
{
    
    

    public KineticWeaponSO weaponData;
    public int idealRange, ammoCapacity, reloadActions, ammoCount, projectileCount;
    //public AttackData attack;
    public float accuracy, critOffset;
    public int cooldownTimer;

    ProjectileRenderer projectileAnimHandler;
    LineRenderer rangeLineRenderer;



    public override void Init()
    {
        
        type = TargetType.Kinetic;
        weaponName = weaponData.weaponName;
        idealRange = weaponData.idealRange;
        accuracy = weaponData.accuracy;
        ammoCapacity = weaponData.ammoCapacity;
        reloadActions = weaponData.reloadActions;
        projectileCount = weaponData.projectileCount;
        critOffset = weaponData.critOffset;


        ammoCount = ammoCapacity;
        cooldownTimer = 0;

        projectileAnimHandler = GetComponent<ProjectileRenderer>();
        rangeLineRenderer = HexGrid.instance.weaponRangeDisplay;
    }

    public float GetCritOffset()
    {
        return critOffset;
    }

    public int GetProjectileCount()
    {
        return projectileCount;
    }

    public AttackData GetAttack(Ship targetShip = null)
    {
        return new AttackData(weaponData.damage, weaponData.armorPen);
    }
    public void ShootShip(Ship targetShip)
    {
        
        float distance = HexCoordinates.Distance(ship.pos, targetShip.pos);
        for (int i = 0; i < projectileCount; i++)
        {
            GameObject projObject = new GameObject(weaponName + " projectile");
            KineticProjectile projectile = projObject.AddComponent<KineticProjectile>();
            projObject.transform.parent = targetShip.transform;
            projectile.Init(this, targetShip);

            if (distance <= idealRange)
            {
                projectile.RollForHit();
                Destroy(projObject);
                if (targetShip.isDestroyed) break;
            }
            else
            {
                targetShip.shipStatus.AddProjectile((IProjectile)projectile);
            }
        }
        projectileAnimHandler.Shoot(targetShip.transform.position,weaponData.visualProjectilePrefab, projectileCount);

        ammoCount--;
        if (ammoCount == 0)
        {
            cooldownTimer = reloadActions;
        }
        GameEvents.instance.UpdateUI();
    }

    public int GetAmmoCount()
    {
        return ammoCount;
    }
    public int GetAmmoCapacity()
    {
        return ammoCapacity;
    }

    public DamageData GetDamage(Ship targetShip)
    {
        return Weapon.CalcMultipleDamage(GetAttack(), targetShip, projectileCount);
    }

    public DamageData GetSingleDamage(Ship targetShip)
    {
        return Weapon.CalcBasicDamage(GetAttack(), targetShip);
    }

    public float ChanceToHit(Ship target, float distance)
    {
        return ChanceToHit(target, distance, target.shipStatus.isEvading);
    }

    public float ChanceToHit(Ship target, float distance, bool isEvading)
    {
        if (isEvading)
            return Mathf.Clamp01((accuracy - RangePenalty(distance)) * (1 - target.shipStatus.mobility.Get() * EvasionPenalty(distance)));
        else return Mathf.Clamp01(accuracy - RangePenalty(distance));
    }

    
    public override bool CanFire()
    {
        return cooldownTimer == 0;
    }

    public override void PassTurn()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer--;
            if (cooldownTimer == 0) Reload();
        }
    }
    public int GetCooldown()
    {
        return cooldownTimer;
    }



    public void DisplayRange()
    {
        rangeLineRenderer.enabled = true;
        rangeLineRenderer.positionCount = 0;
        HexPatch rangeArea = new HexPatch(ship.pos, idealRange);
        foreach (Vector3 vertex in rangeArea.GetRelativeOuterVertices())

        {
            rangeLineRenderer.positionCount++;
            rangeLineRenderer.SetPosition(rangeLineRenderer.positionCount - 1, vertex + ship.transform.position);
        }

    }



    void Reload()
    {
        ammoCount = ammoCapacity;
    }

    public float RangePenalty(float distance)
    {
        return distance / (20 * weaponData.velocity); //10 velocity: 10 cells = 5%, 20 cells = 10%
    }
    public float EvasionPenalty(float distance)
    {
        return distance / (5 * weaponData.velocity);
    }

    public void HideRange()
    {
        rangeLineRenderer.enabled = false;
    }

}
