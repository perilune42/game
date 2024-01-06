using JetBrains.Annotations;
using UnityEngine;

[RequireComponent(typeof(ProjectileRenderer))]
public class LaserWeapon : Weapon, IRanged, ITargetsShip, IHasCooldown
{

    public LaserWeaponSO weaponData;
    public int damage, fallOffRange, reloadActions; //fallOffRange = damage starts to drop linearly
    public float fallOffRate; //damage per tile after fallOffRange
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

        projectileAnimHandler = GetComponent<ProjectileRenderer>();
        lineRenderer = HexGrid.instance.weaponRangeDisplay;
    }

    public AttackData GetAttack(Ship targetShip)
    {
        int finalDamage;
        float distance = HexCoordinates.Distance(targetShip.pos, ship.pos);
        finalDamage = DamageFallOff(damage, distance);
        return new AttackData(finalDamage, weaponData.armorPierce, weaponData.armorBonus);
    }

    public void ShootShip(Ship targetShip)
    {
        targetShip.shipStatus.DealDamage(GetDamage(targetShip));
        
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