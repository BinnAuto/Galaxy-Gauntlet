public partial class TrapTile2(Vector2I coordinate) : MapTile2(coordinate)
{
	public bool IsActive = false;


	public override int ByteCode => (IsActive)
		? GGConstants.ByteCodes.Entities.Trap
		: GGConstants.ByteCodes.Entities.OpenTrap;


	protected override int SpriteFrameIndex => IsActive
		? GGConstants.SpriteFrameIndexes.TrapActive
		: GGConstants.SpriteFrameIndexes.TrapInactive;
}
