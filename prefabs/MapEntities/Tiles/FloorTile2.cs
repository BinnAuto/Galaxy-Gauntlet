public partial class FloorTile2(Vector2I coordinate) : MapTile2(coordinate)
{
	public override int ByteCode => GGConstants.ByteCodes.Entities.Floor;

	protected override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.Floor;
}
