using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    HexCell activeCell, debugCell;
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

    public void SetDebugCell(HexCell cell)
    {
        if (debugCell != null) debugCell.isDebug = false;
        debugCell = cell;
        debugCell.isDebug = true;
        GameEvents.instance.RecolorMesh();
    }

    public void UpdateShipPos(Ship ship)
    {
        ship.TriggerMoveAnim();
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
