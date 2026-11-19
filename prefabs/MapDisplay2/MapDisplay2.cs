using GalaxyGauntlet.scripts;

public partial class MapDisplay2 : Node2D
{
	private Vector2I PutWall = new(-1, -1);

	#region Godot Overrides

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

		int gameTick = GameData.Level.CurrentTick;
		InitialHousekeeping();
		GameData.Level.CurrentTick++;

		#region Choose moves for creatures

		var creaturesToProcess = GameData.Map.GetLynxCreaturesToProcess();
		for(int i = creaturesToProcess.Count - 1; i >= 0; i--)
		{
			var creature = creaturesToProcess[i];
			if(creature.IsHidden 
				&& (creature is not LynxPlayer2 player || false == player.IsRealPlayer))
			{
				continue;
			}

			// TODO: Animation check

			if(creature.IsThePlayer && InEndGame())
			{
				continue;
			}
			if(creature.Moving <= 0)
			{
				creature.ChooseMove();
			}
		}

		#endregion

		#region Check Chip movement

		creaturesToProcess = GameData.Map.GetLynxCreaturesToProcess();
		foreach(var creature in creaturesToProcess)
		{
			if(creature is not LynxPlayer2 player || false == player.IsRealPlayer)
			{
				continue;
			}

			if(creature.GetForceDirection() == Vector2I.Zero && creature.TDir == Vector2I.Zero)
			{
				player.CouldntMove = false;
			}
			else
			{
				player.CheckMovingTo();
			}
			break;
		}

		#endregion


		#region Advance Creatures

		creaturesToProcess = GameData.Map.GetLynxCreaturesToProcess();
		for (int i = creaturesToProcess.Count - 1; i >= 0; i--)
		{
			var creature = creaturesToProcess[i];
			if(creature.IsThePlayer && Completed())
			{
				continue;
			}
			if(creature is not LynxPlayer2 && creature.IsHidden)
			{
				continue;
			}
			if(0 > creature.AdvanceCreature(false))
			{
				continue;
			}
			creature.TDir = Vector2I.Zero;
			creature.SetForceDirection(Vector2I.Zero);
			var currentTile = GameData.Map.GetTile2(creature.Coordinate);
			if(GameData.LynxPedantic && currentTile is RecessedWallTile2)
			{
				if(false == creature.IsThePlayer)
				{
					PutWall = GameData.Level.PlayerCoordinate;
				}
			}
			if(currentTile is BrownButtonTile2 && creature.Moving <= 0)
			{
				GameData.SpringTrap(creature.Coordinate);
			}
		}

		#endregion

		#region Teleporter Logic

		creaturesToProcess = GameData.Map.GetLynxCreaturesToProcess();
		for(int i = creaturesToProcess.Count - 1; i >= 0; i--)
		{
			var creature = creaturesToProcess[i];
			if(creature.IsHidden)
			{
				continue;
			}
			if(creature.Moving != 0)
			{
				continue;
			}
			var currentTile = GameData.Map.GetTile2(creature.Coordinate);
			if(currentTile is BlueTeleporterTile2)
			{
				creature.BlueTeleport();
			}
		}

		#endregion

		if(PutWall.X != -1)
		{
			// TODO
			GD.Print($"Setting wall at {PutWall}");
			GameData.Map.SetTile(new WallTile2(PutWall));
			PutWall = new(-1, -1);
		}

		FinalHousekeeping();
	}

	#endregion

	public void ClearChildren()
	{
		foreach (var child in GetChildren())
		{
			RemoveChild(child);
			child.QueueFree();
		}
	}

	private void InitialHousekeeping()
	{
		// TODO: Stepping logic

		// TODO: Chip pushing logic

		// TODO: Timer check?

		var creaturesToProcess = GameData.Map.GetLynxCreaturesToProcess();

		#region Reverse creatures

		foreach (var creature in creaturesToProcess)
		{
			if(false == creature.IsThePlayer && creature.IsHidden)
			{
				continue;
			}
			if(creature.HasCreatureState(CreatureStateFlags.Reverse))
			{
				creature.RemoveCreatureState(CreatureStateFlags.Reverse);
				if(creature.Moving <= 0)
				{
					creature.AboutFace();
				}
			}
		}

		#endregion


		#region Push creatures

		creaturesToProcess = GameData.Map.GetLynxCreaturesToProcess();
		foreach(var creature in creaturesToProcess)
		{
			if(creature.HasCreatureState(CreatureStateFlags.Pushed))
			{
				if(creature.IsHidden || creature.Moving <= 0)
				{
					creature.RemoveCreatureState(CreatureStateFlags.Pushed);
				}
			}
		}

		#endregion

		// Green wall toggle is handled at the creature level

		LynxPlayer2.ChipToPosition = new(-1, -1);
		LynxPlayer2.ChipToCreature = null;
	}


	private bool InEndGame()
	{
		// TODO
		return false;
	}


	private void FinalHousekeeping()
	{
		// Nothing. It's literally nothing.
	}


	private bool Completed()
	{
		// TODO
		return false;
	}
}
