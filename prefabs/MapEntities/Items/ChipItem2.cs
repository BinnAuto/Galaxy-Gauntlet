public partial class ChipItem2(Vector2I coordinate) : MapItem2(coordinate)
{
	public override int ByteCode => GGConstants.ByteCodes.Entities.Chip;

	protected override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.Chip;
}
