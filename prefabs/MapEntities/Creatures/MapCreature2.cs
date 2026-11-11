using GalaxyGauntlet.prefabs.MapEntities;
using GalaxyGauntlet.scripts;
using System;
using System.Collections.Generic;
using System.Linq;

public class CreatureStateFlags
{
	public const int Released = 0x01;
	public const int Cloning = 0x02;
	public const int HasMoved = 0x04;
	public const int Turning = 0x08;
	public const int Slipping = 0x10;
	public const int Sliding = 0x20;
	public const int DeferPush = 0x40;
	public const int Mutant = 0x80;
}

public static class CanMakeMoveFlags
{
	/// <summary>
	/// Automatically assumes that the creature can leave the 
	/// tile they are currently on.
	/// </summary>
	public const int NoLeaveCheck = 0x01;

	/// <summary>
	/// Causes blue walls and hidden walls to remain unexposed.
	/// </summary>
	public const int NoExposeWalls = 0x02;

	/// <summary>
	/// Creature will not be prevented from moving by another creature in the way.
	/// </summary>
	public const int CloneCantBlock = 0x04;

	/// <summary>
	/// Prevents the player from pushing any blocks.
	/// </summary>
	public const int NoPushing = 0x08;

	/// <summary>
	/// Indicates to block-pushing logic that the player is moving
	/// through a teleporter.
	/// </summary>
	public const int TeleportPush = 0x10;

	/// <summary>
	/// Causes bugs and walkers to walk directly into fire.
	/// </summary>
	public const int NoFireCheck = 0x20;

	/// <summary>
	/// Causes buttons pressed by pushed blocks to take effect immediately.
	/// </summary>
	public const int NoDeferButtons = 0x40;
}

public partial class MapCreature2(Vector2I coordinate) : MapObject2(coordinate)
{
	public int State = 0;

	public override bool RequiresDirectionByte => true;

	public override bool RequiresLowerLayerTile => true;

	/// <summary>
	/// Determines if this creature is of type Player
	/// </summary>
	public bool IsPlayer => (EntityID == GGConstants.ByteCodes.Entities.Player);

	public EntityOrientation Orientation = EntityOrientation.North;

	public Vector2I Dir { get; protected set; } = Vector2I.Zero;

	public Vector2I TDir { get; protected set; } = Vector2I.Zero;

	private Vector2I SlipDirection;

	public List<MapItem2> Inventory = [];

	public Vector2I Forward
	{
		get
		{
			return Orientation.ToVector();
		}
	}


	public override void _Ready()
	{
		base._Ready();
		ZIndex = 10;
	}

	#region Movement logic methods

