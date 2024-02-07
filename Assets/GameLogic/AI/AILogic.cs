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


    //AI TUNING STATS
    //for changes of a product of two numbers, adjust second multiplier only

    //stats defined in scriptable object: baseline all 100, adjust multipliers here if not appropriate
    const float evasionAdj = 1.2f; //score to evade a 5 armor damage attack with 100% accuracy
    const float attackAdj = 1.5f; //score to attack a ship 5 tiles away
    const float interceptAdj = 1f;
    const float stackedThreatAdj = 1f;
    const float distancePenalty = 1 * 1f;
    const float noPenaltyDistance = 5f;
    const float missChancePenaltyThreshold = 0.5f;
    const float missChancePenalty = 100 * 1f; //score reduction per % to miss over (missChancePenaltyThreshold)%
    const float laserDistancePenalty = 10 * 1f;
    const float laserCloseRangeBonus = 15f;
    const float laserFarPenaltyDistance = 10f;


    private void Awake()
    {
        ship = GetComponent<Ship>();
        behaviorType = AIBehaviorData.behaviorType;
        
    }
    private void Start()
    {
        playerShips = TurnHandler.instance.playerShipList;
    }

    public Dictionary<IAIRoutine, float> GetPossibleRoutines()
    {

        possibleRoutines.Clear();
        float score = 0;

        float evasionBias = AIBehaviorData.evasionBias * evasionAdj;
        float attackBias = AIBehaviorData.attackBias * attackAdj;
        float interceptBias = AIBehaviorData.interceptBias * interceptAdj;
        float recentMovePenalty = AIBehaviorData.recentMovePenalty;
        float rangeBiasModifier = AIBehaviorData.rangeBiasModifier;

        possibleRoutines.Add(new AIPass(), 0);

        if (ship.shipStatus.IsUnderAttack())
        {
            foreach (IProjectile proj in ship.shipStatus.incomingProjectiles)
            {
                score += (proj.GetDamage().armorDamage + proj.GetDamage().healthDamage * 3) * evasionBias / 5f * proj.ChanceToHit();
            }
            possibleRoutines.Add(new AIEvade(), score);
        }

        for (int i = 0; i < 3; i++)
        {
            Ship targetShip = AIUtils.ClosestShip(ship, playerShips, i);
            if (targetShip != null)
            {
                foreach (Weapon weapon in ship.weapons) {
                    if (!weapon.CanFire()) continue;
                    score = attackBias;
                    score -= Mathf.Clamp(Ship.Distance(ship, targetShip) - noPenaltyDistance, 0, 100) * distancePenalty; 
                    //factor in weapon accuracy, less emphasis on distance
                    if (weapon is IHasHitChance weapon1)
                    {
                        float chanceToMiss = 1 - weapon1.ChanceToHit(targetShip, ship.DistanceTo(targetShip), true);
                        score -= Mathf.Clamp01(chanceToMiss - missChancePenaltyThreshold) * missChancePenalty;
                        //reduce score per miss % over 50%
                    }
                    if (weapon is LaserWeapon laser)
                    {
                        //score += laserCloseRangeBonus;
                        score -= Mathf.Clamp(Ship.Distance(ship, targetShip) - laser.fallOffRange, 0, 100) * laserDistancePenalty;
                        //apply extra range penalty for laser weapons
                    }
                    if (targetShip.shipStatus.incomingProjectiles.Count >= 5)
                    {
                        foreach (IProjectile proj in targetShip.shipStatus.incomingProjectiles)
                        {
                            score -= stackedThreatAdj * proj.ChanceToHit() //reduce deathstacking on player
                                * (proj.GetDamage().armorDamage + proj.GetDamage().healthDamage);
                        }
                    }

                    possibleRoutines.Add(new AIAttackShip(targetShip, weapon), score); // add penalty based on how many threats target already has
                }
            }
        }
        score = interceptBias;
        score += ship.DistanceTo(AIUtils.ClosestShip(ship, playerShips)) * rangeBiasModifier;
        if (lastMoved != -1 && lastMoved < 3)
        {
            score -= (3 - lastMoved) * recentMovePenalty;
        }
        if (ship.speed <= 1)
        {
            score *= 2f;
        }
        possibleRoutines.Add(new AIIntercept(AIUtils.ClosestShip(ship, playerShips)), score);

        return possibleRoutines;
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






}

