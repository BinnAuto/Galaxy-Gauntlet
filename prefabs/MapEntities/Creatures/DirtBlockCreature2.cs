using GalaxyGauntlet.scripts;

public partial class DirtBlockCreature2(Vector2I coordinate) : MapCreature2(coordinate)
{
	public override string MapObjectName => "Dirt Block";

	public override int EntityID => GGConstants.ByteCodes.Entities.DirtBlock;

	public override int SpriteFrameIndex => GGConstants.SpriteFrameIndexes.DirtBlock;


	public bool CanPushBlock(Vector2I direction, int flags)
	{
		bool isSlipping = HasState(CreatureStateFlags.Sliding | CreatureStateFlags.Slipping);
		if(isSlipping)
		{
			// TODO: Get slip direction
		}

		var currentCreature = GameData.Map.GetCreature(Coordinate);
		if((flags & CanMakeMoveFlags.TeleportPush) == 0
			&& currentCreature is DirtBlockCreature2)
		{
			// TODO
		}
		if((flags & CanMakeMoveFlags.NoDeferButtons) == 0) {
			CreatureState |= CreatureStateFlags.DeferPush;
		}
		var creatureAdvanced = AdvanceCreature(direction);
		if((flags & CanMakeMoveFlags.NoDeferButtons) == 0)
		{
			CreatureState &= ~(CreatureStateFlags.DeferPush);
		}
		if(false == creatureAdvanced)
		{
			CreatureState &= ~(CreatureStateFlags.Slipping | CreatureStateFlags.Sliding);
		}
		return creatureAdvanced;
	}
}
