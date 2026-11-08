using GalaxyGauntlet.prefabs.MapEntities.MapObject;
using GalaxyGauntlet.scripts;
using System;
using System.Collections.Generic;
using System.Linq;

public class CreatureState
{
	public const uint Released = 1;
	public const uint Cloning = 2;
	public const uint HasMoved = 4;
	public const uint Turning = 8;
	public const uint Slipping = 16;
	public const uint Sliding = 32;
	public const uint DeferPush = 64;
	public const uint Mutant = 128;
}

public partial class MapCreature2(Vector2I coordinate) : MapObject(coordinate)
{
	public override bool RequiresLowerLayerTile => true;
	
	public override bool RequiresDirectionByte => true;

	public uint State = 0;
	
	public List<MapItem2> Inventory = [];

	public EntityOrientation Orientation = EntityOrientation.North;

	public Vector2I TDir { get; protected set; } = Vector2I.Zero;


	public override void _Ready()
	{
		base._Ready();
		ZIndex = 10;
	}


	public override void _Process(double delta)
	{
		base._Process(delta);
	}


	#region Inventory Methods

	public bool ConsumeItem(int byteCode)
	{
		if(false == HasItem(byteCode))
		{
			return false;
		}

		var index = Inventory.FindIndex(e => e.ByteCode == byteCode);
		Inventory.RemoveAt(index);
		return true;
	}


	public bool HasItem(int byteCode)
	{
		return Inventory.Any(e => e.ByteCode == byteCode);
	}


	public void StealBoots()
	{
		Inventory = Inventory.Where(e => e is not ShoeItem2).ToList();
	}

	#endregion


	public virtual void UpdateCreature()
	{
		if(this is DirtBlockCreature2)
		{
			if((State & CreatureState.Mutant) == CreatureState.Mutant)
			{
				// TODO: Turn into Chip?
			}
		}
		else if (this is Player2)
		{
			// Update Chip sprite based on death status
		}
		if((State & CreatureState.Turning) == CreatureState.Turning)
		{
			TurnRight();
		}
	}


	public virtual void ChooseMove()
	{
		if((State & CreatureState.Slipping) == CreatureState.Slipping)
		{
			TDir = Vector2I.Zero;
			return;
		}

		#region Choose Creature Move
		// TODO: ChooseCreatureMove
		#endregion
	}


	public virtual bool AdvanceCreature(Vector2I direction)
	{
		if(direction == Vector2I.Zero)
		{
			return true;
		}

		if(this is Player2)
		{
			GameData.Level.ChipWaitTick = 0;
		}

		if(false == StartMovement(direction))
		{
			if(this is Player2)
			{
				GD.Print("Start movement failed");
				// TODO
			}
			return false;
		}
		EndMovement(direction);
		if(this is Player2)
		{
			GameData.Map.HandleButtons();
		}
		return true;
	}


	#region Movement Methods

	public bool StartMovement(Vector2I direction)
	{
		var currentTile = GameData.Map.GetTile2(Coordinate);
		if(false == CanMakeMove(direction))
		{
			if(this is Player2
				|| ((State & CreatureState.Slipping) == 0 
					&& currentTile is not TrapTile2
					&& currentTile is CloneMachineTile2))
			{
				UpdateCreature();
			}
			return false;
		}

		return true;
	}


	public void EndMovement(Vector2I direction)
	{
		var destinationCoordinate = Coordinate + direction;
		var destinationTile = GameData.Map.GetTile2(destinationCoordinate);
		var destinationItem = GameData.Map.GetItem2(destinationCoordinate);
		var destinationCreature = GameData.Map.GetCreature2(destinationCoordinate);

		// Player logic
		if(this is Player2 player)
		{
			if(destinationTile is WaterTile2 && false == HasItem(GGConstants.ByteCodes.Entities.Flippers))
			{
				GameData.Level.CurrentPlayerState = PlayerState.Drowned;
			}
			if(destinationTile is FireTile2 && false == HasItem(GGConstants.ByteCodes.Entities.FireBoots))
			{
				GameData.Level.CurrentPlayerState = PlayerState.Burned;
			}
			if(destinationTile is DirtTile2)
			{
				GameData.Map.SetTile2(new FloorTile2(destinationCoordinate));
			}
			if(destinationTile is BlueWallTile2 blueWall && false == blueWall.IsReal)
			{
				GameData.Map.SetTile2(new FloorTile2(destinationCoordinate));
			}
			if(destinationTile is RecessedWallTile2)
			{
				GameData.Map.SetTile2(new WallTile2(destinationCoordinate));
			}
			if(destinationTile is KeyDoorTile2 keyDoor)
			{
				if (player.CanOpenDoor(destinationTile.ByteCode))
				{
					destinationTile.Kill();
					GameData.Map.SetTile2(new FloorTile2(destinationCoordinate));
				}
			}
			if(destinationTile is ToolBurglarTile2)
			{
				StealBoots();
			}
			if(destinationTile is SocketTile2)
			{
				if(GameData.Level.ChipRequirementMet)
				{
					GameData.Map.SetTile2(new FloorTile2(destinationCoordinate));
				}
			}
			if(destinationItem is ShoeItem2
				|| destinationItem is KeyItem2)
			{
				if(destinationCreature is not null)
				{
					GameData.Level.CurrentPlayerState = PlayerState.Collided;
				}
				Inventory.Add(destinationItem);
				GameData.Map.RemoveItem2(destinationCoordinate);
			}
			if(destinationItem is ChipItem2)
			{
				GameData.Map.RemoveItem2(destinationCoordinate);
				GameData.Level.ChipsCollected++;
			}
			if(destinationCreature is RedBombCreature2)
			{
				GameData.Level.CurrentPlayerState = PlayerState.Bombed;
			}
		}
		
		// Dirt Block logic
		else if(this is DirtBlockCreature2 dirtBlock)
		{
			if(destinationTile is WaterTile2)
			{
				GameData.Map.SetTile2(new DirtTile2(destinationCoordinate));
				Kill();
			}
			if(destinationCreature is RedBombCreature2)
			{
				Kill();
				destinationCreature.Kill();
			}
		}

		if(this is Player2)
		{
			GameData.Level.PlayerCoordinate = destinationCoordinate;
			GD.Print($"Player coordinate updated to {destinationCoordinate}");
		}
		GameData.Map.UpdateCreature(coordinate, destinationCoordinate);
		Coordinate = destinationCoordinate;
	}


