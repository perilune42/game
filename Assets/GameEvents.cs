using System;
using System.Collections;
using System.Collections.Generic;
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

    public event Action onRecolorMesh;
    public void RecolorMesh()
    {
        //hexGrid.hexMesh.CheckNull();
        if(onRecolorMesh != null) onRecolorMesh();
    }

    public event Action<Ship> onDisplayWeapons;
    public void DisplayWeapons(Ship ship)
    {
        if (onDisplayWeapons != null) onDisplayWeapons(ship);
    }

    public event Action onUpdateShipCards;
    public void UpdateShipCards()
    {
        if(onUpdateShipCards != null) onUpdateShipCards();
    }


}
