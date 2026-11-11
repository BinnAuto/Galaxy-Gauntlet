public partial class FireTile2(Vector2I coordinate) : MapTile2(coordinate)
{
	public override string MapObjectName => "Fire";

	public override int EntityID => GGConstants.ByteCodes.Entities.Fire;

	public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.Fire;
}
