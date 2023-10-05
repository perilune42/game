using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class HexGrid : MonoBehaviour
{
    public int width = 6;
	public int height = 6;
    public TMP_Text cellLabelPrefab;
	public HexCell cellPrefab;
    public Color defaultColor = Color.blue;
    public Color hoverColor = Color.magenta;
	public Color shipColor = Color.green;
	public Color highlightColor = Color.red;
	public bool coordsDisplay = false;


    public Canvas gridCanvas;
	[HideInInspector]
    public HexMesh hexMesh;

	public LineRenderer weaponRangeDisplay;

	HexLines hexLines;

    [HideInInspector]
    public HexCell[] cells;

	public static HexGrid instance;

    void Start () {
		hexMesh.Triangulate();
		hexLines.DrawGrid(this);
		instance = this;
	}

	

	public HexCell GetCellAtPos (HexCoordinates pos) {
		HexCell cell;
		try { cell = cells[pos.ToOffsetCoordinates().x + pos.ToOffsetCoordinates().y * width]; }
		catch (NullReferenceException) {
			return null;
		}
		return cell;
	}
    public HexCell GetCellAtPos(int x, int y)
    {
        return cells[x + y * width];
    }

    void Awake () {
        
		hexMesh = GetComponentInChildren<HexMesh>();
        hexLines = GetComponentInChildren<HexLines>();
        cells = new HexCell[height * width];

		for (int z = 0, i = 0; z < height; z++) {
			for (int x = 0; x < width; x++) {
				CreateCell(x, z, i++);
			
			}
		}
	}
	
	void CreateCell (int x, int z, int i) {
		Vector3 position;
		position.x = x * (HexMetrics.outerRadius * 1.5f);
		position.y = 0f;
		if (x > 0)
		position.z = (z + x * 0.5f - x / 2) * (HexMetrics.innerRadius * 2f);
		else position.z = (z + x * 0.5f - (x-1) / 2) * (HexMetrics.innerRadius * 2f);

		HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
		cell.gameObject.hideFlags = HideFlags.HideInHierarchy;
		cell.transform.SetParent(transform, false);
		cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        cell.color = defaultColor;
        cell.id = i;

        TMP_Text label = Instantiate<TMP_Text>(cellLabelPrefab);
		label.rectTransform.SetParent(gridCanvas.transform, false);
		label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
		label.text = cell.coordinates.ToStringOnSeparateLines();
	}

}
