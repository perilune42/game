using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "KineticWeaponSO", menuName = "ScriptableObjects/Kinetic")]
public class KineticWeaponSO : ScriptableObject
{
    public int damage = 3;
    public int idealRange = 5;
    public float accuracy = 0.9f;
    public string weaponName = "Kinetic";
}
