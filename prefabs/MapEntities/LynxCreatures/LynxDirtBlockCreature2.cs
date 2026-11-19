using GalaxyGauntlet.scripts;

public partial class LynxDirtBlockCreature2(Vector2I coordinate) : MapLynxCreature2(coordinate)
{
	public override string MapObjectName => "Dirt Block";

	public override int EntityID => GGConstants.ByteCodes.Entities.DirtBlock;

	public override int SpriteFrameIndex => GameData.Level.XRayMode
		? GGConstants.SpriteFrameIndexes.DirtBlock_XRay
		: GGConstants.SpriteFrameIndexes.DirtBlock;


	public bool CanPushBlock(Vector2I direction, int flags)
	{
		bool pushBlockFlag = (flags & (CanMakeMoveFlags.PushBlocks | CanMakeMoveFlags.PushBlocksNow)) != 0;
		if (false == CanMakeMove(direction, flags))
		{
			if(Moving == 0 && pushBlockFlag)
			{
				SetOrientation(direction);
				if(GameData.LynxPedantic)
				{
					TDir = direction;
				}
			}
			return false;
		}

		if(pushBlockFlag)
		{
			SetOrientation(direction);
			TDir = direction;
			CreatureState |= CreatureStateFlags.Pushed;
			if((flags & CanMakeMoveFlags.PushBlocksNow) != 0)
			{
				AdvanceCreature(false);
			}
		}

		return true;
	}
}
