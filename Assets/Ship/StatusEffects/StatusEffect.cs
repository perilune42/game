using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusEffect
{
    public Ship ship;
    public int duration;
    public bool active;

    public StatusEffect(Ship ship, int duration)
    {
        this.ship = ship;
        this.duration = duration;
        active = true;
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
        duration--;
        TickEffect();
    }
    protected virtual void TickEffect() { }
    public virtual void Finish()
    {
        active = false;
    }
    public virtual bool IsActive()
    {
        return active;
    }
}

public class StunEffect : StatusEffect
{
    public StunEffect(Ship ship, int duration): base(ship, duration) 
    {

    }

    protected override void TickEffect()
    {

        ship.ClearActions();
    }
}

public class SlowEffect : StatusEffect
{
    public SlowEffect(Ship ship, int duration) : base(ship, duration)
    {

    }


    protected override void TickEffect()
    {
        ship.shipStatus.thrust.AddMultiplier(0.5f); //evasion strength
    }
}