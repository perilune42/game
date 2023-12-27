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


public struct AIIntercept: IAIMoveRoutine //intercept target ship at cruising speed
{
    public Ship targetShip;
    public AIIntercept(Ship targetShip)
    {
        this.targetShip = targetShip;
    }
}

public struct AIAdvance: IAIMoveRoutine //go to dest at cruising speed
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


public struct AIAttackShip: IAIAttackRoutine
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

public struct AIFollow: IAIMoveRoutine
{
    public Ship targetShip;
    public AIFollow(Ship targetShip)
    {
        this.targetShip = targetShip;
    }
}

public class AILogic : MonoBehaviour
{
    //high level, e.g. target pos, retreat, focus ship
    public AIBehavior behavior; //more params based on ship type
    Ship ship;
    ShipList playerShips;

    Dictionary<IAIAction, int> possibleActions = new Dictionary<IAIAction, int>();


    public int lastMoved = -1;

    private void Awake()
    {
        ship = GetComponent<Ship>();
        
    }
    private void Start()
    {
        playerShips = TurnHandler.instance.playerShipList;
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

