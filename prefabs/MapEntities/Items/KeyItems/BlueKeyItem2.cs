public partial class BlueKeyItem2(Vector2I coordinate) : KeyItem2(coordinate)
{
	public override string MapObjectName => "Blue Key";

	public override int EntityID => GGConstants.ByteCodes.Entities.BlueKey;

	public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.BlueKey;
}
