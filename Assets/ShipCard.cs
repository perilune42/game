using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShipCard : MonoBehaviour
{
    public TMP_Text shipNameLabel;
    public TMP_Text actionsLabel;
    public TMP_Text healthLabel;
    public Ship ship;
    public int vertSpacing = 10;
    public ShipList shipList;
    public Image image;

    public void SetPosition(int pos)
    {
        transform.localPosition = new Vector3(gameObject.GetComponent<RectTransform>().rect.width / 2, -(gameObject.GetComponent<RectTransform>().rect.height + vertSpacing) * pos, 0);
    }

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
        healthLabel.text = ship.shipHealth.health.ToString() + " / " + ship.shipHealth.maxHealth.ToString();
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
