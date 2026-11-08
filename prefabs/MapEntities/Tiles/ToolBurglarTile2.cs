public partial class ToolBurglarTile2(Vector2I coordinate) : MapTile2(coordinate)
{
	public override int ByteCode => GGConstants.ByteCodes.Entities.ToolThief;

	protected override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.ToolThief;
}
