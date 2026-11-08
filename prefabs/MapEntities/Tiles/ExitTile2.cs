public partial class ExitTile2(Vector2I coordinate) : MapTile2(coordinate)
{
	public override int ByteCode => GGConstants.ByteCodes.Entities.Exit;

	protected override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.Exit;
}
