public partial class HiddenWallTile2(Vector2I coordinate) : MapTile2(coordinate)
{
	public override string MapObjectName => "Hidden Wall";

	public override int EntityID => GGConstants.ByteCodes.Entities.HiddenWall;

	public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.Floor;
}
