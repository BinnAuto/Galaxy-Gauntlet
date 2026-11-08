using GalaxyGauntlet.scripts;

public partial class MapDisplay2 : Node2D
{
	public int GameTick => GameData.Level.CurrentTick;
			

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var newChildren = GameData.Map.GetNewMapObjects();
		foreach(var child in newChildren)
		{
			AddChild(child);
		}
	}


	// AdvanceGame
	public override void _PhysicsProcess(double delta)
	{
		if(false == GameData.Level.TimerEnabled)
		{
			return;
		}
		InitialHousekeeping();

		int gameTick = GameData.Level.CurrentTick;
		GameData.Level.CurrentTick++;
		if(gameTick != 0 && (gameTick & 1) == 0)
		{
			// Executes every other tick
			foreach(var creature in GameData.Map.CreaturesToProcess)
			{
				if(creature is Player2
					|| (creature.State & CreatureState.Cloning) == CreatureState.Cloning)
				{
					continue;
				}

				creature.ChooseMove();
				if(creature.TDir == Vector2I.Zero)
				{
					continue;
				}

				creature.AdvanceCreature(creature.TDir);
			}
			if(CheckForEnding())
			{
				return;
			}

			FloorMovements();
			if(CheckForEnding())
			{
				return;
			}
		}
		UpdateSlipList();

		// TODO: Out of time

		foreach (var creature in GameData.Map.CreaturesToProcess)
		{
			if (creature is not Player2)
			{
				continue;
			}

			creature.ChooseMove();
			if (creature.TDir == Vector2I.Zero)
			{
				continue;
			}

			creature.AdvanceCreature(creature.TDir);
			if (CheckForEnding())
			{
				return;
			}
			GD.Print("Player has moved");
			creature.State |= CreatureState.HasMoved;
		}
		UpdateSlipList();
		CreateClones();
	}


	private void InitialHousekeeping()
	{
		if (GameTick == 0)
		{
			// TODO: laststepping = stepping();
		}

		int tickMod3 = GameTick % 3;
		if(tickMod3 != 0)
		{
			return;
		}

		for(int i = 1; i < GameData.Map.CreaturesToProcess.Count; i++)
		{
			GameData.Map.CreaturesToProcess[i].UpdateCreature();
		}

		GameData.Level.ChipWaitTick++;
		if(GameData.Level.ChipWaitTick > 3)
		{
			GameData.Level.ChipWaitTick = 3;
			GameData.Map.CreaturesToProcess[0].UpdateCreature();
		}
	}


	private bool CheckForEnding()
	{
		var currentTile = GameData.Map.GetTile2(GameData.Level.PlayerCoordinate);
		if(currentTile is ExitTile2)
		{
			// Win!
			return true;
		}

		if(false == string.IsNullOrEmpty(GameData.Level.DeathMessage))
		{
			// Lose!
			return true;
		}

		return false;
	}


	#region Floor Movements

	private void FloorMovements()
	{
		ChipFloorMovements();
		UpdateSlipList();
		if(false == CheckForEnding())
		{
			CreatureFloorMovements();
		}
	}


	private void ChipFloorMovements()
	{
		foreach(var creature in GameData.Map.CreaturesToProcess)
		{
			if(creature is not Player2)
			{
				continue;
			}
			if((creature.State & (CreatureState.Slipping | CreatureState.Sliding)) == 0)
			{
				continue;
			}
			Vector2I slipDirection = creature.Orientation.ToVector();
			if(creature is Player2 && slipDirection == Vector2I.Zero)
			{
				creature.Orientation = EntityOrientation.North;
			}
			if(slipDirection == Vector2I.Zero)
			{
				continue;
			}
			bool creatureAdvanced = creature.AdvanceCreature(slipDirection);
			if (creatureAdvanced)
			{
				creature.State &= ~(CreatureState.HasMoved);
			}
			else
			{
				var mapTile = GameData.Map.GetTile2(creature.Coordinate);
				if(mapTile is ForceFloorTile2)
				{
					creature.State &= ~(CreatureState.HasMoved);
				}
				else if(mapTile is IceTile2 iceTile)
				{
					slipDirection = iceTile.GetNewCreatureDirection(creature.Orientation).ToVector();
					creatureAdvanced = creature.AdvanceCreature(slipDirection);
					if (creatureAdvanced)
					{
						creature.State &= ~(CreatureState.HasMoved);
					}
				}
				// TODO: If floor is Teleporter or Static Block
				if ((creature.State & (CreatureState.Slipping | CreatureState.Sliding)) != 0)
				{
					creature.EndFloorMovement();
					creature.StartFloorMovement();
				}
			}
			if (CheckForEnding())
			{
				return;
			}
		}
	}


	private void CreatureFloorMovements()
	{

	}

	#endregion


	private void UpdateSlipList()
	{
		// TODO
	}


	private void CreateClones()
	{
		foreach(var creature in GameData.Map.CreaturesToProcess)
		{
			if((creature.State & CreatureState.Cloning) == CreatureState.Cloning)
			{
				creature.State &= ~CreatureState.Cloning;
			}
		}
	}
}
