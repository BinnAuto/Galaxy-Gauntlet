public partial class RedKeyItem2(Vector2I coordinate) : KeyItem2(coordinate)
{
	public override string MapObjectName => "Red Key";

	public override int EntityID => GGConstants.ByteCodes.Entities.RedKey;

	public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.RedKey;
}
