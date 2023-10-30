﻿using System.Linq;
using UnityEngine;


[RequireComponent(typeof(ProjectileRenderer))]
public class KineticWeapon : Weapon, ITargetsShip, IRanged
{
    
    

    public KineticWeaponSO weaponData;
    public int damage, idealRange, ammoCapacity, reloadActions, ammoCount;
    public float accuracy, evasionPenaltyPerCell;
    public int cooldown;

    ProjectileRenderer projectileAnimHandler;
    LineRenderer lineRenderer;



    public override void Init()
    {

        
        type = TargetType.Kinetic;
        weaponName = weaponData.weaponName;
        damage = weaponData.damage;
        idealRange = weaponData.idealRange;
        accuracy = weaponData.accuracy;
        evasionPenaltyPerCell = weaponData.evasionPenaltyPerCell;
        ammoCapacity = weaponData.ammoCapacity;
        reloadActions = weaponData.reloadActions;

        ammoCount = ammoCapacity;

        projectileAnimHandler = GetComponent<ProjectileRenderer>();
        lineRenderer = HexGrid.instance.weaponRangeDisplay;
    }
    public void ShootShip(Ship targetShip)
    {
        
        float distance = HexCoordinates.Distance(ship.pos, targetShip.pos);
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

        ammoCount--;
        GameEvents.instance.UpdateUI();
    }

    public int CalculateDamage(Ship targetShip)
    {
        return damage;
    }

    public float ChanceToHit(Ship target)
    {
        return accuracy; //modifiers, range penalties
    }
    
    public override bool CanFire()
    {
        return ammoCount > 0;
    }

    public void DisplayRange()
    {
        lineRenderer.enabled = true;
        lineRenderer.positionCount = 0;
        HexPatch rangeArea = new HexPatch(ship.pos, idealRange);
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
