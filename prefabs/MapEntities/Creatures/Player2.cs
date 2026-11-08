using GalaxyGauntlet.scripts;

public partial class Player2(Vector2I coordinate) : MapCreature2(coordinate)
{
	public override int ByteCode => GGConstants.ByteCodes.Entities.Player;

	public int PlayerState = 0;

	protected override int SpriteFrameIndex => Orientation switch
	{
		EntityOrientation.North => GGConstants.SpriteFrameIndexes.Player_N,
		EntityOrientation.East => GGConstants.SpriteFrameIndexes.Player_E,
		EntityOrientation.South => GGConstants.SpriteFrameIndexes.Player_S,
		EntityOrientation.West => GGConstants.SpriteFrameIndexes.Player_W,
		_ => GGConstants.SpriteFrameIndexes.DebugSprite
	};


	public Vector2I FacingDirection => Orientation switch
	{
		EntityOrientation.North => new(0, -1),
		EntityOrientation.East => new(1, 0),
		EntityOrientation.South => new(0, 1),
		EntityOrientation.West => new(-1, 0),
		_ => new(1, 0)
	};


	public override void _Process(double delta)
	{
		base._Process(delta);
	}


	// Replicates ChooseChipMove
	public override void ChooseMove()
	{
		TDir = Vector2I.Zero;

		var currentTick = GameData.Level.CurrentTick;
		if((currentTick % 3) == 0)
		{
			State &= ~(CreatureState.HasMoved);
		}
		var input = GameData.Level.GetPlayerInput();
		if((State & CreatureState.HasMoved) == CreatureState.HasMoved)
		{
			return;
		}
		if((State & CreatureState.Slipping) == CreatureState.Slipping
			|| ((State & CreatureState.Sliding) == CreatureState.Sliding
				&& input == FacingDirection))
		{
			return;
		}

		TDir = input;
	}


	public bool CanOpenDoor(int byteCode)
	{
		return byteCode switch
		{
			GGConstants.ByteCodes.Entities.RedDoor => ConsumeItem(GGConstants.ByteCodes.Entities.RedKey),
			GGConstants.ByteCodes.Entities.BlueDoor => ConsumeItem(GGConstants.ByteCodes.Entities.BlueKey),
			GGConstants.ByteCodes.Entities.YellowDoor => ConsumeItem(GGConstants.ByteCodes.Entities.YellowKey),
			GGConstants.ByteCodes.Entities.GreenDoor => HasItem(GGConstants.ByteCodes.Entities.GreenKey),
			_ => false
		};
	}
}
