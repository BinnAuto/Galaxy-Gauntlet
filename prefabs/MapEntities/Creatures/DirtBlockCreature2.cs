public partial class DirtBlockCreature2(Vector2I coordinate) : MapCreature2(coordinate)
{
	public override int ByteCode => GGConstants.ByteCodes.Entities.DirtBlock;

	protected override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.DirtBlock;
}
