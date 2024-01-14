using JetBrains.Annotations;
using UnityEngine;

[RequireComponent(typeof(ProjectileRenderer))]
public class LaserWeapon : Weapon, IRanged, ITargetsShip, IHasCooldown
{

    public LaserWeaponSO weaponData;
    public int damage, fallOffRange, reloadActions; //fallOffRange = damage starts to drop linearly
    public float fallOffRate, critOffset; //damage per tile after fallOffRange
    int cooldownTimer;

    ProjectileRenderer projectileAnimHandler;
    LineRenderer lineRenderer;

    public override void Init()
    {
        type = TargetType.Direct;
        weaponName = weaponData.weaponName;
        damage = weaponData.damage;
        fallOffRange = weaponData.fallOffRange;
        fallOffRate = weaponData.fallOffRate;
        reloadActions = weaponData.reloadActions;
        critOffset = weaponData.critOffset;

        projectileAnimHandler = GetComponent<ProjectileRenderer>();
        lineRenderer = HexGrid.instance.weaponRangeDisplay;
    }

    public float GetCritOffset()
    {
        return critOffset;
    }
    public AttackData GetAttack(Ship targetShip = null)
    {
        if (targetShip == null)
        {
            return new AttackData(damage, weaponData.armorPen, weaponData.armorBonus);
        }
        int finalDamage;
        float distance = HexCoordinates.Distance(targetShip.pos, ship.pos);
        finalDamage = DamageFallOff(damage, distance);
        return new AttackData(finalDamage, weaponData.armorPen, weaponData.armorBonus);
    }

    public void ShootShip(Ship targetShip)
    {
        targetShip.shipStatus.DealDamage(GetDamage(targetShip), true, critOffset);
        
        projectileAnimHandler.Shoot(targetShip.transform.position, weaponData.visualProjectilePrefab);

        cooldownTimer = reloadActions;
    }

    public DamageData GetDamage(Ship targetShip)
    {
        return Weapon.CalcBasicDamage(GetAttack(targetShip), targetShip);
    }
    int DamageFallOff(int baseDamage, float dist)
    {
        if (dist <= fallOffRange)
        {
            return baseDamage;
        }
        else {
            return Mathf.Max(Mathf.CeilToInt( (baseDamage - 1) - (dist - fallOffRange) * fallOffRate), 1);
        }
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
        }
    }

    public int GetCooldown()
    {
        return cooldownTimer;
    }
    public void DisplayRange()
    {
        lineRenderer.enabled = true;
        lineRenderer.positionCount = 0;
        HexPatch rangeArea = new HexPatch(ship.pos, fallOffRange);
        foreach (Vector3 vertex in rangeArea.GetRelativeOuterVertices())

        {
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, vertex + ship.transform.position);
        }
    }

    public void HideRange()
    {
        lineRenderer.enabled = false;
    }


}