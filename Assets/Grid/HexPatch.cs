using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct HexPatch
{
    public HexCoordinates center;
    public int radius;

    public HexPatch (HexCoordinates center, int radius)
    {
        this.center = center;
        this.radius = radius;
    }

    public HexCoordinates[] GetOuterCells()
    {
        List<HexCoordinates> outerCells = new List<HexCoordinates>();
        
        for (int i = 0; i < radius; i++)
        {
            outerCells.Add(center + HexCoordinates.N * radius + HexCoordinates.SE * i);
        }
        for (int i = 0; i < radius; i++)
        {
            outerCells.Add(center + HexCoordinates.NE * radius + HexCoordinates.S * i);
        }
        for (int i = 0; i < radius; i++)
        {
            outerCells.Add(center + HexCoordinates.SE * radius + HexCoordinates.SW * i);
        }
        for (int i = 0; i < radius; i++)
        {
            outerCells.Add(center + HexCoordinates.S * radius + HexCoordinates.NW * i);
        }
        for (int i = 0; i < radius; i++)
        {
            outerCells.Add(center + HexCoordinates.SW * radius + HexCoordinates.N * i);
        }
        for (int i = 0; i < radius; i++)
        {
            outerCells.Add(center + HexCoordinates.NW * radius + HexCoordinates.NE * i);
        }

        return outerCells.ToArray();

    }

    public HexCoordinates[] GetCells(bool includeCenter = true)
    {
        List<HexCoordinates> cells = new List<HexCoordinates>();
        if (includeCenter) cells.Add(center);
        for (int r = 1; r <= radius; r++)
        {
            for (int i = 0; i < r; i++)
            {
                cells.Add(center + HexCoordinates.N * r + HexCoordinates.SE * i);
            }
            for (int i = 0; i < r; i++)
            {
                cells.Add(center + HexCoordinates.NE * r + HexCoordinates.S * i);
            }
            for (int i = 0; i < r; i++)
            {
                cells.Add(center + HexCoordinates.SE * r + HexCoordinates.SW * i);
            }
            for (int i = 0; i < r; i++)
            {
                cells.Add(center + HexCoordinates.S * r + HexCoordinates.NW * i);
            }
            for (int i = 0; i < r; i++)
            {
                cells.Add(center + HexCoordinates.SW * r + HexCoordinates.N * i);
            }
            for (int i = 0; i < r; i++)
            {
                cells.Add(center + HexCoordinates.NW * r + HexCoordinates.NE * i);
            }
        }

        return cells.ToArray();
    }

    public Vector3[] GetRelativeOuterVertices()
    {
        //HexCoordinates[] outerCells = GetOuterCells();
        List<Vector3> outerVertices = new List<Vector3>();

        for (int dir = 0; dir < 6; dir++)
        {
            outerVertices.Add(HexMetrics.vectors[dir] * radius + HexMetrics.corners[(dir + 5) % 6]);
            outerVertices.Add(HexMetrics.vectors[dir] * radius + HexMetrics.corners[dir]);
            outerVertices.Add(HexMetrics.vectors[dir] * radius + HexMetrics.corners[(dir + 1) % 6]);
            for (int i = 1; i < radius; i++)
            {
                outerVertices.Add(HexMetrics.vectors[dir] * radius + HexMetrics.vectors[(dir + 2) % 6] * i + HexMetrics.corners[dir]);
                outerVertices.Add(HexMetrics.vectors[dir] * radius + HexMetrics.vectors[(dir + 2) % 6] * i + HexMetrics.corners[(dir + 1) % 6]);
            }
        }
        outerVertices.Add(HexMetrics.vectors[0] * radius + HexMetrics.corners[5]);
        return outerVertices.ToArray();
        
    }
}
