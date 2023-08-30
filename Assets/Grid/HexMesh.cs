using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour {

	Mesh hexMesh;
	HexGrid hexGrid;
	List<Vector3> vertices;
	List<int> triangles;
    MeshCollider meshCollider;
	public float vertOffset = -0.6f;
	List<Color> colors;
	public PlayerControl playerControl;

    void Awake () {
		GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
        meshCollider = gameObject.AddComponent<MeshCollider>();
        hexMesh.name = "Hex Mesh";
		vertices = new List<Vector3>();
        colors = new List<Color>();
        triangles = new List<int>();
		hexGrid = GetComponentInParent<HexGrid>();
		hexMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; 
	}

    public void Triangulate () {
		HexCell[] cells = hexGrid.cells;
		hexMesh.Clear();
		vertices.Clear();
        colors.Clear();
        triangles.Clear();
		for (int i = 0; i < cells.Length; i++) {
			Triangulate(cells[i]);
		}
		hexMesh.vertices = vertices.ToArray();
        hexMesh.colors = colors.ToArray();
        hexMesh.triangles = triangles.ToArray();
		hexMesh.RecalculateNormals();
        meshCollider.sharedMesh = hexMesh;
    }
	
	void Triangulate (HexCell cell) {
        Vector3 center = cell.transform.position + Vector3.up * vertOffset;
        for (int i = 0; i < 6; i++) {
		AddTriangle(
			center,
			center + HexMetrics.corners[i],
			center + HexMetrics.corners[(i+1) % 6]
		);
		//Debug.Log("Triangulating at "  + center);
        AddTriangleColor(cell.color);
        }
	}

	public void RecolorCell (HexCell cell)
	{
		colors[cell.id * 3] = cell.color;
        colors[cell.id * 3 + 1] = cell.color;
        colors[cell.id * 3 + 2] = cell.color;
        hexMesh.colors = colors.ToArray();
    }

    public void RecolorMesh()
    {
		//Triangulate();

        colors.Clear();
        
        for (int i = 0; i < hexGrid.cells.Length; i++)
        {
            for (int j = 0; j < 6; j++)
            {
				AddTriangleColor(hexGrid.cells[i].color);
			}
        }

        hexMesh.colors = colors.ToArray();
    }

    void AddTriangle (Vector3 v1, Vector3 v2, Vector3 v3) {
		int vertexIndex = vertices.Count;
		vertices.Add(v1);
		vertices.Add(v2);
		vertices.Add(v3);
		triangles.Add(vertexIndex);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 2);

	}

    void AddTriangleColor(Color color)
    {
        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
    }
}