using System.Linq;
using UnityEngine;


[RequireComponent(typeof(ProjectileRenderer))]
public class KineticWeapon : Weapon, ITargetsShip, IRanged, IUsesAmmo, IHasCooldown
{
    
    

    public KineticWeaponSO weaponData;
    public int damage, idealRange, ammoCapacity, reloadActions, ammoCount;
    public float accuracy, evasionPenaltyPerCell;
    public int cooldownTimer;

    ProjectileRenderer projectileAnimHandler;
    LineRenderer rangeLineRenderer;



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
        cooldownTimer = 0;

        projectileAnimHandler = GetComponent<ProjectileRenderer>();
        rangeLineRenderer = HexGrid.instance.weaponRangeDisplay;
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
        if (ammoCount == 0)
        {
            cooldownTimer = reloadActions + 1;
        }
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
        return cooldownTimer == 0;
    }

    public override void PassAction()
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
