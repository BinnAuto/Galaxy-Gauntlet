public partial class IceSkatesItem2(Vector2I coordinate) : MapItem2(coordinate)
{
	public override string MapObjectName => "Ice Skates";

	public override int EntityID => GGConstants.ByteCodes.Entities.IceSkates;

	public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.IceSkates;
}
