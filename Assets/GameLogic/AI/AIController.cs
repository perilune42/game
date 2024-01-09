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

    float pauseTime = 1.5f;

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
        yield return new WaitForSeconds(1.5f);
        List<Ship> ships = new List<Ship>(shipList.activeShips);
        foreach (Ship ship in ships) //add randomness
        {
            if (ship.isDestroyed) continue;
            PlayerControl.instance.SwitchShip(ship); 
            GameEvents.instance.CamMoveTo(ship.transform.position);


            yield return new WaitForSeconds(1f);
            
            while (ship.ActionAvailable(ControlAction.Pass))
            {
                ship.AILogic.GetPossibleRoutines();
                Debug.Log(Time.frameCount + " Possible Routines: " + ship.AILogic.possibleRoutines.ToLineSeparatedString());
                IAIRoutine bestRoutine = ship.AILogic.GetBestRoutine();
                Debug.Log(Time.frameCount + " Best Routine: " + bestRoutine);

                if (ship.isDestroyed) break;
                bool finishedAction = false;
                if (ship.AILogic.lastMoved != -1) ship.AILogic.lastMoved++;

                string debugString = "";
                HexDirection targetMoveDirection = HexDirection.None;
                float deviationAngle = 0, maxDeviation = 45;
                int targetSpeedLevel = 2;

                if (bestRoutine is IAIMoveRoutine bestMoveRoutine)
                {

                    if (bestMoveRoutine is AIIntercept interceptRoutine)
                    {
                        debugString += " " + interceptRoutine.targetShip.shipName;
                        debugString += " " + AIUtils.ClosestDirection(ship.pos, interceptRoutine.targetShip.pos);
                        targetMoveDirection = AIUtils.ClosestDirection(ship.pos, interceptRoutine.targetShip.pos);
                        if (HexCoordinates.Distance(ship.pos, interceptRoutine.targetShip.pos) > ship.shipStatus.thrust.Get() * 5)
                        {
                            targetSpeedLevel = 3;
                        }
                        else targetSpeedLevel = 2;


                        deviationAngle = AIUtils.DeviationAngle(ship.moveDir, ship.pos, interceptRoutine.targetShip.pos);

                        //debugString += " Deviation: " + deviationAngle;

                    }
                    else if (bestMoveRoutine is AIAdvance advanceRoutine)
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
                                yield return new WaitForSeconds(pauseTime);
                                continue;
                            }
                            else
                            {
                                StartCoroutine(Boost(ship));
                                ship.AILogic.lastMoved = 0;
                                yield return new WaitForSeconds(pauseTime);
                                continue;
                            }
                        }
                        else if (ship.GetSpeedLevel() != targetSpeedLevel)
                        {
                            StartCoroutine(Boost(ship));
                            ship.AILogic.lastMoved = 0;
                            yield return new WaitForSeconds(pauseTime);
                            continue;
                        }
                    }
                    //above: if moved, loop broken
                    //reached here: no movement necessary
                    bestRoutine = ship.AILogic.GetBestRoutineNot<IAIMoveRoutine>();
                }





                if (bestRoutine is AIEvade)
                {
                    StartCoroutine(Evade(ship));
                    yield return new WaitForSeconds(1.5f);
                    finishedAction = true;
                    continue;
                }

                //AttackRoutines
                else if (bestRoutine is AIAttackShip attackShipRoutine)
                {
                    foreach (Weapon weapon in ship.weapons)
                    {
                        if (weapon.CanFire() && weapon is ITargetsShip t)
                        {
                            StartCoroutine(ShootShip(ship, attackShipRoutine.targetShip, t));
                            yield return new WaitForSeconds(pauseTime);
                            finishedAction = true;
                            break;
                        }
                    }
                    if (finishedAction) continue;

                }



                

                if (!finishedAction)
                {
                    Debug.Log(debugString);
                    StartCoroutine(Pass(ship, true));
                    yield return new WaitForSeconds(pauseTime);
                }
            }
            
        }
    }

    IEnumerator Pass(Ship ship, bool logging = false)
    {
        if (logging) AIUtils.Log("Passing!", ship);
        ship.PassAction(1);
        GameEvents.instance.UpdateUI();
        yield return null;
    }

    IEnumerator Rotate(Ship ship, HexDirection direction)
    {
        AIUtils.Log("Rotating!", ship);
        ship.Rotate(direction);
        GameEvents.instance.UpdateUI();
        yield return null;
    }

    IEnumerator Boost(Ship ship)
    {
        AIUtils.Log("Boosting!", ship);
        ship.Boost(ship.shipStatus.thrust.Get());
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(Pass(ship));
    }
    
   IEnumerator ShootShip(Ship ship, Ship targetShip,ITargetsShip weapon)
    {
        AIUtils.Log("Shooting!", ship);
        weapon.ShootShip(targetShip);
        yield return new WaitForSeconds(1f);
        StartCoroutine(Pass(ship));
    }

    IEnumerator Evade(Ship ship)
    {
        AIUtils.Log("Evading!", ship);
        ship.Evade();
        yield return new WaitForSeconds(0.5f);
    }


    //Movement handler
}
