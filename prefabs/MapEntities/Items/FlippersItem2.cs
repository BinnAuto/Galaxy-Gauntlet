public partial class FlippersItem2(Vector2I coordinate) : MapItem2(coordinate)
{
	public override string MapObjectName => "Flippers";

	public override int EntityID => GGConstants.ByteCodes.Entities.Flippers;

	public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.Flippers;
}
