public partial class GreenKeyItem2(Vector2I coordinate) : KeyItem2(coordinate)
{
	public override string MapObjectName => "Green Key";

	public override int EntityID => GGConstants.ByteCodes.Entities.GreenKey;

	public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.GreenKey;
}
