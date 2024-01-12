using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusEffect
{
    public Ship ship;
    public int duration;
    public bool active = false;
     
    public StatusEffect(int duration) //severity?
    {
        this.duration = duration;
    }

    public void Apply(Ship ship)
    {
        this.ship = ship;
        active = true;
        StatEffect();
    }

    public override string ToString()
    {
        return $"{this.GetType().ToString()}: {duration}";
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
        if (active)
        {
            duration--;
            StatEffect();
            TickEffect();
        }
    }

    public void Merge(StatusEffect other)
    {
        this.duration = Mathf.Max(other.duration, this.duration);
    }

    protected virtual void StatEffect() { } //modify stats of ship starting immediately to end
    protected virtual void TickEffect() { } //ticks every turn starting on turn of control
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
    public StunEffect(int duration): base(duration) 
    {

    }

    protected override void TickEffect()
    {
        ship.ClearActions();
    }
}

public class SlowEffect : StatusEffect
{
    public SlowEffect(int duration) : base(duration)
    {

    }

    protected override void StatEffect()
    {
        ship.shipStatus.thrust.AddMultiplier(0.5f);
        ship.shipStatus.mobility.AddMultiplier(0.5f);
    }

    protected override void TickEffect()
    {

    }
}