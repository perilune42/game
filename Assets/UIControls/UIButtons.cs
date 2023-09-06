using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButtons : MonoBehaviour
{

    public PlayerControl playerControl;
    public Canvas gridCanvas;
    public TurnHandler turnHandler;

    private void Awake()
    {
        playerControl = PlayerControl.Instance;
        turnHandler = FindObjectOfType<TurnHandler>();
    }

    public void Rotate()
    {
        if (playerControl.selectedShip != null && playerControl.selectedShip.ActionAvailable(PlayerAction.Rotate))
        playerControl.SetCurrentAction(PlayerAction.Rotate);
        
    }
    public void Confirm()
    {
        if ( playerControl.selectedShip != null && playerControl.currentAction != PlayerAction.None)
        {
            playerControl.Confirm(); //should change to pass
        }
    }
    public void Pass()
    {
        if (playerControl.selectedShip != null && playerControl.selectedShip.ActionAvailable(PlayerAction.Pass))
            playerControl.SetCurrentAction(PlayerAction.Pass);

    }
    public void ToggleCoords()
    {
        gridCanvas.enabled = !gridCanvas.enabled;
    }

    public void Boost()
    {
        if (playerControl.selectedShip != null && playerControl.selectedShip.ActionAvailable(PlayerAction.Boost))
        {
            playerControl.SetCurrentAction(PlayerAction.Boost);
        }
    }

    public void Fire()
    {
        if (playerControl.selectedShip != null && playerControl.selectedShip.ActionAvailable(PlayerAction.DirectTargetShip))
        {
            playerControl.SetCurrentAction(PlayerAction.DirectTargetShip);
        }
    }

    public void EndTurn()
    {
        turnHandler.CycleTeam();
    }

    public void SelectWeapon(Weapon weapon)
    {
        if (playerControl.selectedShip != null && playerControl.selectedShip.ActionAvailable(PlayerAction.DirectTargetShip))
        {
            playerControl.SetCurrentAction(PlayerAction.DirectTargetShip, false, weapon);
        }
    }

    
}
