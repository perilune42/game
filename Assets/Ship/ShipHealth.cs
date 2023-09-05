using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipHealth : MonoBehaviour
{
    public Ship ship;

    public int health = 20;
    public int maxHealth = 20;
    void Awake()
    {
        ship = GetComponent<Ship>();
    }

    public void Damage(int damage)
    {
        health -= damage;
    }
}
