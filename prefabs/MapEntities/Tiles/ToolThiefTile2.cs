public partial class ToolThiefTile2(Vector2I coordinate) : MapTile2(coordinate)
{
	public override string MapObjectName => "Tool Thief";

	public override int EntityID => GGConstants.ByteCodes.Entities.ToolThief;

	public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.ToolThief;
}
