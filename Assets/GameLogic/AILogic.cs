using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public enum AIBehavior
{
    Default
}
public interface IAIMoveRoutine  //high level, e.g. pursue x ship
{
    
}


public struct AIIntercept: IAIMoveRoutine
{
    public Ship targetShip;
    public AIIntercept(Ship targetShip)
    {
        this.targetShip = targetShip;
    }
}

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

    private void Awake()
    {
        ship = GetComponent<Ship>();
    }

    public IAIMoveRoutine GetBestMoveRoutine()
    {
        return new AIIntercept(AIUtils.ClosestShip(ship, TurnHandler.instance.playerShipList));
    }


}

public class AIUtils
{
    public static Ship ClosestShip(Ship ship, ShipList shipList)
    //returns cloest ship to parameter ship within shipList
    {
        if (shipList.ships.Count == 0) return null;
        Ship closest = shipList.ships[0];
        foreach (Ship otherShip in shipList.ships)
        {
            if (HexCoordinates.Distance(ship.pos, otherShip.pos)
                < HexCoordinates.Distance(ship.pos, closest.pos))
            {
                closest = otherShip;
            }
        }
        return closest;
    }

    public static HexDirection ClosestDirection(HexCoordinates origin, HexCoordinates destination)
    //returns closest direction to target pos
    {
        HexCoordinates vector = destination - origin;
        HexDirection closest = HexDirection.N;
        for (int i = 1; i < 6; i++)
        {
            if (Vector2.Dot(((HexDirection)i).ToV2(), vector.ToV2())
                > Vector2.Dot(closest.ToV2(), vector.ToV2()))
            {
                closest = (HexDirection)i;
            }
            //string debug = ((HexDirection)i).ToString() + " " + ((HexDirection)i).To2D().ToString();
            //Debug.Log(debug);
        }
        return closest;
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