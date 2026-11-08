public partial class MapItem2(Vector2I coordinate) : MapObject(coordinate)
{
	public override bool RequiresLowerLayerTile => true;

	public override void _Ready()
	{
		base._Ready();
		ZIndex = 6;
	}
}
