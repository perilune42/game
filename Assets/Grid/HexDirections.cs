public enum HexDirection {
	N,NE,SE,S,SW,NW
}
public static class HexVector {
	public static readonly HexCoordinates N = new HexCoordinates(0,-1,1);
	public static readonly HexCoordinates NE = new HexCoordinates(1,-1,0);
	public static readonly HexCoordinates SE = new HexCoordinates(1,0,-1);
	public static readonly HexCoordinates S = new HexCoordinates(0,1,-1);
	public static readonly HexCoordinates SW = new HexCoordinates(-1,1,0);
	public static readonly HexCoordinates NW = new HexCoordinates(-1,0,1);


	public static HexCoordinates FromDirection(HexDirection direction) {
        switch (direction) {
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
}
public static class HexDirectionExtensions {

	public static HexDirection Opposite (this HexDirection direction) {
		return (int)direction < 3 ? (direction + 3) : (direction - 3);
	}
}