	public virtual void ChooseMove()
	{
		if(HasState(CreatureStateFlags.Slipping))
		{
			TDir = Vector2I.Zero;
		}
		else
		{
			// ChooseCreatureMove
			Vector2I[] moveChoices = [
				Vector2I.Zero
				, Vector2I.Zero
				, Vector2I.Zero
				, Vector2I.Zero
			];
			TDir = Vector2I.Zero;

			#region Entity and Tick Checks

			if (EntityID == GGConstants.ByteCodes.Entities.DirtBlock)
			{
				return;
			}
			if((GameData.Level.CurrentTick & 2) != 0)
			{
				return;
			}
			if(EntityID == GGConstants.ByteCodes.Entities.Teeth
				|| EntityID == GGConstants.ByteCodes.Entities.Blob)
			{
				if((GameData.Level.CurrentTick & 4) != 0)
				{
					return;
				}
			}

			#endregion

			var currentTile = GameData.Map.GetTile2(Coordinate);

			if(HasState(CreatureStateFlags.Turning)) {
				State &= ~(CreatureStateFlags.Turning | CreatureStateFlags.HasMoved);
				UpdateCreature();
			}
			if(HasState(CreatureStateFlags.HasMoved))
			{
				// Stalled tank
				// TODO
			}

			if(HasState(CreatureStateFlags.Slipping) || HasState(CreatureStateFlags.Sliding))
			{
				return;
			}

			#region Set Choices array

			if (currentTile is CloneMachineTile2 || currentTile is TrapTile2)
			{
				// Behavior in cloning machine or in trap
				switch(EntityID)
				{
					case GGConstants.ByteCodes.Entities.BlueTank:
					case GGConstants.ByteCodes.Entities.Ball:
					case GGConstants.ByteCodes.Entities.Glider:
					case GGConstants.ByteCodes.Entities.Fireball:
					case GGConstants.ByteCodes.Entities.Walker:
						moveChoices[0] = Forward;
						break;

					case GGConstants.ByteCodes.Entities.Blob:
						moveChoices = [
							Forward
							, Left().ToVector()
							, Back().ToVector()
							, Right().ToVector()
						];
						Random.Shared.Shuffle(moveChoices);
						break;

					case GGConstants.ByteCodes.Entities.Bug:
					case GGConstants.ByteCodes.Entities.Paramecium:
					case GGConstants.ByteCodes.Entities.Teeth:
						// TODO
						return;

					default:
						FileLogger.QuietLogMessage($"Unknown creature 0x{EntityID:X2} trying to move");
						return;
				}
			}
			else
			{
				// Normal behavior
				switch(EntityID)
				{
					case GGConstants.ByteCodes.Entities.BlueTank:
						moveChoices[0] = Forward;
						break;

					case GGConstants.ByteCodes.Entities.Ball:
						moveChoices[0] = Forward;
						moveChoices[1] = Back().ToVector();
						break;

					case GGConstants.ByteCodes.Entities.Glider:
						moveChoices = [
							Forward
							, Left().ToVector()
							, Right().ToVector()
							, Back().ToVector()
						];
						break;

					case GGConstants.ByteCodes.Entities.Fireball:
						moveChoices = [
							Forward
							, Right().ToVector()
							, Left().ToVector()
							, Back().ToVector()
						];
						break;

					case GGConstants.ByteCodes.Entities.Walker:
						moveChoices = [
							Forward
							, Left().ToVector()
							, Back().ToVector()
							, Right().ToVector()
						];
						Random.Shared.Shuffle(moveChoices);
						break;

					case GGConstants.ByteCodes.Entities.Blob:
						moveChoices = [
							Forward
							, Left().ToVector()
							, Back().ToVector()
							, Right().ToVector()
						];
						Random.Shared.Shuffle(moveChoices);
						break;

					case GGConstants.ByteCodes.Entities.Bug:
						moveChoices = [
							Left().ToVector()
							, Forward
							, Right().ToVector()
							, Back().ToVector()
						];
						break;

					case GGConstants.ByteCodes.Entities.Paramecium:
						moveChoices = [
							Right().ToVector()
							, Forward
							, Left().ToVector()
							, Back().ToVector()
						];
						break;

					case GGConstants.ByteCodes.Entities.Teeth:
						var chipPosition = GameData.Level.PlayerCoordinate;
						// TODO: Determine Teeth movement
						break;

					default:
						FileLogger.QuietLogMessage($"Unknown creature 0x{EntityID:X2} trying to move");
						return;
				}
			}

			#endregion

			for(int i = 0; i < moveChoices.Length; i++)
			{
				if (moveChoices[i] == Vector2I.Zero)
				{
					break;
				}

				TDir = moveChoices[i];
				if(CanMakeMove(TDir, 0))
				{
					return;
				}
			}

			if(EntityID == GGConstants.ByteCodes.Entities.BlueTank)
			{
				if(HasState(CreatureStateFlags.Released)
					|| currentTile is not TrapTile2)
				{
					State |= CreatureStateFlags.HasMoved;
				}
				TDir = Vector2I.Zero;
			}
			else
			{
				// TODO
			}
		}
	}


	public virtual bool AdvanceCreature(Vector2I direction)
	{
		if(direction == Vector2I.Zero)
		{
			return true;
		}

		if (IsPlayer)
		{
			GameData.Level.ChipTick = 0;
		}

		if(false == StartMovement(direction))
		{
			if(IsPlayer)
			{
				// Can't move sound effect
			}
			return false;
		}

		EndMovement(direction);
		if(IsPlayer)
		{
			HandleButtons();
		}

		return true;
	}


	public bool StartMovement(Vector2I direction)
	{
		var currentTile = GameData.Map.GetTile2(Coordinate);
		if(false == CanMakeMove(direction, 0))
		{
			if(IsPlayer
				|| (currentTile is not TrapTile2
					&& currentTile is not CloneMachineTile2
					&& false == HasState(CreatureStateFlags.Slipping)))
			{
				if(false == IsPlayer
					|| direction != Vector2I.Zero)
				{
					Dir = direction;
				}
				UpdateCreature();
			}
			return false;
		}

		if(currentTile is TrapTile2)
		{
			// TODO
		}
		State &= ~(CreatureStateFlags.Released);
		Dir = direction;
		return true;
	}


