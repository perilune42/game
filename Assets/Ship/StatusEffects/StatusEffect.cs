using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStatusEffect
{
    public int GetRemainingDuration();
    public void Tick();
    public void Finish();
    public bool IsActive();
}

public class StunEffect : IStatusEffect
{
    public bool active = true;
    Ship ship;
    public int duration;
    public StunEffect(Ship ship, int duration)
    {
        this.ship = ship;
        this.duration = duration;
    }

    
    public int GetRemainingDuration()
    {
        return duration;
    }

    public void Tick()
    {
        if (duration == 0) 
        { 
            Finish();
            return;
        }
        //ship.Brace();
        ship.ClearActions();
        duration--;
    }
    
    public void Finish()
    {
        active = false;
    }

    public bool IsActive()
    {
        return active;
    }
}