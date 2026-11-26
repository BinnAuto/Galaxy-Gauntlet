public partial class RedButtonTile2(Vector2I coordinate) : ButtonTile2(coordinate)
{
	public override string MapObjectName => "Red Button";

	public override int EntityID => GGConstants.ByteCodes.Entities.RedButton;

	public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.RedButton;
}
