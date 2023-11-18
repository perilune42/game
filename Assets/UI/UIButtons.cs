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
        GameEvents.instance.onLockControls += LockPlayerControls;
    }

    public void UpdateButtonsVisibility()
    {
        if (playerControl.currentAction != ControlAction.None && playerControl.currentAction != ControlAction.DirectTargetShip)
        {
            if (playerControl.currentAction == ControlAction.Rotate && playerControl.pendingDirection == null) ToggleConfirmButton(false);
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
    }

    public void Rotate()
    {
        if (playerControl.selectedShip != null && playerControl.selectedShip.ActionAvailable(ControlAction.Rotate))
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
        if (playerControl.selectedShip != null && playerControl.selectedShip.ActionAvailable(ControlAction.Pass))
            playerControl.SetCurrentAction(ControlAction.Pass);

    }
    public void ToggleCoords()
    {
        gridCanvas.enabled = !gridCanvas.enabled;
    }

    public void Boost()
    {
        if (playerControl.selectedShip != null && playerControl.selectedShip.ActionAvailable(ControlAction.Boost))
        {
            playerControl.SetCurrentAction(ControlAction.Boost);
        }
    }

    public void Fire()
    {
        if (playerControl.selectedShip != null && playerControl.selectedShip.ActionAvailable(ControlAction.DirectTargetShip))
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
        if (playerControl.selectedShip != null && playerControl.selectedShip.ActionAvailable(ControlAction.DirectTargetShip))
        {
            playerControl.SetCurrentAction(ControlAction.DirectTargetShip, false, weapon);
            
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
        if (playerControl.selectedShip != null && playerControl.selectedShip.ActionAvailable(ControlAction.Evade))
        {
            playerControl.SetCurrentAction(ControlAction.Evade);
        }
    }

    public void Debug()
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
