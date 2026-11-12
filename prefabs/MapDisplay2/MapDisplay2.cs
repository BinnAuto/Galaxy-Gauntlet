using GalaxyGauntlet.scripts;

public partial class MapDisplay2 : Node2D
{
	public override void _Process(double delta)
	{
		// Add new children to display
		var newChildren = GameData.Map.GetNewMapObjects();
		foreach(var child in newChildren)
		{
			AddChild(child);
		}

		// Set position in world
		int viewportSize = GameData.Level.MapViewportSize;
		int deltaFromRegular = viewportSize - GameData.Level.StandardMapViewportSize;
		Position = new Vector2I(192, 192) - (16 * deltaFromRegular * Vector2I.One);
	}


	// AdvanceGame
	public override void _PhysicsProcess(double delta)
	{
		if(false == GameData.Level.TimerEnabled)
		{
			return;
		}

		GameData.Level.Timer -= delta;
		int gameTick = GameData.Level.CurrentTick;
		InitialHousekeeping();
		GameData.Level.CurrentTick++;
		var creaturesToProcess = GameData.Map.GetCreaturesToProcess();

		#region Advance Creatures

		if (gameTick != 0 && (gameTick & 1) == 0)
		{
			foreach (var creature in creaturesToProcess)
			{
				if(creature is Player2)
				{
					continue;
				}
				if(creature.HasState(CreatureStateFlags.Cloning))
				{
					GD.Print($"Creature {creature.MapObjectName} at {creature.Coordinate} is busy cloning");
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
		}

		#endregion

		#region Process "Slip List"

		if (gameTick != 0 && (gameTick & 1) == 0)
		{
			SliplistMovements();
			if(CheckForEnding())
			{
				return;
			}
		}
		UpdateSlipList();

		#endregion

		#region Timer Check

		if (GameData.Map.TimeLimit > 0)
		{
			if (GameData.Level.Timer < 0)
			{
				// TODO: Out of time
			}
		}

		#endregion

		#region Advance Player

		if(GameData.Level.ChipTick >= 3)
		{
			foreach (var creature in creaturesToProcess)
			{
				if(creature is not Player2)
				{
					continue;
				}

				creature.ChooseMove();
				if(creature.TDir != Vector2I.Zero)
				{
					creature.AdvanceCreature(creature.TDir);
					if (CheckForEnding())
					{
						return;
					}
					creature.CreatureState |= CreatureStateFlags.HasMoved;
				}
			}
		}

		#endregion

		UpdateSlipList();
		CreateClones();
		FinalHousekeeping();
	}


	public void ClearChildren()
	{
		foreach(var child in GetChildren())
		{
			RemoveChild(child);
			child.QueueFree();
		}
	}

	#region Tile World adaptation methods

	private void InitialHousekeeping()
	{
		int gameTick = GameData.Level.CurrentTick;
		var creaturesToProcess = GameData.Map.GetCreaturesToProcess();
		if(gameTick % 3 == 0)
		{
			foreach(var creature in creaturesToProcess)
			{
				if(creature is Player2)
				{
					continue;
				}

				if(creature.HasState(CreatureStateFlags.Turning))
				{
					creature.CreatureState &= ~(CreatureStateFlags.Turning | CreatureStateFlags.HasMoved);
					creature.UpdateCreature();
				}
			}
		}

		// In Tile World, this block is inside the above if.
		// But it feels more natural if it's put here?
		GameData.Level.ChipTick++;
		if(GameData.Level.ChipTick > 3)
		{
			GameData.Level.ChipTick = 3;
			foreach (var creature in creaturesToProcess)
			{
				if(creature is not Player2)
				{
					continue;
				}

				creature.UpdateCreature();
			}
		}
	}

	private bool CheckForEnding()
	{
		// TODO
		return false;
	}


	// FloorMovements
	private void SliplistMovements()
	{
		ChipSlipMovements();
		UpdateSlipList();
		if(false == CheckForEnding())
		{
			CreatureSlipMovements();
		}
		// TODO
	}


	// FloorMovements_Of_Chip
	private void ChipSlipMovements()
	{
		// TODO
	}


	// FloorMovements_Of_Blocks_And_Monsters
	private void CreatureSlipMovements()
	{
		// TODO
	}


	private void UpdateSlipList()
	{
		// TODO
	}


	private static void CreateClones()
    {
		// TODO
    }


	private void FinalHousekeeping()
	{
		// Nothing. It's literally nothing.
	}

	#endregion
}
