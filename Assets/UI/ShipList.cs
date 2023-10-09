using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShipList : MonoBehaviour
{
    List<Ship> allShips;
    public List<Ship> ships;
    public List<ShipCard> shipCards;
    public ShipCard shipCardPrefab;
    PlayerControl playerControl;

    public Team team;

    public Color defaultColor = Color.white;
    public Color activeColor = Color.green;
    // Start is called before the first frame update
    void Start()
    { 
        playerControl = PlayerControl.instance;
        allShips = FindObjectsOfType<Ship>().ToList<Ship>();
        int i = 0;
        foreach (Ship ship in allShips)
        {
            if (team == ship.team)
            {
                ship.id = i;
                ships.Add(ship);
                ShipCard card = Instantiate<ShipCard>(shipCardPrefab);
                card.transform.SetParent(transform, false);
                card.ship = ship;
                card.shipList = this;
                UIUtils.List(card.gameObject, i, 10, Vector2.down);
                card.Init();
                shipCards.Add(card);
                

                i++;
            }
        }

        GameEvents.instance.TurnHandlerInit();

        GameEvents.instance.onUpdateUI += UpdateCards;
    }

    public void UpdateCards()
    {
        foreach (ShipCard card in shipCards) {
            card.UpdateLabels();
            card.SetActive(playerControl.selectedShip == card.ship);
        }
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}