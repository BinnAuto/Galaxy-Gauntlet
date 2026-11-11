public partial class GreenButtonTile2(Vector2I coordinate) : ButtonTile2(coordinate)
{
	public override string MapObjectName => "Green Button";

	public override int EntityID => GGConstants.ByteCodes.Entities.GreenButton;

	public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.GreenButton;
}
