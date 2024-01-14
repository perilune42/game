using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LaserWeaponSO", menuName = "ScriptableObjects/Laser")]
public class LaserWeaponSO : ScriptableObject
{
    public int damage = 1;
    public int armorPen = -1;
    public float armorBonus = 1f;
    public int fallOffRange = 1;
    public float fallOffRate = 0.1f;

    public int reloadActions = 1;
    public float critOffset = 0f;

    public string weaponName = "Laser";
    public ProjectileTrail visualProjectilePrefab;
}
