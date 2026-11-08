public partial class HiddenWallTile2(Vector2I coordinate) : MapTile2(coordinate)
{
	public bool IsVisible = false;

	public override int ByteCode => GGConstants.ByteCodes.Entities.HiddenWall;

	protected override int SpriteFrameIndex => (IsVisible)
		? GGConstants.ByteCodes.Entities.Wall
		: GGConstants.SpriteFrameIndexes.Floor;
}
