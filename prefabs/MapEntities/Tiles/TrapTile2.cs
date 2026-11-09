public partial class TrapTile2(Vector2I coordinate) : MapTile2(coordinate)
{
	public bool IsActive = true;

	public override string MapObjectName => "Trap";

	public override int EntityID => IsActive
		? GGConstants.ByteCodes.Entities.TrapActive
		: GGConstants.ByteCodes.Entities.TrapInactive;

	public override int SpriteFrameIndex => IsActive    
		? GGConstants.SpriteFrameIndexes.TrapActive
		: GGConstants.SpriteFrameIndexes.TrapInactive;
}
