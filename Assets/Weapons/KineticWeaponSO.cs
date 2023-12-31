using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "KineticWeaponSO", menuName = "ScriptableObjects/Kinetic")]
public class KineticWeaponSO : ScriptableObject
{
    public int damage = 3;
    public int armorPierce = 1;
    


    public int idealRange = 5;


    public float accuracy = 0.9f;
    public float velocity = 10f;
    public int ammoCapacity = 1;
    public int reloadActions = 1;
    public int projectileCount = 1;

    public string weaponName = "Kinetic";

    public ProjectileTrail visualProjectilePrefab;    
    public float RangePenalty(float distance)
    {
        return distance / (20 * velocity); //10 velocity: 10 cells = 5%, 20 cells = 10%
    }
    public float EvasionPenalty(float distance)
    {
        return distance / (5 * velocity);
    }
}
