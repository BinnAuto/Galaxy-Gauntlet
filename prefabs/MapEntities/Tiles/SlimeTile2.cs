public partial class SlimeTile2(Vector2I coordinate) : ButtonTile2(coordinate)
{
	public override string MapObjectName => "Slime";

	public override int EntityID => GGConstants.ByteCodes.Entities.Slime;

	public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.Slime;
}