	public void StartFloorMovement()
	{
		var currentTile = GameData.Map.GetTile2(Coordinate);
		State &= ~(CreatureState.Slipping | CreatureState.Sliding);
		if(currentTile is IceTile2 iceTile)
		{
			Orientation = iceTile.GetNewCreatureDirection(Orientation);
		}
		else if (currentTile is ForceFloorTile2 forceFloor)
		{
			Orientation = forceFloor.GetInfluenceDirection();
		}
		// TODO: else if teleport
		// TODO: else if trap
		else if(this is not Player2)
		{
			return;
		}

		if(this is Player2)
		{
			// TODO
		}
		else
		{
			State |= CreatureState.Slipping;
		}
	}


	public void EndFloorMovement()
	{
		State &= ~(CreatureState.Slipping | CreatureState.Sliding);
	}

	#endregion


	#region Orientation Methods

	public void TurnRight()
	{
		Orientation = Orientation switch
		{
			EntityOrientation.North => EntityOrientation.East,
			EntityOrientation.East => EntityOrientation.South,
			EntityOrientation.South => EntityOrientation.West,
			EntityOrientation.West => EntityOrientation.North,
			_ => EntityOrientation.North
		};
	}


	public void TurnLeft()
	{
		Orientation = Orientation switch
		{
			EntityOrientation.North => EntityOrientation.West,
			EntityOrientation.East => EntityOrientation.North,
			EntityOrientation.South => EntityOrientation.East,
			EntityOrientation.West => EntityOrientation.South,
			_ => EntityOrientation.North
		};
	}


	public void AboutFace()
	{
		Orientation = Orientation switch
		{
			EntityOrientation.North => EntityOrientation.South,
			EntityOrientation.East => EntityOrientation.West,
			EntityOrientation.South => EntityOrientation.North,
			EntityOrientation.West => EntityOrientation.East,
			_ => EntityOrientation.North
		};
	}

	#endregion


	public virtual bool CanMakeMove(Vector2I direction)
	{
		// Validate creature can escape Thin Wall
		var thinWall = GameData.Map.GetThinWall(Coordinate);
		if(thinWall is not null)
		{
			if(false == thinWall.CanLeave(direction))
			{
				GD.Print("Cannot leave thin wall");
				return false;
			}
		}

		// Validate creature is released from trap
		var currentTile = GameData.Map.GetTile2(Coordinate);
		if(currentTile is TrapTile2 trapTile)
		{
			if((State & CreatureState.Released) == 0)
			{
				GD.Print("Not released from trap");
				return false;
			}
		}

		Vector2I destinationCoordinate = Coordinate + direction;
		var destinationThinWall = GameData.Map.GetThinWall(destinationCoordinate);
		var destinationTile = GameData.Map.GetTile2(destinationCoordinate);
		var destinationCreature = GameData.Map.GetCreature2(destinationCoordinate);
		if(destinationTile is CloneMachineTile2)
		{
			GD.Print("Creature is in cloning machine");
			return false;
		}

		// Chip logic
		if(this is Player2 player)
		{
			var entryLaw = EntryLaws.Laws.GetValueOrDefault(destinationTile.ByteCode) 
				?? throw new Exception($"Entry law not defined for entity 0x{destinationTile.ByteCode:x2}");
			if (false == entryLaw.CanPlayerEnter(direction))
			{
				GD.Print("Does not satisfy entry law");
				return false;
			}
			if(destinationTile is SocketTile2 && false == GameData.Level.ChipRequirementMet)
			{
				GD.Print("Cannot open socket");
				return false;
			}
			if(destinationTile is KeyDoorTile2 keyDoor 
				&& false == player.CanOpenDoor(keyDoor.ByteCode))
			{
				GD.Print("Cannot open door");
				return false;
			}
			if (destinationCreature is not null)
			{
				if(destinationCreature is Player2)
				{
					GD.Print("Destination is player");
					return false;
				}
			}
			if(destinationTile is HiddenWallTile2
				|| (destinationTile is BlueWallTile2 blueWall && blueWall.IsReal))
			{
				GameData.Map.SetTile2(new WallTile2(destinationCoordinate));
				GD.Print("Entered fake wall");
				return false;
			}
			if(destinationCreature is DirtBlockCreature2 dirtBlock)
			{

			}
		}
		else if(this is DirtBlockCreature2)
		{
			if(destinationCreature is not null)
			{
				return (destinationCreature is Player2);
			}
			var entryLaw = EntryLaws.Laws.GetValueOrDefault(destinationTile.ByteCode)
				?? throw new Exception($"Entry law not defined for entity 0x{destinationTile.ByteCode:x2}");
			if (false == entryLaw.CanBlockEnter(direction))
			{
				return false;
			}
		}
		else
		{

		}

		GD.Print("CanMakeMove success");
		return true;
	}
}
