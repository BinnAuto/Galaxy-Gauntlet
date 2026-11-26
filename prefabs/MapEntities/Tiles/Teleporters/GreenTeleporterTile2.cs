public partial class GreenTeleporterTile2(Vector2I coordinate) : ButtonTile2(coordinate)
{
	public override string MapObjectName => "Green Teleporter";

	public override int EntityID => GGConstants.ByteCodes.Entities.GreenTeleporter;

	public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.GreenTeleporter;
}
