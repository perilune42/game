using System.Linq;
using UnityEngine;


[RequireComponent(typeof(ProjectileRenderer))]
public class TorpedoWeapon : Weapon, ITargetsShip, IHasCooldown, ILimitedUse, IHasHitChance, IShootsProjectile
{



    public TorpedoWeaponSO weaponData;
    public int idealRange, remainingUses, reloadActions, projectileCount;
    //public AttackData attack;
    public float accuracy, critOffset;
    public int cooldownTimer;

    ProjectileRenderer projectileAnimHandler;


    public override void Init()
    {

        type = TargetType.Torpedo;
        weaponName = weaponData.weaponName;
        accuracy = weaponData.accuracy;
        remainingUses = weaponData.ammoCapacity;
        projectileCount = weaponData.projectileCount;
        critOffset = weaponData.critOffset;

        cooldownTimer = 0;
        reloadActions = weaponData.reloadActions;

        projectileAnimHandler = GetComponent<ProjectileRenderer>();
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

    public AttackData GetPartialAttack(Ship targetShip = null)
    {
        return new AttackData(weaponData.partialDamage, weaponData.partialArmorPierce);
    }

    public int GetRemainingUses()
    {
        return remainingUses;
    }

    public void ShootShip(Ship targetShip)
    {

        float distance = HexCoordinates.Distance(ship.pos, targetShip.pos);
        for (int i = 0; i < projectileCount; i++)
        {
            GameObject projObject = new GameObject(weaponName + " projectile");
            TorpedoProjectile projectile = projObject.AddComponent<TorpedoProjectile>();
            projObject.transform.parent = targetShip.transform;
            projectile.Init(this, targetShip);
            targetShip.shipStatus.AddProjectile((IProjectile)projectile);

        }
        projectileAnimHandler.Shoot(targetShip.transform.position, weaponData.visualProjectilePrefab, projectileCount);

        remainingUses--;
        cooldownTimer = reloadActions;
        GameEvents.instance.UpdateUI();
    }

    public DamageData GetDamage(Ship targetShip)
    {
        return Weapon.CalcMultipleDamage(GetAttack(), targetShip, projectileCount);
    }

    public DamageData GetSingleDamage(Ship targetShip)
    {
        return Weapon.CalcBasicDamage(GetAttack(), targetShip);
    }

    public DamageData GetPartialDamage(Ship targetShip)
    {
        return Weapon.CalcBasicDamage(GetPartialAttack(), targetShip);
    }

    public float ChanceToHit(Ship target, float distance)
    {
        if (target.shipStatus.isEvading)
            return Mathf.Max(0, (accuracy - weaponData.RangePenalty(distance)) * (1 - weaponData.EvasionPenalty(distance)));
        else return Mathf.Max(0, accuracy - weaponData.RangePenalty(distance));
    }

    public float ChanceToHit(Ship target, float distance, bool isEvading)
    {
        if (isEvading)
            return Mathf.Max(0, (accuracy - weaponData.RangePenalty(distance)) * (1 - weaponData.EvasionPenalty(distance)));
        else return Mathf.Max(0, accuracy - weaponData.RangePenalty(distance));
    }


    public override bool CanFire()
    {
        return cooldownTimer == 0 && remainingUses > 0;
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







}
