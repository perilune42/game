using System;
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
        foreach (Ship ship in shipList.ships) //add randomness
        {
            PlayerControl.instance.SwitchShip(ship); 
            GameEvents.instance.CamMoveTo(ship.transform.position);
            yield return new WaitForSeconds(1f);
            while (ship.ActionAvailable(ControlAction.Pass))
            {
                if (ship.isDestroyed) break;
                bool finishedAction = false;
                if (ship.AILogic.lastMoved != -1) ship.AILogic.lastMoved++;

                string debugString = "";
                debugString += ship.AILogic.GetBestMoveRoutine();
                HexDirection targetMoveDirection = HexDirection.None;
                float deviationAngle = 0, maxDeviation = 45;
                int targetSpeedLevel = 2;

                if (ship.AILogic.GetBestMoveRoutine() is AIIntercept interceptRoutine)
                {
                    debugString += " " + interceptRoutine.targetShip.shipName;
                    debugString += " " + AIUtils.ClosestDirection(ship.pos, interceptRoutine.targetShip.pos);
                    targetMoveDirection = AIUtils.ClosestDirection(ship.pos, interceptRoutine.targetShip.pos);
                    if (HexCoordinates.Distance(ship.pos, interceptRoutine.targetShip.pos) > ship.accel * 5)
                    {
                        targetSpeedLevel = 3;
                    }
                    else targetSpeedLevel = 2;


                    deviationAngle = AIUtils.DeviationAngle(ship.moveDir, ship.pos, interceptRoutine.targetShip.pos);

                    //debugString += " Deviation: " + deviationAngle;

                }
                else if (ship.AILogic.GetBestMoveRoutine() is AIAdvance advanceRoutine)
                {
                    targetMoveDirection = AIUtils.ClosestDirection(ship.pos, advanceRoutine.destination);
                    targetSpeedLevel = 2;
                    deviationAngle = AIUtils.DeviationAngle(ship.moveDir, ship.pos, advanceRoutine.destination);

                    //debugString += " Deviation: " + deviationAngle;
                }

                //perform movement if any

                if (ship.AILogic.lastMoved == -1 || ship.AILogic.lastMoved > 3) //move cooldown, placeholder
                    //to be replaced with score logic
                {
                    if (ship.moveDir != targetMoveDirection && deviationAngle >= maxDeviation)
                    {
                        HexDirection rotateManeuverDirection = AIUtils.RotateManeuverBestHeading(ship, targetMoveDirection);
                        if (ship.headingDir != rotateManeuverDirection)
                        {
                            StartCoroutine(Rotate(ship, rotateManeuverDirection));
                            yield return new WaitForSeconds(1.5f);
                            continue;
                        }
                        else
                        {
                            StartCoroutine(Boost(ship));
                            ship.AILogic.lastMoved = 0;
                            yield return new WaitForSeconds(1.5f);
                            continue;
                        }
                    }
                    else if (ship.GetSpeedLevel() != targetSpeedLevel)
                    {
                        StartCoroutine(Boost(ship));
                        ship.AILogic.lastMoved = 0;
                        yield return new WaitForSeconds(1.5f);
                        continue;
                    }
                }

                //AttackRoutines
                if (ship.AILogic.GetBestAttackRoutine() is AIAttackShip attackShipRoutine)
                {
                    foreach (Weapon weapon in ship.weapons)
                    {
                        if (weapon.CanFire() && weapon is ITargetsShip t)
                        {
                            StartCoroutine(ShootShip(ship, attackShipRoutine.targetShip, t));
                            yield return new WaitForSeconds(1.5f);
                            finishedAction = true;
                            break;
                        }
                    }
                    if (finishedAction) continue;

                }
                

                if (!finishedAction) { 
                    Debug.Log(debugString);
                    StartCoroutine(Pass(ship));
                    yield return new WaitForSeconds(1.5f);
                }

            }
            
        }
    }

    IEnumerator Pass(Ship ship)
    {
        ship.Pass(1);
        GameEvents.instance.CamTrack(ship.transform);
        GridController.instance.UpdateShipPos(ship);
        GameEvents.instance.UpdateUI();
        yield return null;
    }

    IEnumerator Rotate(Ship ship, HexDirection direction)
    {
        ship.Rotate(direction);
        GameEvents.instance.CamTrack(ship.transform);
        GridController.instance.UpdateShipPos(ship);
        GameEvents.instance.UpdateUI();
        yield return null;
    }

    IEnumerator Boost(Ship ship)
    {
        ship.Boost(ship.accel);
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(Pass(ship));
    }
    
   IEnumerator ShootShip(Ship ship, Ship targetShip,ITargetsShip weapon)
    {
        weapon.ShootShip(targetShip);
        yield return new WaitForSeconds(1f);
        StartCoroutine(Pass(ship));
    }


    //Movement handler
}
