using GalaxyGauntlet.scripts;
using System.Reflection.Metadata.Ecma335;

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
					creature.TDir = creature.Forward;
				}
				else
				{
					creature.ChooseMove();
				}
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
		// TODO: Chip status
	}


	// FloorMovements_Of_Chip
	private void ChipSlipMovements()
	{
		var creaturesToProcess = GameData.Map.GetCreaturesToProcess();
		foreach(var creature in creaturesToProcess)
		{
			if(false == creature.IsInSliplist)
			{
				continue;
			}
			if(false == creature.IsPlayer)
			{
				continue;
			}
			Vector2I slipDirection = creature.SlipDirection;
			if(slipDirection == Vector2I.Zero)
			{
				continue;
            }
            // lastSlipDirection = slipDirection
            bool creatureAdvanced = creature.AdvanceCreature(slipDirection);
			if(creatureAdvanced)
			{
				creature.CreatureState &= ~(CreatureStateFlags.HasMoved);
			}
			else
			{
				var tile = GameData.Map.GetTile2(creature.Coordinate);
				if(tile is ForceFloorTile2)
                {
                    creature.CreatureState &= ~(CreatureStateFlags.HasMoved);
                }
				else if(tile is IceTile2 iceTile)
				{
					slipDirection = iceTile.SetCreatureDirection(slipDirection);
					// lastSlipDirection = slipDirection
					creatureAdvanced = creature.AdvanceCreature(slipDirection);
					if(creatureAdvanced)
                    {
                        creature.CreatureState &= ~(CreatureStateFlags.HasMoved);
                    }
				}
				else if(tile is BlueTeleporterTile2) // or is block
				{
					slipDirection *= -1;
					if(creature.AdvanceCreature(slipDirection))
                    {
                        creature.CreatureState &= ~(CreatureStateFlags.HasMoved);
                    }
				}
				if(creature.IsInSliplist)
				{
					tile = GameData.Map.GetTile2(creature.Coordinate);
					creature.EndFloorMovement();
					creature.StartFloorMovement(tile, Vector2I.Zero);
				}
			}
			if(CheckForEnding())
			{
				return;
			}
		}
	}


	// FloorMovements_Of_Blocks_And_Monsters
	private void CreatureSlipMovements()
    {
        var creaturesToProcess = GameData.Map.GetCreaturesToProcess();
		foreach(var creature in creaturesToProcess)
		{
			if(creature.IsPlayer)
			{
				continue;
			}
			if(false == creature.IsInSliplist)
			{
				continue;
			}
			var slipDirection = creature.SlipDirection.Clone();
			var originalDirection = creature.SlipDirection.Clone();
			if(creature.SlipDirection == Vector2I.Zero)
			{
				continue;
			}
			bool creatureAdvanced = creature.AdvanceCreature(creature.SlipDirection);
			if(false == creatureAdvanced)
			{
				var tile = GameData.Map.GetTile2(creature.Coordinate);
				if(tile is IceTile2 ice)
				{
					slipDirection = ice.SetCreatureDirection(slipDirection);
					creatureAdvanced = creature.AdvanceCreature(slipDirection);
				}
				if (creature.IsInSliplist)
				{
					creature.EndFloorMovement();
					Vector2I moveDirection = creatureAdvanced
						? Vector2I.Zero
						: originalDirection;
					creature.StartFloorMovement(tile, moveDirection);
				}
			}
			if(creature.HasState(CreatureStateFlags.Slipping) && creatureAdvanced)
			{
				creature.CreatureState |= CreatureStateFlags.Sliding;
			}
			if(CheckForEnding())
			{
				return;
			}
		}
    }


	private void UpdateSlipList()
	{
		var creaturesToProcess = GameData.Map.GetCreaturesToProcess();
		foreach(var creature in creaturesToProcess)
		{
			if(false == creature.IsInSliplist)
			{
				var tile = GameData.Map.GetTile2(creature.Coordinate);
				creature.EndFloorMovement();
			}
		}
	}


	private static void CreateClones()
    {
		for(int x = 0; x < GameData.Map.Dimensions.X; x++)
		{
			for(int y = 0; y < GameData.Map.Dimensions.Y; y++)
			{
				Vector2I mapCoordinate = new(x, y);
				var creature = GameData.Map.GetCreature(mapCoordinate);
				if(creature is null)
				{
					continue;
				}

				creature.CreatureState &= ~(CreatureStateFlags.Cloning);
			}
		}
    }


	private void FinalHousekeeping()
	{
		// Nothing. It's literally nothing.
	}

	#endregion
}
