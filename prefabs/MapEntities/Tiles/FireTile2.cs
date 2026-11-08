public partial class FireTile2(Vector2I coordinate) : MapTile2(coordinate)
{
	public override int ByteCode => GGConstants.ByteCodes.Entities.Fire;

	protected override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.Fire;
}
