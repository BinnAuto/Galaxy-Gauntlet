public partial class SocketTile2(Vector2I coordinate) : MapTile2(coordinate)
{
	public override string MapObjectName => "Socket";

	public override int EntityID => GGConstants.ByteCodes.Entities.Socket;

	public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.Socket;
}
