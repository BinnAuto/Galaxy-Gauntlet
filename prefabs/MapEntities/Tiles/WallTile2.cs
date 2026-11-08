public partial class WallTile2(Vector2I coordinate) : MapTile2(coordinate)
{
	public override int ByteCode => GGConstants.ByteCodes.Entities.Wall;

	protected override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.Wall;
}
