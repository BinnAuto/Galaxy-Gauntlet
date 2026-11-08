public partial class SocketTile2(Vector2I coordinate) : MapTile2(coordinate)
{
	public override int ByteCode => GGConstants.ByteCodes.Entities.Socket;

	protected override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.Socket;
}
