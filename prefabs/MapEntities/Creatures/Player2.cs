using GalaxyGauntlet.scripts;

public enum PlayerStatus
{
	Okay,
	Drowned,
	Burned,
	Bombed,
	OutOfTime,
	Collided,
	Squished,
	SquishedDeath,
	NotOkay
}

public partial class Player2(Vector2I coordinate) : MapCreature2(coordinate)
{
	public override string MapObjectName => "Player";

	public override int EntityID => GGConstants.ByteCodes.Entities.Player;

	public PlayerStatus PlayerStatus = PlayerStatus.Okay;

    public override int SpriteFrameIndex => Orientation switch
	{
		EntityOrientation.North => GGConstants.SpriteFrameIndexes.Player_N,
		EntityOrientation.East => GGConstants.SpriteFrameIndexes.Player_E,
		EntityOrientation.South => GGConstants.SpriteFrameIndexes.Player_S,
		EntityOrientation.West => GGConstants.SpriteFrameIndexes.Player_W,
		_ => GGConstants.SpriteFrameIndexes.DebugSprite
	};

    public override void _Ready()
    {
        base._Ready();
		ZIndex = 9;
    }


	public override void ChooseMove()
	{
		ChooseChipMove();
	}


	private void ChooseChipMove()
	{
		bool isSlipping = HasState(CreatureStateFlags.Slipping);
		TDir = Vector2I.Zero;
		int gameTick = GameData.Level.CurrentTick;
		if((gameTick & 3) == 0)
		{
			CreatureState &= ~(CreatureStateFlags.HasMoved);
		}
		if(HasState(CreatureStateFlags.HasMoved))
		{
			return;
		}

		var direction = GameData.GetPlayerInput();
		if(isSlipping
			|| (HasState(CreatureStateFlags.Sliding) && direction == Forward))
		{
			return;
		}

		TDir = direction;
	}
}
