public partial class CloneMachineTile2(Vector2I coordinate) : MapTile2(coordinate)
{
	public override int ByteCode => GGConstants.ByteCodes.Entities.CloneMachine;

	protected override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.CloneMachine;
}
