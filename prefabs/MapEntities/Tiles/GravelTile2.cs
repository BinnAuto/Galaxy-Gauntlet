public partial class GravelTile2(Vector2I coordinate) : MapTile2(coordinate)
{
	public override string MapObjectName => "Gravel";

	public override int EntityID => GGConstants.ByteCodes.Entities.Gravel;

	public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.Gravel;
}
