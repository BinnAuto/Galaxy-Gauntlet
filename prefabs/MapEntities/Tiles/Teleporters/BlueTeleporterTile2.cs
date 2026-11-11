public partial class BlueTeleporterTile2(Vector2I coordinate) : ButtonTile2(coordinate)
{
	public override string MapObjectName => "Blue Teleporter";

	public override int EntityID => GGConstants.ByteCodes.Entities.BlueTeleporter;

	public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.BlueTeleporter;
}
