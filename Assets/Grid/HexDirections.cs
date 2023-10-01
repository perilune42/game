public enum HexDirection {
	N,NE,SE,S,SW,NW
}

public static class HexDirectionExtensions {

	public static HexDirection Opposite (this HexDirection direction) {
		return (int)direction < 3 ? (direction + 3) : (direction - 3);
	}
}