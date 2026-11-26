public partial class ExitTile2(Vector2I coordinate) : MapTile2(coordinate)
{
	public override string MapObjectName => "Exit";

	public override int EntityID => GGConstants.ByteCodes.Entities.Exit;

	public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.Exit;
}
