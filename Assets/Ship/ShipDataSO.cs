using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShipDataSO", menuName = "ScriptableObjects/ShipData")]
public class ShipDataSO : ScriptableObject
{
    public int thrust;
    public float mobility = 1;
    public int health;
    public int armorPoints;
    public int armorLevel;
    public int defaultMaxActions = 2;
}
