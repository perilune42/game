using UnityEngine;

[RequireComponent(typeof(ProjectileRenderer))]
public class LaserWeapon : Weapon, IRanged, ITargetsShip
{

    public LaserWeaponSO weaponData;
    public int damage, fallOffRange, reloadActions; //fallOffRange = damage starts to drop linearly
    public float fallOffRate; //damage per tile after fallOffRange

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


    public void ShootShip(Ship targetShip)
    {
        targetShip.shipStatus.Damage(CalculateDamage(targetShip));
        GameEvents.instance.HitShip(targetShip, HitType.Hit, CalculateDamage(targetShip));
        projectileAnimHandler.Shoot(targetShip.transform.position, weaponData.visualProjectilePrefab);
    }

    public int CalculateDamage(Ship targetShip)
    {
        int finalDamage;
        float distance = HexCoordinates.Distance(targetShip.pos, ship.pos);
        finalDamage = DamageFallOff(damage, distance);
        return finalDamage;
    }
    int DamageFallOff(int baseDamage, float dist)
    {
        if (dist <= fallOffRange)
        {
            return damage;
        }
        else {
            return Mathf.Max(Mathf.CeilToInt( damage - (dist - fallOffRange) * fallOffRate), 1);
        }
    }

    public override bool CanFire()
    {
        return true;
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