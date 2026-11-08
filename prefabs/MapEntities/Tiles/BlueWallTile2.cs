public partial class BlueWallTile2(int byteCode, Vector2I coordinate) : MapTile2(coordinate)
{
	public override int ByteCode => byteCode;

	public bool IsReal => ByteCode == GGConstants.ByteCodes.Entities.BlueWall;

	protected override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.BlueWall;
}
