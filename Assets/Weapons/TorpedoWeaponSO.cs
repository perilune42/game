using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TorpedoWeaponSO", menuName = "ScriptableObjects/Torpedo")]
public class TorpedoWeaponSO : ScriptableObject
{
    public int damage = 3;
    public int armorPierce = 1;

    public int partialDamage = 2;
    public int partialArmorPierce = 0;
    

    public float accuracy = 0.9f;
    public float velocity = 10f;
    public int ammoCapacity = 3;
    public int projectileCount = 1;

    public int reloadActions = 1;

    public string weaponName = "Torpedo";

    public ProjectileTrail visualProjectilePrefab;    
    public float RangePenalty(float distance)
    {
        return 0; //10 velocity: 10 cells = 5%, 20 cells = 10%
    }
    public float EvasionPenalty(float distance)
    {
        return distance / (5 * velocity);
    }
}
