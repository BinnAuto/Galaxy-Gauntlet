public partial class FloorTile2(Vector2I coordinate) : MapTile2(coordinate)
{
	public override string MapObjectName => "Floor";

	public override int EntityID => GGConstants.ByteCodes.Entities.Floor;

	public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.Floor;
}