	public void EndMovement(Vector2I direction)
	{
		var currentCoordinate = Coordinate.Clone();
		var destinationCoordinate = Coordinate + direction;

		var currentTile = GameData.Map.GetTile2(currentCoordinate);
		var destinationTile = GameData.Map.GetTile2(destinationCoordinate);
		var destinationItem = GameData.Map.GetItem2(destinationCoordinate);
		var destinationCreature = GameData.Map.GetCreature(destinationCoordinate);

		if (this is Player2 player)
		{
			switch (destinationTile.EntityID)
			{
				case GGConstants.ByteCodes.Entities.Water:
					if(false == HasItem(new FlippersItem2(Vector2I.Zero)))
					{
						player.PlayerStatus = PlayerStatus.Drowned;
					}
					break;

				case GGConstants.ByteCodes.Entities.Fire:
					if(false == HasItem(new FireBootsItem2(Vector2I.Zero)))
					{
						player.PlayerStatus = PlayerStatus.Burned;
					}
					break;

				case GGConstants.ByteCodes.Entities.Dirt:
				case GGConstants.ByteCodes.Entities.FakeBlueWall:
					GameData.Map.SetTile(new FloorTile2(destinationCoordinate));
					break;

				case GGConstants.ByteCodes.Entities.RecessedWall:
					GameData.Map.SetTile(new WallTile2(destinationCoordinate));
					break;

				case GGConstants.ByteCodes.Entities.RedDoor:
				case GGConstants.ByteCodes.Entities.BlueDoor:
				case GGConstants.ByteCodes.Entities.YellowDoor:
				case GGConstants.ByteCodes.Entities.GreenDoor:
					// Assume open check has already happened
					GameData.Map.SetTile(new FloorTile2(destinationCoordinate));
					break;

				case GGConstants.ByteCodes.Entities.ToolThief:
					StealTools();
					break;

				case GGConstants.ByteCodes.Entities.KeyThief:
					StealKeys();
					break;

				case GGConstants.ByteCodes.Entities.Socket:
					if(false == GameData.Level.ChipRequirementMet)
					{
						return;
					}

					GameData.Map.SetTile(new FloorTile2(destinationCoordinate));
					break;
			}
			if (destinationItem is not null)
			{
				switch (destinationItem.EntityID)
				{
					case GGConstants.ByteCodes.Entities.IceSkates:
					case GGConstants.ByteCodes.Entities.FireBoots:
					case GGConstants.ByteCodes.Entities.SuctionBoots:
					case GGConstants.ByteCodes.Entities.Flippers:
					case GGConstants.ByteCodes.Entities.RedKey:
					case GGConstants.ByteCodes.Entities.BlueKey:
					case GGConstants.ByteCodes.Entities.YellowKey:
					case GGConstants.ByteCodes.Entities.GreenKey:
						PickUpItem(destinationItem);
						break;

					case GGConstants.ByteCodes.Entities.Chip:
					case GGConstants.ByteCodes.Entities.ExtraChip:
						var chipItem = (ChipItem2)destinationItem;
						GameData.OnChipCollected(chipItem);
						break;
				}
			}
			if(destinationCreature is not null)
			{
				switch(destinationCreature.EntityID)
				{
					case GGConstants.ByteCodes.Entities.RedBomb:
						player.PlayerStatus = PlayerStatus.Bombed;
						break;
				}

				player.PlayerStatus = PlayerStatus.Collided;
			}
		}
		else if(EntityID == GGConstants.ByteCodes.Entities.DirtBlock)
		{
			if(destinationTile is WaterTile2)
			{
				PlaySoundEffect("Splash");
				GameData.Map.SetTile(new DirtTile2(destinationCoordinate));
				Kill();
			}
			if(destinationCreature is RedBombCreature2)
			{
				destinationCreature.PlaySoundEffect("Explosion");
				destinationCreature.Kill();
				Kill();
			}
			// TODO: Teleporter
		}
		SetCoordinateAndOrientation(destinationCoordinate);
		if(IsDead)
		{
			GameData.Map.RemoveCreature(Coordinate);
		}
	}


