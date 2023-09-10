using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurnHandler : MonoBehaviour
{

    public Team currentTeam = Team.Enemy;
    Team[] teams = { Team.Player, Team.Enemy };
    public TMP_Text teamLabel;
    PlayerControl playerControl;
    ShipList[] shipLists;
    private void Start()
    {
        playerControl = PlayerControl.Instance;
        shipLists = FindObjectsOfType<ShipList>();
        GameEvents.instance.onTurnEnd += CycleTeam;
        GameEvents.instance.onTurnHandlerInit += Init;
    }

    public void CycleTeam()
    {
        currentTeam = teams[((int)currentTeam + 1) % teams.Length];
        foreach (ShipList list in shipLists) {
            if (list.team == currentTeam) {
                playerControl.shipList = list;
                foreach (Ship ship in list.ships)
                {
                    ship.actions = 4;
                }
            }
            else
            {
                foreach (Ship ship in list.ships)
                {
                    ship.actions = 0;
                }
            }
        }
        playerControl.SwitchShip(0);
        foreach (ShipList list in shipLists)
        {
            list.UpdateCards();
        }
        teamLabel.text = currentTeam.ToString();
    }

    public void Init()
    {
        currentTeam = Team.Player;
        foreach (ShipList list in shipLists)
        {
            if (list.team == currentTeam)
            {
                playerControl.shipList = list;
                foreach (Ship ship in list.ships)
                {
                    ship.actions = 4;
                }
            }
            else
            {
                foreach (Ship ship in list.ships)
                {
                    ship.actions = 0;
                }
            }
        }
        foreach (ShipList list in shipLists)
        {
            list.UpdateCards();
        }
        teamLabel.text = currentTeam.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
