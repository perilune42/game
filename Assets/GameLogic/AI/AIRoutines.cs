using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public enum AIBehaviorType
{
    Interceptor, Frontline
}

public interface IAIRoutine
{

}
public interface IAIMoveRoutine : IAIRoutine //high level, e.g. pursue x ship
{

}

public interface IAIAttackRoutine : IAIRoutine
{

}

public class AIPass : IAIRoutine
{
    public AIPass() { }
}

public class AIIntercept : IAIMoveRoutine //intercept target ship at cruising speed
{
    public Ship targetShip;
    public AIIntercept(Ship targetShip)
    {
        this.targetShip = targetShip;
    }
}

public class AIAdvance : IAIMoveRoutine //go to dest at cruising speed
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


public class AIAttackShip : IAIAttackRoutine
{
    public Ship targetShip;
    public Weapon weapon;
    public AIAttackShip(Ship targetShip, Weapon weapon)
    {
        this.targetShip = targetShip;
        this.weapon = weapon;
    }

    public override string ToString()
    {
        string str = "AIAttackShip";
        str += $" attack {targetShip} using {weapon}";
        return str;
    }
}

//AttackRoutine
//focus fire: use all attacks on 1 ship
//attack closest: attacks closest/highest chance to hit
//defense: attack ships threatening friendlies
//brace: do not attack, defensive actions

//score rank all available routines
//add randomness

public class AIFollow : IAIMoveRoutine
{
    public Ship targetShip;
    public AIFollow(Ship targetShip)
    {
        this.targetShip = targetShip;
    }
}

public class AIEvade : IAIRoutine
{
    public AIEvade()
    {

    }
}