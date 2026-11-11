public partial class BlueButtonTile2(Vector2I coordinate) : ButtonTile2(coordinate)
{
	public override string MapObjectName => "Blue Button";

	public override int EntityID => GGConstants.ByteCodes.Entities.BlueButton;

	public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.BlueButton;
}
