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
			var slipDirection = SlipDirection;
			if(direction == SlipDirection || direction == -1 * slipDirection)
			{
				if((flags & CanMakeMoveFlags.TeleportPush) == 0)
				{
					return false;
				}
			}
		}

		var currentCreature = GameData.Map.GetCreature(Coordinate); // Is this not just itself?
		if((flags & CanMakeMoveFlags.TeleportPush) == 0
			&& currentCreature is DirtBlockCreature2)
		{

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
