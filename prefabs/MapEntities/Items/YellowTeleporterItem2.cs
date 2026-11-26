public partial class YellowTeleporterItem2(Vector2I coordinate) : MapItem2(coordinate)
{
	public override bool RequiresLowerLayerTile => false;

	public override string MapObjectName => "Yellow Teleporter";

	public override int EntityID => GGConstants.ByteCodes.Entities.YellowTeleporter;

	public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.YellowTeleporter;
}
