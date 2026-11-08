public partial class IceBlockCreature2(Vector2I coordinate) : MapCreature2(coordinate)
{
	public override int ByteCode => GGConstants.ByteCodes.Entities.IceBlock;

	protected override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.IceBlock;
}
