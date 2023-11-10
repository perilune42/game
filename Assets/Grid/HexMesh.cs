using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour {

	Mesh hexMesh;

    [SerializeField]
	HexGrid hexGrid;
	List<Vector3> vertices;
	List<int> triangles;
    MeshCollider meshCollider;
	public float vertOffset = -0.6f;
	List<Color> colors;
	public PlayerControl playerControl;
    Color tempColor;

    void Awake () {
		GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
        meshCollider = gameObject.AddComponent<MeshCollider>();
        hexMesh.name = "Hex Mesh";
		vertices = new List<Vector3>();
        colors = new List<Color>();
        triangles = new List<int>();
		hexMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        
    }

    private void Start()
    {
        GameEvents.instance.onRecolorMesh += RecolorMesh;
    }

    void Update()
    {


    }

    public void CheckNull()
    {
        if (hexGrid == null)
        {
            Debug.LogError("hexGrid nulled.");

        }
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

    private void RecolorMesh()
    {
        // Check if hexGrid is null before accessing its properties
        if (hexGrid == null)
        {
            //Debug.LogError("hexGrid is null.");
            return;
        }

        colors.Clear();

        for (int i = 0; i < hexGrid.height * hexGrid.width; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                if (hexGrid.cells[i].isDebug) tempColor = hexGrid.debugColor;
                else if (hexGrid.cells[i].isHighlighting) tempColor = hexGrid.highlightColor;
                else if (hexGrid.cells[i].isActive) tempColor = hexGrid.shipColor;
                else tempColor = hexGrid.cells[i].color;

                if (hexGrid.cells[i].isHovering) tempColor += new Color(0.1f, 0.1f, 0.1f, 0.1f);

                if (hexGrid.cells[i] != null)
                {
                    AddTriangleColor(tempColor);
                }
                else
                {
                    Debug.LogWarning("hexGrid.cells[" + i + "] is null.");
                }
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