public partial class HintTile2(Vector2I coordinate) : MapTile2(coordinate)
{
	public override string MapObjectName => "Hint Panel";

	public override int EntityID => GGConstants.ByteCodes.Entities.HintPanel;

	public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.HintPanel;
}
