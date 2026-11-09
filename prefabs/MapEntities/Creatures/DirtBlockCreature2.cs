public partial class DirtBlockCreature2(Vector2I coordinate) : MapCreature2(coordinate)
{
	public override string MapObjectName => "Dirt Block";

	public override int EntityID => GGConstants.ByteCodes.Entities.DirtBlock;

	public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.DirtBlock;
}
