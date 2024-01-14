using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathShower : MonoBehaviour
{
    Ship ship;
    LineRenderer lineRenderer;
    public Material speed1;
    public Material speed2;
    public Material speed3;

    // Start is called before the first frame update
    void Awake()
    {
        GameEvents.instance.onUpdateUI += UpdateColor;
        ship = GetComponentInParent<Ship>();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
    }



    public void Hide()
    {
        lineRenderer.enabled = false;
    }
    public void UnHide()
    {
        lineRenderer.enabled = true;
    }

    public void Show(HexCoordinates nextTile)
    {
        
        lineRenderer.SetPosition(0, ship.transform.position);
        lineRenderer.SetPosition(1, HexGrid.instance.GetCellAtPos(nextTile).transform.position);
        UpdateColor();
    }

    void UpdateColor()
    {
        if (ship.GetSpeedLevel() == 1) lineRenderer.material = speed1;
        else if (ship.GetSpeedLevel() == 2) lineRenderer.material = speed2;
        else lineRenderer.material = speed3;
    }
}
