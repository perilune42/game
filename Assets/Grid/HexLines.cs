using UnityEngine;
using System.Collections.Generic;

public class HexLines : MonoBehaviour {

	//Mesh hexMesh;
	List<Vector3> vertices;
	List<int> triangles;
	LineRenderer lineRenderer;
	public float vertOffset = -1f;

	void Awake () {
		vertices = new List<Vector3>();
		//triangles = new List<int>();
		lineRenderer = GetComponent<LineRenderer>();
	}

	public void DrawGrid(HexGrid hexGrid)
	{
		HexCell cell;
		Vector3 lastCellPos;
		int lastOddX;
        vertices.Clear();
		lineRenderer.positionCount = 0;
        for (int y = 0; y < hexGrid.height; y++)
        {

            for (int x = 0; x < hexGrid.width; x+= 2) {
                cell = hexGrid.GetCellAtPos(x,y);
                Vector3 center = cell.transform.position + Vector3.up * vertOffset;
                AddPoint(center + HexMetrics.corners[4]); //W
                AddPoint(center + HexMetrics.corners[3]); //SW
                AddPoint(center + HexMetrics.corners[2]); //SE
                AddPoint(center + HexMetrics.corners[1]); //E
				
            }
			if (hexGrid.width % 2 == 0) { lastOddX = hexGrid.width-1; }
			else { lastOddX = hexGrid.width - 2; };

			lastCellPos = hexGrid.GetCellAtPos(hexGrid.width - 1, y).transform.position + Vector3.up * vertOffset;
			if (hexGrid.width % 2 == 1) AddPoint(lastCellPos + HexMetrics.corners[0]);
			else
			{
				AddPoint(lastCellPos + HexMetrics.corners[2]);
				AddPoint(lastCellPos + HexMetrics.corners[1]);
				AddPoint(lastCellPos + HexMetrics.corners[0]);
            }


            for (int x = lastOddX; x > 0; x -= 2)
            {
                cell = hexGrid.GetCellAtPos(x, y);
                Vector3 center = cell.transform.position + Vector3.up * vertOffset;
                AddPoint(center + HexMetrics.corners[1]); //W
                AddPoint(center + HexMetrics.corners[2]); //SW
                AddPoint(center + HexMetrics.corners[3]); //SE
                AddPoint(center + HexMetrics.corners[4]); //E

            }
			lastCellPos = hexGrid.GetCellAtPos(0, y).transform.position + Vector3.up * vertOffset;
            AddPoint(lastCellPos + HexMetrics.corners[5]);
            AddPoint(lastCellPos + HexMetrics.corners[4]);
            AddPoint(lastCellPos + HexMetrics.corners[5]);

        }
		for (int x = 1; x < hexGrid.width; x+= 2)
		{
            cell = hexGrid.GetCellAtPos(x, hexGrid.height-1);
            Vector3 center = cell.transform.position + Vector3.up * vertOffset;
            AddPoint(center + HexMetrics.corners[4]); 
            AddPoint(center + HexMetrics.corners[5]); 
            AddPoint(center + HexMetrics.corners[0]); 
            AddPoint(center + HexMetrics.corners[1]); 
        }
    }

	void Triangulate (HexCell cell) {

    	
        Vector3 center = cell.transform.position + Vector3.up * vertOffset;
        for (int i = 0; i < 6; i++) {
        Vector3 start = center + HexMetrics.corners[i];
        Vector3 end = center + HexMetrics.corners[(i + 1) % 6];
        //AddPoint(start, end);
        }
	}


	void AddPoint(Vector3 pos) {
    lineRenderer.positionCount += 1;
    lineRenderer.SetPosition(lineRenderer.positionCount - 1, pos);
}
    
}