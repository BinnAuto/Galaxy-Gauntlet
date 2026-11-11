public partial class YellowKeyItem2(Vector2I coordinate) : KeyItem2(coordinate)
{
	public override string MapObjectName => "Yellow Key";

	public override int EntityID => GGConstants.ByteCodes.Entities.YellowKey;

	public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.YellowKey;
}
