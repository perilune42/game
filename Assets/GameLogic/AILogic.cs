using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public enum AIBehavior
{
    Interceptor, Frontline 
}

public interface IAIAction
{

}
public interface IAIMoveRoutine : IAIAction //high level, e.g. pursue x ship
{
    
}

public interface IAIAttackRoutine : IAIAction
{

}


public class AIIntercept: IAIMoveRoutine //intercept target ship at cruising speed
{
    public Ship targetShip;
    public AIIntercept(Ship targetShip)
    {
        this.targetShip = targetShip;
    }
}

public class AIAdvance: IAIMoveRoutine //go to dest at cruising speed
{
    public HexCoordinates destination;
    public AIAdvance(HexCoordinates destination)
    {
        this.destination = destination;
    }
} 

//fast pursuit: if deviation < 15, and moving right dir, boost to high speed
//retreat: drift until far enough away from target/for x actions
//charge: move at speed 2 towards center of all enemy ships, update every 2 turns
//advance: move at speed 1 towards center of all enemy ships, update every 2 turns
//guard: stay with friendly ships that are advancing/charging
//flank: advance, but +- 1 direction


public class AIAttackShip: IAIAttackRoutine
{
    public Ship targetShip;
    public AIAttackShip(Ship targetShip)
    {
        this.targetShip = targetShip;
    }
}

//AttackRoutine
//focus fire: use all attacks on 1 ship
//attack closest: attacks closest/highest chance to hit
//defense: attack ships threatening friendlies
//brace: do not attack, defensive actions

//score rank all available routines
//add randomness

public class AIFollow: IAIMoveRoutine
{
    public Ship targetShip;
    public AIFollow(Ship targetShip)
    {
        this.targetShip = targetShip;
    }
}

public class AIEvade: IAIAction
{
    public AIEvade()
    {

    }
}

public class AILogic : MonoBehaviour
{
    //high level, e.g. target pos, retreat, focus ship
    public AIBehavior behavior; //more params based on ship type
    Ship ship;
    ShipList playerShips;

    public Dictionary<IAIAction, float> possibleActions = new Dictionary<IAIAction, float>();


    public int lastMoved = -1;

    float AIEvasionBias; //score to evade a 5 armor damage attack with 100% accuracy
    float AIAttackBias; //score to attack a ship 5 tiles away
    float AIInterceptBias;
    float AIRecentMovePenalty;

    private void Awake()
    {
        ship = GetComponent<Ship>();
        
    }
    private void Start()
    {
        playerShips = TurnHandler.instance.playerShipList;
    }

    public void GetPossibleActions()
    {

        possibleActions.Clear();
        float score = 0;
        switch (this.behavior)
        {

            case AIBehavior.Interceptor:
                AIEvasionBias = 100; //score to evade a 5 armor damage attack with 100% accuracy
                AIAttackBias = 110; //score to attack a ship 5 tiles away
                AIInterceptBias = 100;
                AIRecentMovePenalty = 40;
                if (ship.shipStatus.IsUnderAttack())
                {
                    foreach (KineticProjectile proj in ship.shipStatus.incomingProjectiles)
                    {
                        score += (proj.damage.armorDamage + proj.damage.healthDamage * 3) * AIEvasionBias / 5f * proj.ChanceToHit();
                    }
                    possibleActions.Add(new AIEvade(), score);
                }

                for (int i = 0; i < 3; i++)
                {
                    Ship targetShip = AIUtils.ClosestShip(ship, playerShips, i);
                    if (targetShip != null)
                    {
                        score = AIAttackBias - Mathf.Clamp(Ship.Distance(ship, targetShip) - 5, 0, 100); //factor in weapon accuracy, less emphasis on distance
                        possibleActions.Add(new AIAttackShip(targetShip), score); // add penalty based on how many threats target already has
                    }
                }
                score = AIInterceptBias;
                if (lastMoved != -1 && lastMoved < 3)
                {
                    score -= (3 - lastMoved) * AIRecentMovePenalty;
                }
                possibleActions.Add(new AIIntercept(AIUtils.ClosestShip(ship, playerShips)), score);

                break;

            case AIBehavior.Frontline:
                AIEvasionBias = 100; //score to evade a 5 armor damage attack with 100% accuracy
                AIAttackBias = 110; //score to attack a ship 5 tiles away
                AIInterceptBias = 100;
                AIRecentMovePenalty = 40;
                if (ship.shipStatus.IsUnderAttack())
                {
                    foreach (KineticProjectile proj in ship.shipStatus.incomingProjectiles)
                    {
                        score += (proj.damage.armorDamage + proj.damage.healthDamage * 3) * AIEvasionBias / 5f * proj.ChanceToHit();
                    }
                    possibleActions.Add(new AIEvade(), score);
                }

                for (int i = 0; i < 3; i++)
                {
                    Ship targetShip = AIUtils.ClosestShip(ship, playerShips, i);
                    if (targetShip != null)
                    {
                        score = AIAttackBias - Mathf.Clamp(Ship.Distance(ship, targetShip) - 5, 0, 100);
                        possibleActions.Add(new AIAttackShip(targetShip), score);
                    }
                }
                score = AIInterceptBias;
                if (lastMoved != -1 && lastMoved < 3)
                {
                    score -= (3 - lastMoved) * AIRecentMovePenalty;
                }
                possibleActions.Add(new AIIntercept(AIUtils.ClosestShip(ship, playerShips)), score);

                break;
                
        }
    }

    public IAIMoveRoutine GetBestMoveRoutine()
    {
        switch (this.behavior) {
            case AIBehavior.Interceptor:
                return new AIIntercept(AIUtils.ClosestShip(ship, playerShips));
            case AIBehavior.Frontline:
                return new AIAdvance(AIUtils.CenterMass(playerShips));
            default:
                return null;
        }
    }

    public IAIAttackRoutine GetBestAttackRoutine()
    //prob doesn't need a routine, attacks are only really limited to "attack x ship"
    //which can be defined in struct parameters
    {
        switch (this.behavior)
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

