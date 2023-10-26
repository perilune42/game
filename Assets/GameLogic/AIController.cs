using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;


public class AIController : MonoBehaviour
{
    //Low level: direct actions, e.g. boost, shoot
    public static AIController instance;
    public ShipList shipList;
    public Ship activeShip;

    private void Awake()
    {
        instance = this;
    }
    public void PlayTurn()
    {
        StartCoroutine(DoActions());
    }

    IEnumerator DoActions()
    {
        foreach (Ship ship in shipList.ships)
        {
            GameEvents.instance.CamMoveTo(ship.transform.position);
            while (ship.ActionAvailable(ShipAction.Pass))
            {
                string debugString = "";
                debugString += ship.AILogic.GetBestMoveRoutine();

                if (ship.AILogic.GetBestMoveRoutine() is AIIntercept a)
                {
                    debugString += " " + a.targetShip.shipName;
                    debugString += " " + AIUtils.ClosestDirection(ship.pos, a.targetShip.pos);
                }
                Debug.Log(debugString);

                if (ship.AILogic.GetBestMoveRoutine() is AIIntercept routine)
                {
                    HexDirection targetMoveDirection = AIUtils.ClosestDirection(ship.pos, routine.targetShip.pos);
                    if (ship.moveDir != targetMoveDirection)
                    {
                        HexDirection rotateManeuverDirection = AIUtils.RotateManeuverBestHeading(ship, targetMoveDirection);
                        if (ship.headingDir != rotateManeuverDirection)
                        {
                            Rotate(ship, rotateManeuverDirection); 
                            //zigzagging fix: cooldown before next rotate is allowed?
                            //or implement "close enough" condition for leniency
                            yield return new WaitForSeconds(1);
                            continue;
                        }
                        else
                        {
                            Boost(ship);
                            yield return new WaitForSeconds(1);
                            continue;
                        }
                    }
                }


                Pass(ship);
                yield return new WaitForSeconds(1);
            }
            
        }
    }

    void Pass(Ship ship)
    {
        ship.Pass(1);
        GameEvents.instance.CamTrack(ship.transform);
        GridController.instance.UpdateShipPos(ship);
        GameEvents.instance.UpdateUI();

    }

    void Rotate(Ship ship, HexDirection direction)
    {
        ship.Rotate(direction);
        GameEvents.instance.CamTrack(ship.transform);
        GridController.instance.UpdateShipPos(ship);
        GameEvents.instance.UpdateUI();
    }

    void Boost(Ship ship)
    {
        ship.Boost(ship.accel);
        GameEvents.instance.CamTrack(ship.transform);
        GridController.instance.UpdateShipPos(ship);
        GameEvents.instance.UpdateUI();
    }
    



    //Movement handler
}
