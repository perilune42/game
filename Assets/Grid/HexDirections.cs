using UnityEngine;

public enum HexDirection {
	N,NE,SE,S,SW,NW

}

public static class HexDirectionE {

	public static HexDirection Opposite(this HexDirection direction)
	{
		return (int)direction < 3 ? (direction + 3) : (direction - 3);
	}
    public static Vector2 ToV2(this HexDirection direction)
    {
        return HexCoordinates.FromDirection(direction).ToV2();
    }
	
	public static int Difference(HexDirection direction1, HexDirection direction2)
	{
		return Mathf.Abs((int)direction1 - (int)direction2);
	}
}