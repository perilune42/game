using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LaserWeaponSO", menuName = "ScriptableObjects/Laser")]
public class LaserWeaponSO : ScriptableObject
{
    public int damage = 1;
    public int fallOffRange = 1;
    public string weaponName = "Laser";
}
