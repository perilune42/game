using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.ComponentModel;

public class UIButtons : MonoBehaviour
{

    public PlayerControl playerControl;
    public Canvas gridCanvas;
    public ControlButton[] buttons;

    [SerializeField] Button confirmButton, fireButton, guardButton, endTurnButton, translateButton;

    public static UIButtons instance;

    private void Awake()
    {
        playerControl = PlayerControl.instance;
        instance = this;
        GameEvents.instance.onUpdateUI += UpdateButtonsAvailability;
        GameEvents.instance.onLockControls += LockPlayerControls;
    }

    public void UpdateButtonsAvailability()
    {
        if (playerControl.currentAction != ControlAction.None && playerControl.currentAction != ControlAction.DirectTargetShip)
        {
            if ((playerControl.currentAction == ControlAction.Rotate && playerControl.pendingDirection == null)
                || (playerControl.currentAction == ControlAction.Translate && playerControl.pendingCell == null))
            {
                ToggleConfirmButton(false);
            }
            else ToggleConfirmButton(true);
        }
        else ToggleConfirmButton(false);
        if (playerControl.currentAction == ControlAction.DirectTargetShip)
        {
            ToggleFireButtons(true);
            if (playerControl.targetedShip != null)
            {
                fireButton.interactable = true;
                guardButton.interactable = false;
            }
            else
            {
                fireButton.interactable = false;
                guardButton.interactable = true;
            }
        }
        else
        {
            fireButton.interactable = false;
            guardButton.interactable = false;
            ToggleFireButtons(false);
        }

        if (playerControl.HasShipsUnderAttack()) endTurnButton.interactable = false;
        else endTurnButton.interactable = true;
    }

    public void Rotate()
    {
        if (playerControl.selectedShip != null && playerControl.ActionAvailable(ControlAction.Rotate))
        playerControl.SetCurrentAction(ControlAction.Rotate);
        
    }
    public void Confirm()
    {
        if ( playerControl.selectedShip != null && playerControl.currentAction != ControlAction.None)
        {
            playerControl.Confirm(); //should change to pass
        }
    }
    public void Pass()
    {
        if (playerControl.selectedShip != null && playerControl.ActionAvailable(ControlAction.Pass))
            playerControl.SetCurrentAction(ControlAction.Pass);

    }
    public void ToggleCoords()
    {
        gridCanvas.enabled = !gridCanvas.enabled;
    }

    public void Boost()
    {
        if (playerControl.selectedShip != null && playerControl.ActionAvailable(ControlAction.Boost))
        {
            playerControl.SetCurrentAction(ControlAction.Boost);
        }
    }

    public void Translate()
    {
        if (playerControl.selectedShip != null && playerControl.ActionAvailable(ControlAction.Translate))
        {
            playerControl.SetCurrentAction(ControlAction.Translate);
        }
    }

    public void Fire()
    {
        if (playerControl.selectedShip != null && playerControl.ActionAvailable(ControlAction.DirectTargetShip))
        {
            playerControl.SetCurrentAction(ControlAction.DirectTargetShip);
        }
    }

    public void EndTurn()
    {
        
        GameEvents.instance.TurnEnd();
    }

    public void SelectWeapon(Weapon weapon)
    {
        if (playerControl.selectedShip != null && playerControl.ActionAvailable(ControlAction.DirectTargetShip))
        {
            playerControl.SetCurrentAction(ControlAction.DirectTargetShip, false, weapon);
            
        }
    }

    public void ToggleAI(TMP_Dropdown dropdown)
    {
        if (dropdown.value == 0) TurnHandler.instance.AIControl = false;
        else TurnHandler.instance.AIControl = true;

    }

    public void ToggleInfiniteActions(TMP_Dropdown dropdown)
    {
        if (dropdown.value == 0) TurnHandler.instance.debugInfiniteActions = false;
        else TurnHandler.instance.debugInfiniteActions = true;

    }

    public void LockPlayerControls(bool toggle)
    {
        foreach (ControlButton controlButton in buttons)
        {
            if (toggle) controlButton.button.interactable = false;
            else controlButton.button.interactable = playerControl.ActionAvailable(controlButton.action);

        }
    }

    public void Evade()
    {
        if (playerControl.selectedShip != null && playerControl.ActionAvailable(ControlAction.Evade))
        {
            playerControl.SetCurrentAction(ControlAction.Evade);
        }
    }

    public void Brace()
    {
        if (playerControl.selectedShip != null && playerControl.ActionAvailable(ControlAction.Brace))
        {
            playerControl.SetCurrentAction(ControlAction.Brace);
        }
    }


    public void _Debug()
    {
        GameEvents.instance._Debug();
    }
    
    public void ToggleConfirmButton(bool toggle)
    {
        confirmButton.gameObject.SetActive(toggle);
    }
    public void ToggleFireButtons(bool toggle)
    {
        fireButton.gameObject.SetActive(toggle);
        guardButton.gameObject.SetActive(toggle);
    }

}
