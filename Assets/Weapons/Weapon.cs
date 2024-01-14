using JetBrains.Annotations;
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
        return CalcMultipleDamage(attack, targetShip, 1);
    }

    public static DamageData CalcMultipleDamage(AttackData attack, Ship targetShip, int count)
    {
        int remainingHealth = targetShip.shipStatus.health;
        int remainingArmor = targetShip.shipStatus.armorPoints;

        for (int i = 0; i < count; i++)
        {
            if (remainingArmor <= 0)
            {
                remainingHealth -= attack.damage;
                continue;
            }
            int finalHealthDamage, blockedDamage;
            if (attack.armorPen >= 0)
            {
                finalHealthDamage = Mathf.Max(0, attack.damage - Mathf.Max(0, targetShip.shipStatus.armorLevel - attack.armorPen));
                blockedDamage = Mathf.Max(0, targetShip.shipStatus.armorLevel - attack.armorPen);
            }
            else
            {
                finalHealthDamage = 0;
                blockedDamage = attack.damage;
            }
            int baseArmorDamage = attack.damage;
            int finalArmorDamage = Mathf.RoundToInt(baseArmorDamage * attack.armorBonus);
            if (finalArmorDamage <= remainingArmor)
            {
                remainingHealth -= finalHealthDamage;
                remainingArmor -= finalArmorDamage;
            }
            else
            {
                
                int overDamage = 0;
                for (int partialDamage = 1; partialDamage <= baseArmorDamage; partialDamage++)
                {
                    finalArmorDamage = Mathf.RoundToInt(partialDamage * attack.armorBonus);
                    if (finalArmorDamage > remainingArmor)
                    {
                        overDamage = baseArmorDamage - partialDamage;
                        break;
                    }
                }
                finalArmorDamage = remainingArmor;
                finalHealthDamage += Mathf.RoundToInt(((float)overDamage / baseArmorDamage) * blockedDamage);
                remainingHealth -= finalHealthDamage;
                remainingArmor -= finalArmorDamage;
            }
        }
        return new DamageData(targetShip.shipStatus.health - remainingHealth, targetShip.shipStatus.armorPoints - remainingArmor);
    }
}
