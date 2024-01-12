using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum Modifier
{
    Flat, Multiplier
}

public interface IStat
{
    public void ClearModifiers();
}
public class Stat<T> :IStat where T : IComparable
{
    protected T value;
    protected List<T> flatModifiers = new List<T>();
    protected List<float> multipliers = new List<float>();

    public Stat(T value)
    {
        this.value = value;
    }

    public void SetBaseValue(T value)
    {
        this.value = value;
    }

    public void AddFlatModifier(T modifier)
    {
        flatModifiers.Add(modifier);
    }
    public void RemoveFlatModifier(T modifier)
    {
        flatModifiers.Remove(modifier);
    }

    public void AddMultiplier(float multiplier)
    {
        multipliers.Add(multiplier);
    }
    public void RemoveMultiplier(float multiplier)
    {
        multipliers.Remove(multiplier);
    }

    public void ClearModifiers()
    {
        flatModifiers.Clear();
        multipliers.Clear();
    }

}
public class StatInt: Stat<int>
{

    public StatInt(int value) : base(value) { }


    public int Get()
    {
        int finalValue = value;

        float finalMultiplier = 1f;
        foreach (float multiplier in multipliers)
        {
            finalMultiplier += multiplier - 1;
        }

        int finalFlatModifier = 0;

        foreach (int flatModifier in flatModifiers)
        {
            finalFlatModifier += flatModifier;
        }

        return Mathf.RoundToInt((finalValue * finalMultiplier) + finalFlatModifier);
    }


}

public class StatFloat: Stat<float>
{

    public StatFloat(float value): base(value) { }

    public float Get()
    {
        float finalValue = value;

        float finalMultiplier = 1f;
        foreach (float multiplier in multipliers)
        {
            finalMultiplier += multiplier - 1;
        }

        int finalFlatModifier = 0;

        foreach (int flatModifier in flatModifiers)
        {
            finalFlatModifier += flatModifier;
        }

        return (finalValue * finalMultiplier) + finalFlatModifier;
    }


}
