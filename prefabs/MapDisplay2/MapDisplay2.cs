using GalaxyGauntlet.scripts;

public partial class MapDisplay2 : Node
{
	public override void _Process(double delta)
	{
		var newChildren = GameData.Map.GetNewMapObjects();
		foreach(var child in newChildren)
		{
			AddChild(child);
		}
	}


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

		#region Advance Creatures

		if (gameTick != 0 && (gameTick & 1) == 0)
		{
			foreach(var creature in GameData.Map.CreaturesToProcess)
			{
				if(creature is Player2)
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
		}

		#endregion

		#region Floor Movements

		if (gameTick != 0 && (gameTick & 1) == 0)
		{
			FloorMovements();
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

		foreach(var creature in GameData.Map.CreaturesToProcess)
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
				creature.State |= CreatureStateFlags.HasMoved;
			}
		}

		#endregion

		UpdateSlipList();
		CreateClones();
		FinalHousekeeping();
	}


	#region Tile World adaptation methods

	private void InitialHousekeeping()
	{
		int gameTick = GameData.Level.CurrentTick;
		if(gameTick % 3 == 0)
		{
			foreach(var creature in GameData.Map.CreaturesToProcess)
			{
				if(creature is Player2)
				{
					continue;
				}

				if(creature.HasState(CreatureStateFlags.Turning))
				{
					creature.State &= ~(CreatureStateFlags.Turning | CreatureStateFlags.HasMoved);
					creature.UpdateCreature();
				}
			}

			GameData.Level.ChipTick++;
			GD.Print(GameData.Level.ChipTick);
			if(GameData.Level.ChipTick > 3)
			{
				GameData.Level.ChipTick = 3;
				foreach(var creature in GameData.Map.CreaturesToProcess)
				{
					if(creature is not Player2)
					{
						continue;
					}

					creature.UpdateCreature();
				}
			}
		}
	}

	private bool CheckForEnding()
	{
		// TODO
		return false;
	}


	private void FloorMovements()
	{
		// TODO
	}


	private void UpdateSlipList()
	{
		// TODO
	}


	private void CreateClones()
	{
		// TODO
	}


	private void FinalHousekeeping()
	{
		// Nothing. It's literally nothing.
    }

    #endregion
}
