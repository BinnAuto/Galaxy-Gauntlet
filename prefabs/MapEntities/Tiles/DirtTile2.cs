public partial class DirtTile2(Vector2I coordinate) : MapTile2(coordinate)
{
	public override string MapObjectName => "Dirt";

	public override int EntityID => GGConstants.ByteCodes.Entities.Dirt;

	public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.Dirt;
}
