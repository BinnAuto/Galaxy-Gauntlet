public partial class WaterTile2(Vector2I coordinate) : MapTile2(coordinate)
{
	public override string MapObjectName => "Water";

	public override int EntityID => GGConstants.ByteCodes.Entities.Water;

	public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.Water;
}
