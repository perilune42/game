using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static GameEvents instance;
    [SerializeField]
    HexGrid hexGrid;

    private void Awake()
    {
        instance = this; 
    }

    public event Action onDebug;
    public void _Debug()
    {
        if (onDebug != null) onDebug();
    }


    public event Action onTurnEnd;
    public void TurnEnd()
    {
        if(onTurnEnd != null) onTurnEnd();
    }

    public event Action onTurnHandlerInit;
    public void TurnHandlerInit()
    {
        if(onTurnHandlerInit != null) onTurnHandlerInit();
    }

    public event Action<Vector3> onCamMoveTo; 
    public void CamMoveTo(Vector3 position)
    {
        if(onCamMoveTo != null) onCamMoveTo(position);
    }

    public event Action<Transform> onCamTrack;
    public void CamTrack(Transform transform)
    {
        if (onCamTrack != null) onCamTrack(transform);
    }

    public event Action onRecolorMesh;
    public void RecolorMesh()
    {

        if(onRecolorMesh != null) onRecolorMesh();
    }

    public event Action<Ship> onDisplayWeapons;
    public void DisplayWeapons(Ship ship)
    {
        if (onDisplayWeapons != null) onDisplayWeapons(ship);
    }

    public event Action onUpdateUI;
    public void UpdateUI()
    {
        //GridController.instance.SetDebugCell(hexGrid.GetCellAtPos
        //(AIUtils.CenterMass(TurnHandler.instance.enemyShipList)));
        if (onUpdateUI != null) onUpdateUI();
    }

    public event Action<bool> onLockControls;
    public void LockControls(bool toggle)
    {
        if (onLockControls != null) onLockControls(toggle);
    }

    public event Action<string> onDisplayPopup;
    public void DisplayPopup(string message)
    {
        if(onDisplayPopup != null) onDisplayPopup(message);
    }

    public event Action<bool> onFreezeProjDisplay;
    public void FreezeProjDisplay(bool freeze)
    {
        if (onFreezeProjDisplay != null) onFreezeProjDisplay(freeze);
    }

    public event Action<IProjectile, bool> onUpdateProjDisplay;
    public void UpdateProjDisplay(IProjectile projectile, bool hit)
    {
        if (onUpdateProjDisplay != null) onUpdateProjDisplay(projectile, hit);
    }

    public event Action<Ship, DamageData> onPreviewDamage;
    public void PreviewDamage(Ship ship, DamageData damage)
    {
        if (onPreviewDamage != null) onPreviewDamage(ship, damage);
    }

    public event Action<Ship, HitType, int> onHitShip;
    public void HitShip(Ship ship, HitType hitType, int damage)
    {
        if (onHitShip != null) onHitShip(ship, hitType, damage);  
    }

    public event Action onShipDestroyed;
    public void ShipDestroyed()
    {
        if (onShipDestroyed != null)
        {
            onShipDestroyed();
            UpdateUI();
            Debug.Log("destroyed ship");
        }
    }

    public event Action onUseCommandPoint;
    public void UseCommandPoint()
    {
        if (onUseCommandPoint != null) onUseCommandPoint();
    }
}