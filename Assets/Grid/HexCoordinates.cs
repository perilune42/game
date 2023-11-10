using System;
using System.Drawing;
using UnityEngine;

[System.Serializable]
public struct HexCoordinates {

    [SerializeField]
	private int x, z;

	public int X { 		
        get {
			return x;
		} 
    }

	public int Z { 		
        get {
			return z;
		} 
    }

    public int Y {
		get {
			return -X - Z;
		}
	}

	public HexCoordinates (int x, int z) {
		this.x = x;
		this.z = z;
	}

	public HexCoordinates (int x, int y, int z) {
		this.x = x;
		this.z = z;
	}

    public static readonly HexCoordinates N = new HexCoordinates(0, -1, 1);
    public static readonly HexCoordinates NE = new HexCoordinates(1, -1, 0);
    public static readonly HexCoordinates SE = new HexCoordinates(1, 0, -1);
    public static readonly HexCoordinates S = new HexCoordinates(0, 1, -1);
    public static readonly HexCoordinates SW = new HexCoordinates(-1, 1, 0);
    public static readonly HexCoordinates NW = new HexCoordinates(-1, 0, 1);

    public static HexCoordinates FromDirection(HexDirection direction)
    {
        switch (direction)
        {
            case HexDirection.N:
                return N;
            case HexDirection.NE:
                return NE;
            case HexDirection.SE:
                return SE;
            case HexDirection.S:
                return S;
            case HexDirection.SW:
                return SW;
            case HexDirection.NW:
                return NW;
            default:
                throw new System.ArgumentException("Invalid HexDirection");
        }
    }


    public static HexCoordinates operator +(HexCoordinates a, HexCoordinates b) {
		return new HexCoordinates(a.X+b.X, a.Z+b.Z);
	}

    public static HexCoordinates operator -(HexCoordinates a, HexCoordinates b)
    {
        return new HexCoordinates(a.X - b.X, a.Z - b.Z);
    }

    public static HexCoordinates operator *(HexCoordinates a, int b) {
		return new HexCoordinates(a.X * b, a.Z * b);
	}

    public static HexCoordinates operator *(HexCoordinates a, float b)
    {
        return new HexCoordinates(Mathf.RoundToInt(a.X * b), Mathf.RoundToInt(a.Z * b));
    }

    public static HexCoordinates operator *(int b, HexCoordinates a) {
		return new HexCoordinates(a.X * b, a.Z * b);
	}
    public static int operator *(HexCoordinates a, HexCoordinates b)
    {
        return a.X * b.X + a.Z * b.Z;
    }

    public static bool operator ==(HexCoordinates a, HexCoordinates b)
    {
        return (a.X == b.X) && (a.Z == b.Z);
    }

    public static bool operator !=(HexCoordinates a, HexCoordinates b)
    {
        return !((a.X == b.X) && (a.Z == b.Z));
    }

    public static HexCoordinates FromWorldPosition(Vector3 position)
    {
        float z = position.z / (HexMetrics.innerRadius * 2f);
		float y = -z;
        float offset = position.x / (HexMetrics.outerRadius * 3f);
        z -= offset;
        y -= offset;
        int iZ = Mathf.RoundToInt(z);
        int iY = Mathf.RoundToInt(y);
        int iX = Mathf.RoundToInt(-z - y);

        if (iX + iY + iZ != 0)
        {
            return new HexCoordinates(-1, -1);
            /*
            float dX = Mathf.Abs(z - iX);
            float dY = Mathf.Abs(y - iY);
            float dZ = Mathf.Abs(-z - y - iZ);

            if (dX > dY && dX > dZ)
            {
                iX = -iY - iZ;
            }
            else if (dZ > dY)
            {
                iZ = -iX - iY;
            }
            */
        }

        return new HexCoordinates(iX, iZ);
    }
    public static HexCoordinates FromOffsetCoordinates (int x, int z) {
		return new HexCoordinates(x, z-x/2);
	}

	public Vector2Int ToOffsetCoordinates () {
		return new Vector2Int(X,Z+X/2);
	}
    public int ToOffsetCoordinates(int axis)
    {
		if (axis == 0) return X;
		else if (axis == 1) return Z + X / 2;
		return 0;
    }

    public override string ToString () {
		return "(" + X.ToString() + ", " + Y.ToString() + ", " + Z.ToString() + ")";
	}

	public string ToStringOnSeparateLines () {
		return X.ToString()+ "\n" + Y.ToString() + "\n" + Z.ToString();
	}

    public override bool Equals(object obj)
    {
        return obj is HexCoordinates coordinates &&
               x == coordinates.x &&
               z == coordinates.z;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x, z);
    }

    public static float Distance(HexCoordinates A, HexCoordinates B)
    {
        HexCoordinates vector = A - B;
        return (Math.Abs(vector.X) + Math.Abs(vector.Y) + Math.Abs(vector.Z)) / 2;
    }

    public Vector2 ToV2()
    {
        float x = (3.0f / 2 * X);
        float y = (float)(-Math.Sqrt(3) / 2 * X - Math.Sqrt(3) * Y);

        return new Vector2 (x, y).normalized;
    }
}