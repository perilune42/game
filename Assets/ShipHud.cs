using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShipHud : MonoBehaviour
{
    Ship ship;
    [SerializeField]
    TMP_Text healthLabel;
    [SerializeField]
    TMP_Text actionLabel;

    [SerializeField] Color activeColor, inactiveColor, enemyColor;
    [SerializeField] Image hudBackground;

    private void Start()
    {
        ship = GetComponentInParent<Ship>();
        GameEvents.instance.onUpdateUI += UpdateLabels;
    }

    /*private void Update()
    {
        if (Vector2.Distance(Input.mousePosition, transform.position) < 50)
        {
           hudBackground.enabled = false;
        } 
        else if (!hudBackground.enabled)
        {
            hudBackground.enabled = true;
        }
    }*/
    void UpdateLabels()
    {
        healthLabel.text = ship.shipStatus.health.ToString() + "/" + ship.shipStatus.maxHealth;
        actionLabel.text = ship.actions.ToString();

        if (ship.team == TurnHandler.instance.currentTeam)
        {
            if (ship.isSelected) hudBackground.color = activeColor;
            else hudBackground.color = inactiveColor;
        }
        else { hudBackground.color = enemyColor; }

    }
}
