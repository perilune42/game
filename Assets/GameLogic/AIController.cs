using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;


public class AIController : MonoBehaviour
{
    public static AIController instance;
    public ShipList shipList;

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
        foreach (Ship ship in shipList.ships)
        {
            while (ship.ActionAvailable(ShipAction.Pass))
            {
                Pass(ship);
                yield return new WaitForSeconds(2);
            }
            
        }
    }

    void Pass(Ship ship)
    {
        ship.Pass(1);
        GridController.instance.UpdateShipPos(ship);
        GameEvents.instance.UpdateUI();

    }


}
