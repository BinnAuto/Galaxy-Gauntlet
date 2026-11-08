public partial class HintPanelTile2(Vector2I coordinate) : MapTile2(coordinate)
{
	public override int ByteCode => GGConstants.ByteCodes.Entities.HintPanel;

	protected override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.HintPanel;
}
