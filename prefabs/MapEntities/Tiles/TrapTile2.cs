using GalaxyGauntlet.scripts;

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


	public void OpenTrap()
	{
		IsActive = false;
		var creature = GameData.Map.GetCreature(Coordinate);
		if(creature is null)
		{
			GD.Print($"Creature not found at {Coordinate}");
			return;
		}

		GD.Print($"Releasing {creature.MapObjectName} {creature.Coordinate}");
		creature.CreatureState |= CreatureStateFlags.Released;
	}
}
