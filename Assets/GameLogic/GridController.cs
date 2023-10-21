using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    HexCell activeCell;
    public static GridController instance;

    private void Awake()
    {
        instance = this;
    }

    public void SetActiveCell(HexCell cell)
    {
        if (activeCell != null) activeCell.isActive = false;
        activeCell = cell;
        activeCell.isActive = true;
        GameEvents.instance.RecolorMesh();
    }
    public void UpdateShipPos(Ship ship)
    {
        ship.TriggerMoveAnim();
        activeCell.ColorDefault();
        SetActiveCell(HexGrid.instance.GetCellAtPos(ship.pos));
        
    }

    public void UpdateShipPos(Ship ship, float delay)
    {
        StartCoroutine(DelayUpdateShipPos(ship, delay));
    }

    IEnumerator DelayUpdateShipPos(Ship ship, float time)
    {
        yield return new WaitForSeconds(time);
        UpdateShipPos(ship);
    }
}