	// Updates the creature's display on the grid. Is this even needed?
	public virtual void UpdateCreature()
	{
		/*
		var currentTile = GameData.Map.GetTile2(Coordinate);
		if(EntityID == GGConstants.ByteCodes.Entities.DirtBlock)
		{
			return;
		}
		if(IsPlayer)
		{
			// TODO
		}

		Vector2I direction = Dir;
		if(HasState(CreatureStateFlags.Turning))
		{
			Orientation = Right();
		}
		// TODO
		*/
	}


	public virtual bool CanMakeMove(Vector2I direction, int flags)
	{
		var destinationCoordinate = Coordinate + direction;
		SetOrientation(direction);
		if(false == GameData.Map.IsWithinDimensions(destinationCoordinate))
		{
			return false;
		}

		var destinationTile = GameData.Map.GetTile2(destinationCoordinate);
		var destinationCreature = GameData.Map.GetCreature(destinationCoordinate);
		var destinationItem = GameData.Map.GetItem2(destinationCoordinate);
		if(destinationTile is CloneMachineTile2)
		{
			return false;
		}

		// TODO: Check for thin wall exit

		if(this is Player2 player)
		{
			#region Entry Law checks

			// Tile
			var entryLaw = EntryLaws.Laws.GetValueOrDefault(destinationTile.EntityID);
			if (entryLaw is null)
			{
				GD.Print($"Entry law for entity {destinationTile.MapObjectName} not found.");
			}
			else
			{
				if (false == entryLaw.CanPlayerEnter(direction))
				{
					return false;
				}
			}

			#endregion

			#region Socket Check

			if (destinationTile.EntityID == GGConstants.ByteCodes.Entities.Socket)
			{
				if (false == GameData.Level.ChipRequirementMet)
				{
					return false;
				}
			}

			#endregion

			if(destinationTile is KeyDoorTile2 keyDoor)
			{
				if(false == keyDoor.CanOpen(this))
				{
					return false;
				}
				keyDoor.PlaySoundEffect("Door_Open");
			}

			// TODO: Hidden/Blue wall check

			if(destinationCreature is DirtBlockCreature2 dirtBlock)
			{
				if(false == dirtBlock.CanPushBlock(direction, flags))
				{
					return false;
				}
				else if ((flags & CanMakeMoveFlags.NoPushing) != 0)
				{
					return false;
				}
				if (destinationTile is CloneMachineTile2)
				{
					return false;
				}
				// TODO: Teleporter check
				return CanMakeMove(direction, flags | CanMakeMoveFlags.NoPushing);
			}

			if(destinationCreature is not null)
			{
				if(HasItem(new HelmetItem2(Vector2I.Zero)))
				{
					return false;
				}
				player.PlayerStatus = PlayerStatus.Collided;
			}
		}
		else if(EntityID == GGConstants.ByteCodes.Entities.DirtBlock)
		{
			if(destinationCreature is not null)
			{
				if(destinationCreature is not Player2)
				{
					return false;
				}
			}

			#region Entry Law checks

			// Tile
			var entryLaw = EntryLaws.Laws.GetValueOrDefault(destinationTile.EntityID);
			if (entryLaw is null)
			{
				GD.Print($"Entry law for tile {destinationTile.MapObjectName} not found.");
			}
			else
			{
				if (false == entryLaw.CanCreatureEnter(direction))
				{
					return false;
				}
			}

			// Item
			if (destinationItem is not null)
			{
				entryLaw = EntryLaws.Laws.GetValueOrDefault(destinationItem.EntityID);
				if (entryLaw is null)
				{
					GD.Print($"Entry law for item {destinationItem.MapObjectName} not found.");
				}
				else
				{
					if (false == entryLaw.CanCreatureEnter(direction))
					{
						return false;
					}
				}
			}

			// Creature
			if (destinationCreature is not null)
			{
				entryLaw = EntryLaws.Laws.GetValueOrDefault(destinationCreature.EntityID);
				if (entryLaw is null)
				{
					GD.Print($"Entry law for creature {destinationCreature.MapObjectName} not found.");
				}
				else
				{
					if (false == entryLaw.CanCreatureEnter(direction))
					{
						return false;
					}
				}
			}

			#endregion
		}
		else
		{
			if(destinationCreature is not null)
			{
				return (destinationCreature is Player2);
			}

			#region Entry Law checks

			// Tile
			var entryLaw = EntryLaws.Laws.GetValueOrDefault(destinationTile.EntityID);
			if (entryLaw is null)
			{
				GD.Print($"Entry law for tile {destinationTile.MapObjectName} not found.");
			}
			else
			{
				if (false == entryLaw.CanCreatureEnter(direction))
				{
					return false;
				}
			}

			// Item
			if(destinationItem is not null)
			{
				entryLaw = EntryLaws.Laws.GetValueOrDefault(destinationItem.EntityID);
				if (entryLaw is null)
				{
					GD.Print($"Entry law for item {destinationItem.MapObjectName} not found.");
				}
				else
				{
					if (false == entryLaw.CanCreatureEnter(direction))
					{
						return false;
					}
				}
			}

			// Creature
			if (destinationCreature is not null)
			{
				entryLaw = EntryLaws.Laws.GetValueOrDefault(destinationCreature.EntityID);
				if (entryLaw is null)
				{
					GD.Print($"Entry law for creature {destinationCreature.MapObjectName} not found.");
				}
				else
				{
					if (false == entryLaw.CanCreatureEnter(direction))
					{
						return false;
					}
				}
			}

			#endregion
		}

		return true;
	}


