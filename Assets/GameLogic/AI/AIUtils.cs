
using System.Collections.Generic;
using UnityEngine;
public class AIUtils
{
    public static Ship ClosestShip(Ship ship, ShipList shipList, int position = 0)
    //returns cloest ship to parameter ship within shipList
    {
        List<Ship> ships = new List<Ship>(shipList.activeShips);
        int i = position;
        while (true) {
            if (ships.Count == 0) return null;
            Ship closest = ships[0];
            foreach (Ship otherShip in ships)
            {
                if (HexCoordinates.Distance(ship.pos, otherShip.pos)
                    < HexCoordinates.Distance(ship.pos, closest.pos))
                {
                    closest = otherShip;
                }
            }
            if (i <= 0) return closest;
            else ships.Remove(closest);
            i--;
        }
    }

    public static HexCoordinates CenterMass(ShipList shipList)
    {
        HexCoordinates positionSum = new HexCoordinates(0, 0);
        foreach (Ship ship in shipList.activeShips)
        {
            positionSum = positionSum + ship.pos;
        }
        return positionSum * (1f / shipList.activeShips.Count);
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
            if (Mathf.Abs(ship.moveDir - targetDirection) == 1)
            {
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

    public static void Log(string message, Ship ship = null)
    {
        if (ship == null)
        {
            Debug.Log($"{Time.frameCount} {message}");
        }
        else
        {
            Debug.Log($"{Time.frameCount} {ship.shipName} {message}");
        }
    }
}