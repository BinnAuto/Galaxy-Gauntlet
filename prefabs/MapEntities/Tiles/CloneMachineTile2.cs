public partial class CloneMachineTile2(Vector2I coordinate) : MapTile2(coordinate)
{
	public override string MapObjectName => "Clone Machine";

	public override int EntityID => GGConstants.ByteCodes.Entities.CloneMachine;

	public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.CloneMachine;
}
