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
    public Button[] buttons;

    [SerializeField] Button confirmButton, fireButton, guardButton;

    public static UIButtons instance;

    private void Awake()
    {
        playerControl = PlayerControl.instance;
        instance = this;
        GameEvents.instance.onUpdateUI += UpdateButtonsVisibility;
    }

    public void UpdateButtonsVisibility()
    {
        if (playerControl.currentAction != ShipAction.None && playerControl.currentAction != ShipAction.DirectTargetShip)
        {
            if (playerControl.currentAction == ShipAction.Rotate && playerControl.pendingDirection == null) ToggleConfirmButton(false);
            else ToggleConfirmButton(true);
        }
        else ToggleConfirmButton(false);
        if (playerControl.currentAction == ShipAction.DirectTargetShip)
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
    }

    public void Rotate()
    {
        if (playerControl.selectedShip != null && playerControl.selectedShip.ActionAvailable(ShipAction.Rotate))
        playerControl.SetCurrentAction(ShipAction.Rotate);
        
    }
    public void Confirm()
    {
        if ( playerControl.selectedShip != null && playerControl.currentAction != ShipAction.None)
        {
            playerControl.Confirm(); //should change to pass
        }
    }
    public void Pass()
    {
        if (playerControl.selectedShip != null && playerControl.selectedShip.ActionAvailable(ShipAction.Pass))
            playerControl.SetCurrentAction(ShipAction.Pass);

    }
    public void ToggleCoords()
    {
        gridCanvas.enabled = !gridCanvas.enabled;
    }

    public void Boost()
    {
        if (playerControl.selectedShip != null && playerControl.selectedShip.ActionAvailable(ShipAction.Boost))
        {
            playerControl.SetCurrentAction(ShipAction.Boost);
        }
    }

    public void Fire()
    {
        if (playerControl.selectedShip != null && playerControl.selectedShip.ActionAvailable(ShipAction.DirectTargetShip))
        {
            playerControl.SetCurrentAction(ShipAction.DirectTargetShip);
        }
    }

    public void EndTurn()
    {
        GameEvents.instance.TurnEnd();
    }

    public void SelectWeapon(Weapon weapon)
    {
        if (playerControl.selectedShip != null && playerControl.selectedShip.ActionAvailable(ShipAction.DirectTargetShip))
        {
            playerControl.SetCurrentAction(ShipAction.DirectTargetShip, false, weapon);
            
        }
    }

    public void ToggleAI(TMP_Dropdown dropdown)
    {
        if (dropdown.value == 0) TurnHandler.instance.AIControl = false;
        else TurnHandler.instance.AIControl = true;

    }

    public void LockPlayerControls(bool toggle)
    {
        foreach (Button button in buttons)
        {
            button.interactable = !toggle;
        }
    }

    public void Evade()
    {
        if (playerControl.selectedShip != null && playerControl.selectedShip.ActionAvailable(ShipAction.Evade))
        {
            playerControl.SetCurrentAction(ShipAction.Evade);
        }
    }

    public void Debug()
    {
        GameEvents.instance.Debug();
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
