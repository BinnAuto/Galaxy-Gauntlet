public partial class RedTeleporterTile2(Vector2I coordinate) : ButtonTile2(coordinate)
{
	public override string MapObjectName => "Red Teleporter";

	public override int EntityID => GGConstants.ByteCodes.Entities.RedTeleporter;

	public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.RedTeleporter;
}
