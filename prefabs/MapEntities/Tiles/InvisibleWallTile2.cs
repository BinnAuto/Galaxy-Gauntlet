public partial class InvisibleWallTile2(Vector2I coordinate) : MapTile2(coordinate)
{
	public override string MapObjectName => "Invisible Wall";

	public override int EntityID => GGConstants.ByteCodes.Entities.InvisibleWall;

	public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.Floor;
}
