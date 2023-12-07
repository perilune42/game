using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public enum AIBehavior
{
    Interceptor, Frontline 
}
public interface IAIMoveRoutine  //high level, e.g. pursue x ship
{
    
}

public interface IAIAttackRoutine
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

public class AIUtils
{
    public static Ship ClosestShip(Ship ship, ShipList shipList)
    //returns cloest ship to parameter ship within shipList
    {
        if (shipList.ships.Count == 0) return null;
        Ship closest = shipList.activeShips[0];
        foreach (Ship otherShip in shipList.activeShips)
        {
            if (HexCoordinates.Distance(ship.pos, otherShip.pos)
                < HexCoordinates.Distance(ship.pos, closest.pos))
            {
                closest = otherShip;
            }
        }
        return closest;
    }

    public static HexCoordinates CenterMass(ShipList shipList)
    {
        HexCoordinates positionSum = new HexCoordinates(0,0);
        foreach (Ship ship in shipList.activeShips)
        {
            positionSum = positionSum + ship.pos;
        }
        return positionSum * (1f/ shipList.ships.Count);
    }

    public static HexDirection ClosestDirection(HexCoordinates origin, HexCoordinates destination)
    //returns closest direction to target pos
    {
        HexCoordinates vector = destination - origin;
        HexDirection closest = HexDirection.N;
        for (int i = 1; i < 6; i++)
        {
            if (Vector2.Dot(((HexDirection)i).ToV2(), vector.ToV2Norm())
                > Vector2.Dot(closest.ToV2(), vector.ToV2Norm()))
            {
                closest = (HexDirection)i;
            }
            //string debug = ((HexDirection)i).ToString() + " " + ((HexDirection)i).To2D().ToString();
            //Debug.Log(debug);
        }
        return closest;
    }

    public static bool WithinDeviationRange(HexDirection direction, HexCoordinates origin,
                                            HexCoordinates destination, float maxAngle)
    //returns whether deviation between moving direction and desired heading is within
    //acceptable maxAngle range
    {
        HexCoordinates vector = destination - origin;
        if (Vector2.Angle(direction.ToV2(), vector.ToV2Norm()) <= maxAngle)
        {
            return true;
        }
        return false;
    }

    public static float DeviationAngle(HexDirection direction, HexCoordinates origin,
                                  HexCoordinates destination)
    {
        HexCoordinates vector = destination - origin;
        return Vector2.Angle(direction.ToV2(), vector.ToV2Norm());
    }

    public static HexDirection RotateManeuverBestHeading(Ship ship, HexDirection targetDirection)
    //best way to boost to start rotate to desired direction
    {
        if (ship.GetSpeedLevel() == 1)
        {
            return targetDirection;
        }
        if (ship.GetSpeedLevel() == 2)
        {
            if (Mathf.Abs(ship.moveDir - targetDirection) == 1) {
                return targetDirection - (ship.moveDir - targetDirection) % 6;
            }
            else if (Mathf.Abs(ship.moveDir - targetDirection) == 2)
            {
                return targetDirection;
            }
            else { return targetDirection; }

        }
        else
        {
            return ship.moveDir.Opposite();
        }

    }

}