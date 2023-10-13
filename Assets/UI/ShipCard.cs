using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShipCard : MonoBehaviour
{
    public TMP_Text shipNameLabel;
    [SerializeField] TMP_Text actionsLabel;
    [SerializeField] TMP_Text healthLabel;
    public Ship ship;
    public ShipList shipList;
    public Image image;


    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void Init()
    {
        shipNameLabel.text = ship.shipName;
        UpdateLabels();
    }
    public void UpdateLabels()
    {
        actionsLabel.text = ship.actions.ToString() + " / 4";
        healthLabel.text = ship.shipStatus.health.ToString() + " / " + ship.shipStatus.maxHealth.ToString();
    }
    public void SetActive(bool active)
    {
        if (active)
        {
            image.color = shipList.activeColor;
            
        }
        else
        {
            image.color = shipList.defaultColor;
        }
    }

    public void ClickShip()
    {
        PlayerControl.instance.TargetShip(ship);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
