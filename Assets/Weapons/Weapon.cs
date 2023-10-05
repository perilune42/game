using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{

    //public abstract void ShootShip(Ship ship);
    public string weaponName;
    public Ship ship;
    public TargetType type;
    
    public abstract void Init();
    private void Start()
    {
        Init();
        ship = GetComponentInParent<Ship>();
    }
    

}
