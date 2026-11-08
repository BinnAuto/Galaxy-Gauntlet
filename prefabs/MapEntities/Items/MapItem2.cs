public partial class MapItem2(Vector2I coordinate) : MapObject2(coordinate)
{
	public override bool RequiresLowerLayerTile => true;
}
