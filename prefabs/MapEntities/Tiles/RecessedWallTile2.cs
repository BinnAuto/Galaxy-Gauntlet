public partial class RecessedWallTile2(Vector2I coordinate) : MapTile2(coordinate)
{
	public override int ByteCode => GGConstants.ByteCodes.Entities.RecessedWall;

	protected override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.RecessedWall;
}
