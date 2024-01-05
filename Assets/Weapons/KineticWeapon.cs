using System.Linq;
using UnityEngine;


[RequireComponent(typeof(ProjectileRenderer))]
public class KineticWeapon : Weapon, ITargetsShip, IRanged, IUsesAmmo, IHasCooldown
{
    
    

    public KineticWeaponSO weaponData;
    public int idealRange, ammoCapacity, reloadActions, ammoCount, projectileCount;
    //public AttackData attack;
    public float accuracy;
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


        ammoCount = ammoCapacity;
        cooldownTimer = 0;

        projectileAnimHandler = GetComponent<ProjectileRenderer>();
        rangeLineRenderer = HexGrid.instance.weaponRangeDisplay;
    }

    public AttackData GetAttack(Ship targetShip)
    {
        return new AttackData(weaponData.damage, weaponData.armorPierce);
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

    public DamageData GetDamage(Ship targetShip)
    {
        return Weapon.CalcMultipleDamage(GetAttack(targetShip), targetShip, projectileCount);
    }

    public DamageData GetSingleDamage(Ship targetShip)
    {
        return Weapon.CalcBasicDamage(GetAttack(targetShip), targetShip);
    }

    public float ChanceToHit(Ship target, float distance)
    {
        if (target.shipStatus.isEvading)
            return Mathf.Max(0, (accuracy - weaponData.RangePenalty(distance)) * (1 - weaponData.EvasionPenalty(distance)));
        else return Mathf.Max(0, accuracy - weaponData.RangePenalty(distance));
    }

    public float ChanceToHitPreview(Ship target, float distance, bool isEvading)
    {
        if (isEvading)
            return Mathf.Max(0, (accuracy - weaponData.RangePenalty(distance)) * (1 - weaponData.EvasionPenalty(distance)));
        else return Mathf.Max(0, accuracy - weaponData.RangePenalty(distance));
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

    public void HideRange()
    {
        rangeLineRenderer.enabled = false;
    }

}
