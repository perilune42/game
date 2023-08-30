using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionPreview : MonoBehaviour
{
    HexGrid hexGrid;
    Canvas canvas;
    private void Awake()
    {
        hexGrid = GetComponentInParent<HexGrid>();
        canvas = GetComponent<Canvas>();
    }

    public void PreviewAt(HexCoordinates pos, HexDirection direction)
    {
        canvas.enabled = true;
        transform.position = hexGrid.GetCellAtPos(pos).transform.position;
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, 60 * (int)direction, transform.rotation.eulerAngles.z);
    }

    public void Hide()
    {
        canvas.enabled = false;
    }
}
