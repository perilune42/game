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
                

                if (ship.AILogic.GetBestMoveRoutine() is AIIntercept routine)
                {
                    HexDirection targetMoveDirection = AIUtils.ClosestDirection(ship.pos, routine.targetShip.pos);
                    float deviationAngle = AIUtils.DeviationAngle(ship.moveDir, ship.pos, routine.targetShip.pos);

                    debugString += " Deviation: " + deviationAngle;
                    if (ship.moveDir != targetMoveDirection && deviationAngle >= 45f)
                    {
                        HexDirection rotateManeuverDirection = AIUtils.RotateManeuverBestHeading(ship, targetMoveDirection);
                        if (ship.headingDir != rotateManeuverDirection)
                        {
                            Rotate(ship, rotateManeuverDirection); 
                            yield return new WaitForSeconds(1.5f);
                            continue;
                        }
                        else
                        {
                            StartCoroutine(Boost(ship));
                            yield return new WaitForSeconds(1.5f);
                            continue;
                        }
                    }
                }

                Debug.Log(debugString);
                Pass(ship);
                yield return new WaitForSeconds(1.5f);
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

    IEnumerator Boost(Ship ship)
    {
        ship.Boost(ship.accel);
        yield return new WaitForSeconds(0.5f);
        Pass(ship);
    }
    



    //Movement handler
}
