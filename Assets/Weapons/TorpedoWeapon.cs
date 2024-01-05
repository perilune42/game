using System.Linq;
using UnityEngine;


[RequireComponent(typeof(ProjectileRenderer))]
public class TorpedoWeapon : Weapon, ITargetsShip, IHasCooldown, ILimitedUse
{



    public TorpedoWeaponSO weaponData;
    public int idealRange, remainingUses, reloadActions, projectileCount;
    //public AttackData attack;
    public float accuracy;
    public int cooldownTimer;

    ProjectileRenderer projectileAnimHandler;


    public override void Init()
    {

        type = TargetType.Torpedo;
        weaponName = weaponData.weaponName;
        accuracy = weaponData.accuracy;
        remainingUses = weaponData.ammoCapacity;
        projectileCount = weaponData.projectileCount;

        cooldownTimer = 0;

        projectileAnimHandler = GetComponent<ProjectileRenderer>();
    }

    public AttackData GetAttack(Ship targetShip)
    {
        return new AttackData(weaponData.damage, weaponData.armorPierce);
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
