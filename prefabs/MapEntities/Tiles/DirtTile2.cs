public partial class DirtTile2(Vector2I coordinate) : MapTile2(coordinate)
{
	public override int ByteCode => GGConstants.ByteCodes.Entities.Dirt;

	protected override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.Dirt;
}
