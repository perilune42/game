using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TurnHandler : MonoBehaviour
{

    public Team currentTeam = Team.Enemy;
    Team[] teams = { Team.Player, Team.Enemy };
    public TMP_Text teamLabel;
    PlayerControl playerControl;
    ShipList[] shipLists;

    public ShipList playerShipList, enemyShipList;

    public bool AIControl = false;

    public static TurnHandler instance;
    private void Start()
    {
        playerControl = PlayerControl.instance;
        shipLists = FindObjectsOfType<ShipList>();
        foreach  (ShipList list in shipLists)
        {
            if (list.team == Team.Player) playerShipList = list;
            else if (list.team == Team.Enemy) enemyShipList = list;
        }

        instance = this;

        GameEvents.instance.onTurnEnd += CycleTeam;
        GameEvents.instance.onTurnHandlerInit += Init;

        GameEvents.instance.UpdateUI();
    }

    public ShipList ShipListOfTeam(Team team)
    {
        switch (team) {
            case Team.Player:
                return playerShipList;
            case Team.Enemy:
                return enemyShipList;
            default:
                return null;
        }
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
        foreach (ShipList list in shipLists)
        {
            list.UpdateCards();
        }
        teamLabel.text = currentTeam.ToString();

        if (currentTeam == Team.Enemy && AIControl)
        {
            AIController.instance.PlayTurn();
        }
        else if (currentTeam == Team.Player)
        {
            playerControl.CycleShip();
        }
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

    public bool IsAITurn()
    {
        return (currentTeam == Team.Enemy && AIControl);
    }

}
