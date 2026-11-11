public partial class BrownButtonTile2(Vector2I coordinate) : ButtonTile2(coordinate)
{
	public override string MapObjectName => "Brown Button";

	public override int EntityID => GGConstants.ByteCodes.Entities.BrownButton;

	public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.BrownButton;
}
