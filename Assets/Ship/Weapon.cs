using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int damage = 1;
    public WeaponType type = WeaponType.Direct;
    public string weaponName = "weapon";

    public void Shoot(Ship ship)
    {
        ship.shipHealth.Damage(damage);
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
