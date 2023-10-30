using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "KineticWeaponSO", menuName = "ScriptableObjects/Kinetic")]
public class KineticWeaponSO : ScriptableObject
{
    public int damage = 3;
    public int idealRange = 5;


    public float accuracy = 0.9f;
    public float evasionPenaltyPerCell = 0.02f; //tracking stat

    public int ammoCapacity = 1;
    public int reloadActions = 1;

    public string weaponName = "Kinetic";


    public ProjectileTrail visualProjectilePrefab;
}
