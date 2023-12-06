using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{

    //public abstract void ShootShip(Ship ship);
    public string weaponName;
    public Ship ship;
    public TargetType type;
    
    public abstract void Init();
    public abstract bool CanFire();
    public abstract void PassTurn();
    private void Start()
    {
        Init();
        ship = GetComponentInParent<Ship>();
    }
    
    public static DamageData CalcBasicDamage(AttackData attack, Ship targetShip)
    {
        
        if (targetShip.shipStatus.armorPoints <= 0)
        {
            return new DamageData(attack.damage, 0);
        }
        int finalHealthDamage;
        if (attack.armorPierce >= 0)
        {
            finalHealthDamage = Mathf.Max(0, attack.damage - Mathf.Max(0, targetShip.shipStatus.armorLevel - attack.armorPierce));
        }
        else
        {
            finalHealthDamage = 0;
        }
        int baseArmorDamage = attack.damage;
        int finalArmorDamage = Mathf.RoundToInt(baseArmorDamage * attack.armorBonus);
        if (finalArmorDamage <= targetShip.shipStatus.armorPoints) return new DamageData(finalHealthDamage, finalArmorDamage);
        else
        {
            int overDamage = 0;
            for (int partialDamage = 1; partialDamage <= baseArmorDamage; partialDamage++)
            {
                finalArmorDamage = Mathf.RoundToInt(partialDamage * attack.armorBonus);
                if (finalArmorDamage > targetShip.shipStatus.armorPoints)
                {
                    overDamage = baseArmorDamage - partialDamage;
                }
            }
            finalArmorDamage = targetShip.shipStatus.armorPoints;
            finalHealthDamage += overDamage;
            return new DamageData(finalHealthDamage, finalArmorDamage);
        }
    }
}
