using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShipDataSO", menuName = "ScriptableObjects/ShipData")]
public class ShipDataSO : ScriptableObject
{
    public int thrust;
    public int health;
    public int armorPoints;
    public int armorLevel;
}