	private void HandleButtons()
	{

	}


	public bool HasState(int state)
	{
		return (State & state) != 0;
	}


	public void SetCoordinateAndOrientation(Vector2I newCoordinate)
	{
		if (Coordinate == newCoordinate)
		{
			return;
		}

		var direction = newCoordinate - Coordinate;

		// Update creature position
		GameData.Map.RemoveCreature(Coordinate);
		Coordinate = newCoordinate;
		GameData.Map.Creatures2[newCoordinate.X, newCoordinate.Y] = this;
		if(IsPlayer)
		{
			GameData.Level.PlayerCoordinate = newCoordinate;
		}

		SetOrientation(direction);
	}


	public void SetOrientation(Vector2I direction)
	{
		if (direction == new Vector2I(0, -1))
		{
			Orientation = EntityOrientation.North;
		}
		if (direction == new Vector2I(0, 1))
		{
			Orientation = EntityOrientation.South;
		}
		if (direction == new Vector2I(1, 0))
		{
			Orientation = EntityOrientation.East;
		}
		if (direction == new Vector2I(-1, 0))
		{
			Orientation = EntityOrientation.West;
		}
	}

	#endregion 


	#region Orientation Methods

	public EntityOrientation Right()
	{
		return Orientation switch
		{
			EntityOrientation.North => EntityOrientation.East,
			EntityOrientation.East => EntityOrientation.South,
			EntityOrientation.South => EntityOrientation.West,
			EntityOrientation.West => EntityOrientation.North,
			_ => EntityOrientation.North
		};
	}


	public EntityOrientation Left()
	{
		return Orientation switch
		{
			EntityOrientation.North => EntityOrientation.West,
			EntityOrientation.East => EntityOrientation.North,
			EntityOrientation.South => EntityOrientation.East,
			EntityOrientation.West => EntityOrientation.South,
			_ => EntityOrientation.North
		};
	}


	public EntityOrientation Back()
	{
		return Orientation switch
		{
			EntityOrientation.North => EntityOrientation.South,
			EntityOrientation.East => EntityOrientation.West,
			EntityOrientation.South => EntityOrientation.North,
			EntityOrientation.West => EntityOrientation.East,
			_ => EntityOrientation.North
		};
	}

	#endregion


	#region Inventory Methods

	public void PickUpItem(MapItem2 item)
	{
		Inventory.Add(item);
		GameData.Map.RemoveItem2(item.Coordinate);
	}


	public bool UseItem(MapItem2 item)
	{
		int index = Inventory.FindIndex(e => e.EntityID == item.EntityID);
		if(index == -1)
		{
			return false;
		}

		Inventory.RemoveAt(index);
		return true;
	}


	public bool HasItem(MapItem2 item)
	{
		return Inventory.FindIndex(e => e.EntityID == item.EntityID) > -1;
	}


	public void StealKeys()
	{
		if (UseItem(new BribeItem2(Vector2I.Zero)))
		{
			return;
		}

		List<MapItem2> newInventory = [];
		foreach(var item in Inventory)
		{
			if(item is not KeyItem2)
			{
				newInventory.Add(item);
			}
		}
		Inventory = newInventory;
	}


	public void StealTools()
	{
		if(UseItem(new BribeItem2(Vector2I.Zero)))
		{
			return;
		}

		List<MapItem2> newInventory = [];
		foreach (var item in Inventory)
		{
			if (item is KeyItem2)
			{
				newInventory.Add(item);
			}
		}
		Inventory = newInventory;
	}

	#endregion
}
