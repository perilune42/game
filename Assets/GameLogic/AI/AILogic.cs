using OpenCover.Framework.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class AILogic : MonoBehaviour
{
    //high level, e.g. target pos, retreat, focus ship
    [HideInInspector] public AIBehaviorType behaviorType; //more params based on ship type
    public AIBehaviorSO AIBehaviorData;
    Ship ship;
    ShipList playerShips;

    public Dictionary<IAIRoutine, float> possibleRoutines = new Dictionary<IAIRoutine, float>();


    public int lastMoved = -1;

    float evasionBias; //score to evade a 5 armor damage attack with 100% accuracy
    float attackBias; //score to attack a ship 5 tiles away
    float interceptBias;
    float recentMovePenalty;

    private void Awake()
    {
        ship = GetComponent<Ship>();
        behaviorType = AIBehaviorData.behaviorType;
        
    }
    private void Start()
    {
        playerShips = TurnHandler.instance.playerShipList;
    }

    public void GetPossibleRoutines()
    {

        possibleRoutines.Clear();
        float score = 0;

        evasionBias = AIBehaviorData.evasionBias;
        attackBias = AIBehaviorData.attackBias;
        interceptBias = AIBehaviorData.interceptBias;
        recentMovePenalty = AIBehaviorData.recentMovePenalty;
        if (ship.shipStatus.IsUnderAttack())
        {
            foreach (KineticProjectile proj in ship.shipStatus.incomingProjectiles)
            {
                score += (proj.damage.armorDamage + proj.damage.healthDamage * 3) * evasionBias / 5f * proj.ChanceToHit();
            }
            possibleRoutines.Add(new AIEvade(), score);
        }

        for (int i = 0; i < 3; i++)
        {
            Ship targetShip = AIUtils.ClosestShip(ship, playerShips, i);
            if (targetShip != null)
            {
                score = attackBias - Mathf.Clamp(Ship.Distance(ship, targetShip) - 5, 0, 100); //factor in weapon accuracy, less emphasis on distance
                possibleRoutines.Add(new AIAttackShip(targetShip), score); // add penalty based on how many threats target already has
            }
        }
        score = interceptBias;
        if (lastMoved != -1 && lastMoved < 3)
        {
            score -= (3 - lastMoved) * recentMovePenalty;
        }
        possibleRoutines.Add(new AIIntercept(AIUtils.ClosestShip(ship, playerShips)), score);

    }

    public IAIRoutine GetBestRoutine()
    {
        GetPossibleRoutines(); //possible optimization: get routines once only
        IAIRoutine bestRoutine = null;
        foreach (IAIRoutine routine in possibleRoutines.Keys)
        {
            if (bestRoutine == null || possibleRoutines[routine] > possibleRoutines[bestRoutine])
            {
                bestRoutine = routine;
            }
        }
        return bestRoutine;
    }

    public RoutineType GetBestRoutine<RoutineType>()
    {
        GetPossibleRoutines(); //possible optimization: get routines once only
        IAIRoutine bestRoutine = null;
        RoutineType bestMatchedRoutine = default;
        foreach (IAIRoutine routine in possibleRoutines.Keys)
        {
            if (possibleRoutines[routine] is RoutineType matchedRoutine)
            {
                if (bestRoutine == null || possibleRoutines[routine] > possibleRoutines[bestRoutine])
                {
                    bestRoutine = routine;
                    bestMatchedRoutine = matchedRoutine;
                }
            }
        }
        return bestMatchedRoutine;
    }

    public IAIRoutine GetBestRoutineNot<RoutineType>()
    {
        GetPossibleRoutines(); //possible optimization: get routines once only
        IAIRoutine bestRoutine = null;
        foreach (IAIRoutine routine in possibleRoutines.Keys)
        {
            if ((bestRoutine == null || possibleRoutines[routine] > possibleRoutines[bestRoutine]) && !(routine is RoutineType))
            {
                bestRoutine = routine;
            }
        }
        return bestRoutine;
    }


    public IAIMoveRoutine GetBestMoveRoutine()
    {
        switch (this.behaviorType) {
            case AIBehaviorType.Interceptor:
                return new AIIntercept(AIUtils.ClosestShip(ship, playerShips));
            case AIBehaviorType.Frontline:
                return new AIAdvance(AIUtils.CenterMass(playerShips));
            default:
                return null;
        }
    }

    public IAIAttackRoutine GetBestAttackRoutine()
    //prob doesn't need a routine, attacks are only really limited to "attack x ship"
    //which can be defined in struct parameters
    {
        switch (this.behaviorType)
        {
            /*
            case AIBehavior.Interceptor:
                return new AIIntercept(AIUtils.ClosestShip(ship, playerShips));
            case AIBehavior.Frontline:
                return new AIAdvance(AIUtils.CenterMass(playerShips));
            */
            default:
                return new AIAttackShip(AIUtils.ClosestShip(ship, playerShips));
        }
    }



}

