using System.Linq;
using UnityEngine;

public class KineticWeapon : Weapon
{

    public KineticWeaponSO weaponData;
    public int damage, idealRange;
    public float accuracy;
    ProjectileRenderer projectileAnimHandler;

    public LineRenderer lineRenderer;

    public override void Init()
    {
        type = TargetType.Kinetic;
        weaponName = weaponData.weaponName;
        damage = weaponData.damage;
        idealRange = weaponData.idealRange;
        accuracy = weaponData.accuracy;
        projectileAnimHandler = GetComponent<ProjectileRenderer>();
    }
    public void ShootShip(Ship targetShip)
    {
        
        float distance = HexCoordinates.Distance(ship.pos, targetShip.pos);
        Debug.Log(distance);
        GameObject projObject = new GameObject(weaponName + " projectile");
        KineticProjectile projectile = projObject.AddComponent<KineticProjectile>();
        projObject.transform.parent = targetShip.transform;
        projectile.Init(this, targetShip);

        if (distance <= idealRange) 
        {
            projectile.RollForHit();
            Destroy(projObject);
        }
        else
        {
            targetShip.shipStatus.AddProjectile(projectile);
        }
        projectileAnimHandler.Shoot(targetShip.transform.position,weaponData.visualProjectilePrefab);
    }

    public float ChanceToHit(Ship target)
    {
        return accuracy; //modifiers, range penalties
    }

    public void DisplayRange()
    {
        lineRenderer.positionCount = 0;
        HexPatch rangeArea = new HexPatch(ship.pos, 5);
        foreach (Vector3 vertex in rangeArea.GetRelativeOuterVertices())

        {
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, vertex + ship.transform.position);
        }

    }

}
