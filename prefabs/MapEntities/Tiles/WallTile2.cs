public partial class WallTile2(Vector2I coordinate) : MapTile2(coordinate)
{
	public override string MapObjectName => "Wall";

	public override int EntityID => GGConstants.ByteCodes.Entities.Wall;

	public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.Wall;
}
