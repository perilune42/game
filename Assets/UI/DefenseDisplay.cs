using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseDisplay : MonoBehaviour
{
    [SerializeField] Canvas defenseOptionsCanvas;

    private void Start()
    {
        GameEvents.instance.onUpdateUI += InitList;
    }

    void InitList()
    {

        Ship ship = PlayerControl.instance.selectedShip;

        if (ship.shipStatus.IsUnderAttack())
        {
            gameObject.SetActive(true);
        }
        else gameObject.SetActive(false);

    }
    
}
