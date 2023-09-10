using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCell : MonoBehaviour
{
    public int id;
    public HexCoordinates coordinates;
    [SerializeField]
	HexCell[] neighbors;
    HexGrid hexGrid;
    public Color color;
    Color originalColor;
    public Ship containedShip = null;

    public HexCell GetNeighbor (HexDirection direction) {
		return neighbors[(int)direction];
	}
    public void SetNeighbor (HexDirection direction, HexCell cell) {
		neighbors[(int)direction] = cell;
        cell.neighbors[(int)direction.Opposite()] = this;
	}
    // Start is called before the first frame update
    public void ColorSelectShip ()
    {
        color = hexGrid.shipColor;
        originalColor = color;
    }
    public void ColorHover()
    {
        color = hexGrid.hoverColor;
        
    }
    public void ColorDefault()
    {
        color = hexGrid.defaultColor;
        originalColor = color;
    }
    public void ColorHighlight()
    {
        color = hexGrid.highlightColor;
        originalColor = color;
    }
    public void RevertColor()
    {
        color = originalColor;
    }
    private void Awake()
    {
        hexGrid = FindObjectOfType<HexGrid>();
        ColorDefault();
    }
    void Start()
    {
        
    }

    // Update is called once per frame

}
