using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AIBehaviorSO", menuName = "ScriptableObjects/AIBehavior")]
public class AIBehaviorSO : ScriptableObject
{
    public AIBehaviorType behaviorType;
    public float evasionBias = 100; //score to evade a 5 armor damage attack with 100% accuracy
    public float attackBias = 100; //score to attack a ship 5 tiles away
    public float interceptBias = 100;
    public float recentMovePenalty = 40;
    public float rangeBiasModifier = 1f; //positive: prefer close range
}
