public partial class WaterTile2(Vector2I coordinate) : MapTile2(coordinate)
{
	public override int ByteCode => GGConstants.ByteCodes.Entities.Water;

	protected override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.Water;
}
