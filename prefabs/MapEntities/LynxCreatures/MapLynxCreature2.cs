using GalaxyGauntlet.prefabs.MapEntities;
using GalaxyGauntlet.scripts;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class MapLynxCreature2(Vector2I coordinate) : MapObject2(coordinate)
{
	public static int CreatureOfFocus = -1;

	public override bool RequiresDirectionByte => true;

	public override bool RequiresLowerLayerTile => true;

	public int CreatureID = GameData.Map.GetNextCreatureID();

	public int Moving = 0;

	public bool IsHidden = false;

	public EntityOrientation Orientation = EntityOrientation.North;

	public int CreatureState;

	public List<MapItem2> Inventory = [];

	/// <summary>
	/// Indicates if this creature is the instance of Chip that is controlled by the player.
	/// </summary>
	public bool IsThePlayer
	{
		get
		{
			return this is LynxPlayer2 lynxPlayer && lynxPlayer.IsRealPlayer;
		}
	}


	/*
	 * 1: North
	 * 2: West?
	 * 4: South
	 * 8: East?
	 */
	private int ForceDirection
	{
		get // getfdir
		{
			return CreatureState & CreatureStateFlags.ForceDirection;
		}
		set // setfdir
		{
			CreatureState = (CreatureState & ~(CreatureStateFlags.ForceDirection))
				| (value & CreatureStateFlags.ForceDirection);
		}
	}

	public Vector2I Forward
	{
		get
		{
			return Orientation.ToVector();
		}
	}

	public Vector2I TDir = Vector2I.Zero;


	#region Child Nodes

	private Button FocusButton;

	#endregion


	#region Godot Overrides

	public override void _Ready()
	{
		base._Ready();
		ZIndex = GGConstants.ZIndexes.Creature;

		FocusButton = new()
		{
			Position = -16 * Vector2.One,
			Size = 32 * Vector2.One,
			Modulate = Godot.Color.Color8(0, 0, 0, 0)
		};
		FocusButton.Pressed += () => {
			GD.Print($"Logging events from creature {CreatureID}");
			CreatureOfFocus = CreatureID;
		};
		AddChild(FocusButton);
	}

	#endregion


	#region Creature State Methods

	public bool HasCreatureState(int stateValue)
	{
		return (CreatureState & stateValue) == stateValue;
	}


	public void RemoveCreatureState(int stateValue)
	{
		CreatureState &= ~(stateValue);
	}


	public void SetCreatureState(int stateValue)
	{
		CreatureState |= stateValue;
	}

	#endregion


	#region Inventory Methods

	public bool HasItem(int entityID)
	{
		return Inventory.Any(e => e.EntityID == entityID);
	}


	public bool ConsumeItem(int entityID)
	{
		int index = Inventory.FindIndex(e => e.EntityID == entityID);
		if(index == -1)
		{
			return false;
		}

		Inventory.RemoveAt(index);
		return true;
	}


	public void PickUpItem(MapItem2 item)
	{
		if(false == GameData.Archipelago.IsItemUnlocked(item.EntityID))
		{
			return;
		}

		GameData.Map.RemoveItem2(item.Coordinate);
		Inventory.Add(item);
		if(IsThePlayer)
		{
			GameData.Level.UpdatePlayerItems(Inventory);
		}
	}


	public void StealKeys()
	{
		if(ConsumeItem(GGConstants.ByteCodes.Entities.Bribe))
		{
			return;
		}

		Inventory = Inventory.Where(e => e is not KeyItem2).ToList();
		GameData.Level.UpdatePlayerItems(Inventory);
	}


	public void StealTools()
	{
		if (ConsumeItem(GGConstants.ByteCodes.Entities.Bribe))
		{
			return;
		}

		Inventory = Inventory.Where(e => e is KeyItem2).ToList();
		GameData.Level.UpdatePlayerItems(Inventory);
	}

	#endregion


	#region Choose Move Methods

	public bool ChooseMove()
	{
		if(IsThePlayer)
		{
			ChooseChipMove(GetForcedMove());
			if(TDir == Vector2I.Zero && ForceDirection == 0)
			{
				// ResetFloorSounds(false);
			}
		}
		else
		{
			if(GetForcedMove())
			{
				TDir = Vector2I.Zero;
			}
			else
			{
				ChooseCreatureMove();
			}
		}

		return (TDir != Vector2I.Zero || GetForceDirection() != Vector2I.Zero);
	}


	public void ChooseChipMove(bool discard)
	{
		LynxPlayer2.ChipPushing = false;
		Vector2I dir = GameData.GetPlayerInput();

		if(dir == Vector2I.Zero || discard || LynxPlayer2.IsStuck)
		{
			TDir = Vector2I.Zero;
			return;
		}

		FocusCreatureLog($"Player input: {dir}");
		TDir = dir;

		if(TDir != Vector2I.Zero)
		{
			dir = TDir;
		}
		else if(GetForceDirection() != Vector2I.Zero)
		{
			dir = GetForceDirection();
		}
		else
		{
			return;
		}

		if(dir.IsDiagonal())
		{
			if(Forward.X == dir.X || Forward.Y == dir.Y)
			{
				bool makeMove1 = CanMakeMove(Forward, CanMakeMoveFlags.PushBlocks);
				bool makeMove2 = CanMakeMove(Forward.XOR(dir), CanMakeMoveFlags.PushBlocks);
				dir = (!makeMove1 && makeMove2)
					? dir.XOR(Forward)
					: Forward;
			}
			else
			{
				Vector2I testDir = new(dir.X, 0);
				if(CanMakeMove(testDir, CanMakeMoveFlags.PushBlocks))
				{
					dir = testDir;
				}
				else
				{
					dir = new(0, dir.Y);
				}
			}
			TDir = dir;
		}
		else if(CanMakeMove(dir, CanMakeMoveFlags.PushBlocks))
		{
			TDir = dir;
		}
	}


	public void ChooseCreatureMove()
	{
		TDir = Vector2I.Zero;
		if(this is LynxDirtBlockCreature2)
		{
			return;
		}

		if(GetForceDirection() != Vector2I.Zero)
		{
			return;
		}
		var currentTile = GameData.Map.GetTile2(Coordinate);
		if(currentTile is CloneMachineTile2 || currentTile is TrapTile2)
		{
			TDir = Forward;
			return;
		}

		Vector2I pDir = Vector2I.Zero;

		#region Define move choices

		Vector2I[] moveChoices = [
			Forward
			, Vector2I.Zero
			, Vector2I.Zero
			, Vector2I.Zero
		];
		switch(EntityID)
		{
			case GGConstants.ByteCodes.Entities.RedBomb:
				moveChoices = [];
				break;

			case GGConstants.ByteCodes.Entities.BlueTank:
				break;

			case GGConstants.ByteCodes.Entities.Ball:
				moveChoices[1] = -Forward;
				break;

			case GGConstants.ByteCodes.Entities.Glider:
				moveChoices[1] = Left().ToVector();
				moveChoices[2] = Right().ToVector();
				moveChoices[3] = -Forward;
				break;

			case GGConstants.ByteCodes.Entities.Fireball:
				moveChoices[1] = Right().ToVector();
				moveChoices[2] = Left().ToVector();
				moveChoices[3] = -Forward;
				break;

			case GGConstants.ByteCodes.Entities.Bug:
				moveChoices = [
					Left().ToVector()
					, Forward
					, Right().ToVector()
					, -Forward
				];
				break;

			case GGConstants.ByteCodes.Entities.Paramecium:
				moveChoices = [
					Right().ToVector()
					, Forward
					, Left().ToVector()
					, -Forward
				];
				break;

			case GGConstants.ByteCodes.Entities.Walker:
				moveChoices = [
					Left().ToVector()
					, Back().ToVector()
					, Right().ToVector()
				];
				Random.Shared.Shuffle(moveChoices);
				moveChoices = [Forward, moveChoices[0]];
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

			case GGConstants.ByteCodes.Entities.Teeth:
				if (Coordinate == GameData.Level.PlayerCoordinate)
				{
					// Teeth is on Player. No movement needed.
					return;
				}
				if((GameData.Level.CurrentTick + GameData.Level.SteppingOffset) % 4 != 0)
				{
					return;
				}

				var chipPosition = GameData.Level.PlayerCoordinate;
				var delta = chipPosition - Coordinate;
				Vector2I choiceY = (delta.Y < 0) ? new(0, -1)
					: (delta.Y > 0) ? new(0, 1) : Vector2I.Zero;
				delta.Y = Math.Abs(delta.Y);
				Vector2I choiceX = (delta.X < 0) ? new(-1, 0)
					: (delta.X > 0) ? new(1, 0) : Vector2I.Zero;
				delta.X = Math.Abs(delta.X);
				moveChoices = (delta.X > delta.Y)
					? [choiceX, choiceY] : [choiceY, choiceX];
				pDir = moveChoices[0];
				break;
		}

		#endregion

		for(int i = 0; i < moveChoices.Length; i++)
		{
			if (moveChoices[i] == Vector2I.Zero)
			{
				break;
			}

			TDir = moveChoices[i];
			FocusCreatureLog($"Testing move choice of {TDir} from {Coordinate}");
			if (CanMakeMove(moveChoices[i], CanMakeMoveFlags.ClearAnimations))
			{
				return;
			}
		}

		if(pDir != Vector2I.Zero)
		{
			TDir = pDir;
		}
	}


	#endregion


	#region Movement Methods

	public int AdvanceCreature(bool isReleasing)
	{
		// FocusCreatureLog("Advancing creature");
		Vector2I tDir = Vector2I.Zero;

		if (Moving <= 0 /* && false == IsAnimation() */)
		{
			if (isReleasing)
			{
				tDir = TDir;
				TDir = Orientation.ToVector();
			}
			else if (TDir == Vector2I.Zero && ForceDirection == 0)
			{
				if (GameData.LynxPedantic && false == EndMovement(true))
				{
					return -1;
				}
				return 1;
			}

			int moveResult = StartMovement(isReleasing);
			if (moveResult > 0)
			{
				IsHidden = false;
			}
			if (GameData.LynxPedantic && moveResult == 0 && false == EndMovement(true))
			{
				return -1;
			}
			if (moveResult < 0)
			{
				return moveResult;
			}
			if (moveResult == 0)
			{
				if (isReleasing)
				{
					TDir = tDir;
					return 0;
				}
			}

			TDir = Vector2I.Zero;
		}

		if(false == ContinueMovement())
		{
			if(false == EndMovement(false))
			{
				return -1;
			}
		}

		return 1;
	}


	private int StartMovement(bool isReleasing)
	{
		FocusCreatureLog("Starting movement");
		Global.AssertTrue(Moving <= 0, "Wuh oh! Moving value is positive");

		Vector2I direction = Vector2I.Zero;
		if(TDir != Vector2I.Zero)
		{
			direction = TDir;
		}
		else if(GetForceDirection() != Vector2I.Zero)
		{
			direction = GetForceDirection();
		}
		else
		{
			return 0;
		}
		Global.AssertTrue(false == direction.IsDiagonal(), $"Oh no! Direction {direction} is diagonal!");
		SetOrientation(direction);
		var destinationCoordinate = Coordinate + direction;

		var currentTile = GameData.Map.GetTile2(Coordinate);
		var destinationCreature = GameData.Map.GetLynxCreature(destinationCoordinate);

		#region Check sliding state for Player

		if (IsThePlayer)
		{
			if(false == HasItem(GGConstants.ByteCodes.Entities.SuctionBoots))
			{
				if(currentTile is ForceFloorTile2 && TDir == Vector2I.Zero)
				{
					SetCreatureState(CreatureStateFlags.SlideToken);
				}
				else if(currentTile is not IceTile2 || HasItem(GGConstants.ByteCodes.Entities.IceSkates))
				{
					RemoveCreatureState(CreatureStateFlags.SlideToken);
				}
			}
		}

		#endregion

		int movementFlags = CanMakeMoveFlags.PushBlocksNow 
			| CanMakeMoveFlags.ClearAnimations 
			| CanMakeMoveFlags.StartMovement
			| (isReleasing ? CanMakeMoveFlags.Releasing : 0);
		if(false == CanMakeMove(direction, movementFlags))
		{
			if(IsThePlayer)
			{
				var player = (LynxPlayer2)this;
				if(false == player.CouldntMove)
				{
					player.CouldntMove = true;
					// Sound effect
				}
				LynxPlayer2.ChipPushing = true;
			}
			if(currentTile is IceTile2 iceTile && (false == IsThePlayer || false == HasItem(GGConstants.ByteCodes.Entities.IceSkates)))
			{
				AboutFace();
				var iceDirection = iceTile.SetCreatureDirection(Forward);
				SetOrientation(iceDirection);
			}
			return 0;
		}

		// TODO: Map breach check

		if(currentTile is CloneMachineTile2 || currentTile is TrapTile2)
		{
			Global.AssertTrue(isReleasing, $"Wuh oh! Creature is on a {currentTile.MapObjectName} but is not released!");
		}

		if(false == IsThePlayer)
		{
			ReleaseTileClaim();
		}
		if(this is not LynxDirtBlockCreature2 && Coordinate == LynxPlayer2.ChipToPosition)
		{
			LynxPlayer2.ChipToCreature = this;
		}
		/*
		else if(LynxPlayer2.ChipToCreature != null && false == LynxPlayer2.ChipToCreature.IsHidden)
		{
			LynxPlayer2.ChipToCreature.Moving = 8;
			GameData.Level.DeathMessage = GGConstants.DeathMessages.Monsters;
			return -1;
		}
		*/

		SetCoordinate(destinationCoordinate);
		if(false == IsThePlayer)
		{
			SetTileClaim();
		}
		Moving += 8;

		if(false == IsThePlayer && Coordinate == GameData.Level.PlayerCoordinate /* && false == Player.IsHidden */)
		{
			GameData.Level.DeathMessage = (EntityID == GGConstants.ByteCodes.Entities.DirtBlock)
				? GGConstants.DeathMessages.MovingBlocks
				: GGConstants.DeathMessages.Monsters;
			return -1;
		}

		if(IsThePlayer)
		{
			((LynxPlayer2)this).CouldntMove = false;
			var otherCreature = GameData.Map.GetLynxCreature(Coordinate);
			if(otherCreature is not null && otherCreature.CreatureID != CreatureID)
			{
				GameData.Level.DeathMessage = GGConstants.DeathMessages.Monsters;
				return -1;
			}
		}

		if(HasCreatureState(CreatureStateFlags.Pushed))
		{
			LynxPlayer2.ChipPushing = true;
			// Sound effect
		}

		#region Check Current Monster

		if (destinationCreature is not null)
		{
			switch (destinationCreature.EntityID)
			{
				case GGConstants.ByteCodes.Entities.RedBomb:
					if (IsThePlayer)
					{
						GameData.Level.DeathMessage = GGConstants.DeathMessages.Bombs;
					}
					else
					{
						destinationCreature.Kill();
						Kill();
					}
					break;

				case GGConstants.ByteCodes.Entities.Player:
					if (IsThePlayer)
					{
						Global.AssertTrue(false, "...Huh? The player died to themselves?");
					}
					else
					{
						if (destinationCreature.IsThePlayer && false == destinationCreature.HasItem(GGConstants.ByteCodes.Entities.Helmet))
						{
							GameData.Level.DeathMessage = GGConstants.DeathMessages.Monsters;
						}
					}
					break;

				default:
					if (IsThePlayer 
						&& false == HasItem(GGConstants.ByteCodes.Entities.Helmet)
						&& destinationCreature is not LynxDirtBlockCreature2)
					{
						GameData.Level.DeathMessage = GGConstants.DeathMessages.Monsters;
					}
					break;
			}
		}

		#endregion

		return 1;
	}


	private bool ContinueMovement()
	{
		FocusCreatureLog($"Continuing movement (Moving: {Moving})");
		if(Moving <= 0)
		{
			return true;
		}

		Global.AssertTrue(Moving > 0, "Wuh oh! This creature is not moving while in Continue Movement!");

		if(IsThePlayer && ((LynxPlayer2)this).PlayerIsStuck)
		{
			return true;
		}

		int speed = (this is LynxBlobCreature2 || this is LynxTeethCreature2) ? 1 : 2;
		var currentTile = GameData.Map.GetTile2(Coordinate);
		if(currentTile is ForceFloorTile2 
			&& (false == IsThePlayer || false == HasItem(GGConstants.ByteCodes.Entities.SuctionBoots)))
		{
			speed *= 2;
		}
		else if(currentTile is IceTile2
			&& (false == IsThePlayer || false == HasItem(GGConstants.ByteCodes.Entities.IceSkates)))
		{
			speed *= 2;
		}
		Moving -= speed;
		return (Moving > 0);
	}


	private bool EndMovement(bool isStationary)
	{
		FocusCreatureLog("Ending movement");
		bool survived = true;

		Global.AssertTrue(false == isStationary || GameData.LynxPedantic, "Wuh oh! Can't be pedantic and nonstationary!");

		// TODO: If animation return true

		Global.AssertTrue(Moving <= 0, "Wuh oh! We're in EndMovement but the creature is still moving!");

		var currentTile = GameData.Map.GetTile2(Coordinate);
		var currentItem = GameData.Map.GetItem2(Coordinate);
		var currentCreature = GameData.Map.GetLynxCreature(Coordinate);

		if (IsThePlayer && LynxPlayer2.PutWall != -1)
		{
			return true;
		}

		#region Ice Slipping logic

		if (currentTile is IceTile2 iceTile)
		{
			var iceDirection = iceTile.SetCreatureDirection(Forward);
			if(IsThePlayer && false == HasItem(GGConstants.ByteCodes.Entities.IceSkates))
			{
				SetOrientation(iceDirection);
			}
			if(false == IsThePlayer && false == isStationary)
			{
				SetOrientation(iceDirection);
			}
		}

		#endregion

		#region Creature-specific logic

		if (this is LynxPlayer2 thisPlayer)
		{
			switch(currentTile.EntityID)
			{
				case GGConstants.ByteCodes.Entities.Water:
					if(false == HasItem(GGConstants.ByteCodes.Entities.Flippers))
					{
						GameData.Level.DeathMessage = GGConstants.DeathMessages.Water;
						survived = false;
					}
					break;

				case GGConstants.ByteCodes.Entities.Fire:
					if(false == isStationary
						&& false == HasItem(GGConstants.ByteCodes.Entities.FireBoots))
					{
						GameData.Level.DeathMessage = GGConstants.DeathMessages.Fire;
						survived = false;
					}
					break;

				case GGConstants.ByteCodes.Entities.Dirt:
				case GGConstants.ByteCodes.Entities.FakeBlueWall:
					GameData.Map.SetTile(new FloorTile2(Coordinate));
					break;

				case GGConstants.ByteCodes.Entities.RecessedWall:
					GameData.Map.SetTile(new WallTile2(Coordinate));
					break;

				case GGConstants.ByteCodes.Entities.RedDoor:
				case GGConstants.ByteCodes.Entities.BlueDoor:
				case GGConstants.ByteCodes.Entities.YellowDoor:
				case GGConstants.ByteCodes.Entities.GreenDoor:
					var keyDoor = (KeyDoorTile2)currentTile;
					keyDoor.CanCreatureOpenDoor(this, true);
					GameData.Map.SetTile(new FloorTile2(Coordinate));
					break;

				case GGConstants.ByteCodes.Entities.ToolThief:
					StealTools();
					break;

				case GGConstants.ByteCodes.Entities.KeyThief:
					StealKeys();
					break;

				case GGConstants.ByteCodes.Entities.Socket:
					if(GameData.Level.ChipRequirementMet)
					{
						GameData.Map.SetTile(new FloorTile2(Coordinate));
					}
					break;
			}
			if (currentItem is not null)
			{
				switch(currentItem.EntityID)
				{
					case GGConstants.ByteCodes.Entities.RedKey:
					case GGConstants.ByteCodes.Entities.BlueKey:
					case GGConstants.ByteCodes.Entities.GreenKey:
					case GGConstants.ByteCodes.Entities.YellowKey:
					case GGConstants.ByteCodes.Entities.IceSkates:
					case GGConstants.ByteCodes.Entities.FireBoots:
					case GGConstants.ByteCodes.Entities.SuctionBoots:
					case GGConstants.ByteCodes.Entities.Flippers:
					case GGConstants.ByteCodes.Entities.Helmet:
					case GGConstants.ByteCodes.Entities.SecretEye:
						PickUpItem(currentItem);
						break;

					case GGConstants.ByteCodes.Entities.Chip:
					case GGConstants.ByteCodes.Entities.ExtraChip:
						if(false == isStationary)
						{
							var chipItem = (ChipItem2)currentItem;
							GameData.OnChipCollected(chipItem);
						}
						break;
				}
			}
			if(currentCreature is not null && currentCreature.EntityID != EntityID)
			{
				switch (currentCreature.EntityID)
				{
					case GGConstants.ByteCodes.Entities.RedBomb:
						GameData.Level.DeathMessage = GGConstants.DeathMessages.Bombs;
						survived = false;
						break;

					default:
						if (false == thisPlayer.HasItem(GGConstants.ByteCodes.Entities.Helmet))
						{
							GameData.Level.DeathMessage = (currentCreature is LynxDirtBlockCreature2)
								? GGConstants.DeathMessages.MovingBlocks
								: GGConstants.DeathMessages.Monsters;
							survived = false;
						}
						break;
				}
			}
		}
		else if(this is LynxDirtBlockCreature2 dirtBlock)
		{
			switch(currentTile.EntityID)
			{
				case GGConstants.ByteCodes.Entities.Water:
					GameData.Map.SetTile(new DirtTile2(Coordinate));
					FocusCreatureLog("Dirt block is sinking in the water");
					survived = false;
					break;
			}
			if(currentItem is not null)
			{
				switch(currentItem.EntityID)
				{
					// Dirt blocks eat blue keys?
					case GGConstants.ByteCodes.Entities.BlueKey:
						GameData.Map.RemoveItem2(Coordinate);
						break;
				}
			}
		}
		else
		{
			switch (currentTile.EntityID)
			{
				case GGConstants.ByteCodes.Entities.Water:
					if(EntityID != GGConstants.ByteCodes.Entities.Glider)
					{
						FocusCreatureLog($"{MapObjectName} is drowning in the water");
						survived = false;
					}
					break;
			}
			if (currentItem is not null)
			{
				switch (currentItem.EntityID)
				{
					// Monsters eat blue keys?
					case GGConstants.ByteCodes.Entities.BlueKey:
						GameData.Map.RemoveItem2(Coordinate);
						break;
				}
			}
		}

		#endregion

		if(false == survived)
		{
			FocusCreatureLog($"Killing creature at {Coordinate}");
			Kill();
			return false;
		}

		#region Universal creature logic

		switch(currentTile.EntityID)
		{
			case GGConstants.ByteCodes.Entities.TrapActive:
			case GGConstants.ByteCodes.Entities.TrapInactive:
				TrapTile2 trap = (TrapTile2)currentTile;
				break;

			case GGConstants.ByteCodes.Entities.BlueButton:
				GameData.BlueButtonPressed(Coordinate);
				break;

			case GGConstants.ByteCodes.Entities.GreenButton:
				GameData.GreenButtonPressed(Coordinate);
				break;

			case GGConstants.ByteCodes.Entities.RedButton:
				GameData.RedButtonPressed(Coordinate);
				break;

			case GGConstants.ByteCodes.Entities.BrownButton:
				// Where does the trap logic go?
				break;

			// There's supposed to be socket, dirt, and fake blue wall logic here
			// but I already did that up top?
		}

		#endregion

		return survived;
	}


	public bool CanMakeMove(Vector2I direction, int flags)
	{
		Global.AssertTrue(direction != Vector2I.Zero, $"{MapObjectName} {CreatureID}: Wuh oh! Running CanMakeMove with zero direction!");
		var destinationCoordinate = Coordinate + direction;
		FocusCreatureLog($"Can {MapObjectName} move from {Coordinate} to {destinationCoordinate}?");

        #region Thin Wall Checks

        var currentThinWall = GameData.Map.GetThinWall(Coordinate);
		if(currentThinWall is not null)
		{
			if(false == currentThinWall.CreatureCanExit(direction))
            {
                FocusCreatureLog($"No, it cannot leave {currentThinWall.MapObjectName} from that direction");
                return false;
            }
		}

		var destinationThinWall = GameData.Map.GetThinWall(destinationCoordinate);
		if(destinationThinWall is not null)
		{
			if(false == destinationThinWall.CreatureCanEnter(direction))
            {
                FocusCreatureLog($"No, it cannot enter {destinationThinWall.MapObjectName} from that direction");
                return false;
            }
		}

		#endregion

		var currentTile = GameData.Map.GetTile2(Coordinate);
		if(currentTile is IceTile2 iceTile)
		{
			if(false == iceTile.CreatureCanExit(direction))
			{
				FocusCreatureLog($"No, it cannot leave {iceTile.MapObjectName} from that direction");
				return false;
			}
		}

		if(currentTile is TrapTile2 || currentTile is CloneMachineTile2)
		{
			if((flags & CanMakeMoveFlags.Releasing) == 0)
			{
				FocusCreatureLog($"No, it has not been released from the {currentTile.MapObjectName}");
				return false;
			}
		}

		if(currentTile is ForceFloorTile2 forceFloor 
			&& (false == IsThePlayer || false == HasItem(GGConstants.ByteCodes.Entities.SuctionBoots))
			&& forceFloor.GetInfluenceDirection() == -1 * direction)
		{
			FocusCreatureLog($"No, something something force floor");
			return false;
		}

		if(false == GameData.Map.IsWithinBoundaries(destinationCoordinate))
		{
			FocusCreatureLog($"No, {destinationCoordinate} is not within the bounds of the map!");
			return false;
		}

		var destinationTile = GameData.Map.GetTile2(destinationCoordinate);
		var destinationCreature = GameData.Map.GetLynxCreature(destinationCoordinate);
		var destinationItem = GameData.Map.GetItem2(destinationCoordinate);

		if(IsThePlayer)
        {
            if (destinationCreature is not null
                && HasItem(GGConstants.ByteCodes.Entities.Helmet)
				&& destinationCreature is not LynxDirtBlockCreature2)
            {
                FocusCreatureLog($"No, you have a helmet and are approaching a {destinationCreature.MapObjectName}");
				return false;
            }
            if (false == EntryLaws.CanPlayerEnter(destinationTile.EntityID, direction))
			{
				FocusCreatureLog($"No, you cannot enter {destinationTile.MapObjectName} from that direction");
				return false;
			}
			if(destinationTile is SocketTile2 && false == GameData.Level.ChipRequirementMet)
			{
				FocusCreatureLog($"No, you have not gotten the required amount of Chips");
				return false;
			}
			if(destinationTile is KeyDoorTile2 keyDoor && false == keyDoor.CanCreatureOpenDoor(this, false))
			{
				FocusCreatureLog($"No, you cannot open the {keyDoor.MapObjectName}");
				return false;
			}
			if(destinationTile is HiddenWallTile2 
				|| (destinationTile is BlueWallTile2 blueWall && blueWall.IsReal))
			{
				GameData.Map.SetTile(new WallTile2(destinationCoordinate));
				FocusCreatureLog($"No, there is a {destinationTile.MapObjectName} there");
				return false;
			}
            // IsMarkedAnimated

			if(destinationCreature is LynxDirtBlockCreature2 dirtBlock)
			{
				FocusCreatureLog("Checking block push");
				if(false == dirtBlock.CanPushBlock(direction, flags & ~(CanMakeMoveFlags.Releasing)))
				{
					FocusCreatureLog($"No, the dirt block cannot be pushed");
					return false;
				}
			}
        }
		else if(this is LynxDirtBlockCreature2 dirtBlock)
		{
			if(Moving > 0)
			{
				FocusCreatureLog($"No, it is still moving");
				return false;
			}

            #region Entry Law Checks

			// Tile
            if (false == EntryLaws.CanBlockEnter(destinationTile.EntityID, direction))
			{
				FocusCreatureLog($"No, it cannot enter {destinationTile.MapObjectName} from that direction");
				return false;
			}

			// Item
			if(destinationItem is not null)
			{
				if(false == EntryLaws.CanBlockEnter(destinationItem.EntityID, direction))
                {
                    FocusCreatureLog($"No, it cannot enter {destinationItem.MapObjectName} from that direction");
                    return false;
                }
			}

			// Creature
			if(destinationCreature is not null)
			{
				if(false == EntryLaws.CanBlockEnter(destinationCreature.EntityID, direction))
                {
                    FocusCreatureLog($"No, it cannot enter {destinationCreature.MapObjectName} from that direction");
                    return false;
                }
			}

			#endregion

			if(GameData.Map.IsTileClaimed(destinationCoordinate))
			{
				FocusCreatureLog($"No, the destination has been claimed");
				return false;
			}
			if((flags & CanMakeMoveFlags.ClearAnimations) != 0)
			{
				// Clear animations
			}
		}
		else
        {
            #region Entry Law Checks

            // Tile
            if (false == EntryLaws.CanCreatureEnter(destinationTile.EntityID, direction))
            {
                FocusCreatureLog($"No, it cannot enter {destinationTile.MapObjectName} from that direction");
                return false;
            }

            // Item
            if (destinationItem is not null)
            {
                if (false == EntryLaws.CanCreatureEnter(destinationItem.EntityID, direction))
                {
                    FocusCreatureLog($"No, it cannot enter {destinationItem.MapObjectName} from that direction");
                    return false;
                }
            }

            // Creature
            if (destinationCreature is not null)
            {
                if (false == EntryLaws.CanCreatureEnter(destinationCreature.EntityID, direction))
                {
                    FocusCreatureLog($"No, it cannot enter {destinationCreature.MapObjectName} from that direction");
                    return false;
                }
				if(destinationCreature is LynxPlayer2 
					&& destinationCreature.HasItem(GGConstants.ByteCodes.Entities.Helmet))
				{
					FocusCreatureLog($"No, the Player has a helmet");
					return false;
				}
            }

            #endregion
            FocusCreatureLog("Checking tile claim");
            if (GameData.Map.IsTileClaimed(destinationCoordinate))
            {
                FocusCreatureLog($"No, the destination has been claimed");
                return false;
            }
			if(destinationTile is FireTile2 && this is not LynxFireballCreature2)
			{
				FocusCreatureLog("No, destination is a fire tile");
				return false;
            }
            if ((flags & CanMakeMoveFlags.ClearAnimations) != 0)
            {
                // Clear animations
            }
        }

		FocusCreatureLog("Yes!");
        return true;
	}


	private bool GetForcedMove()
	{
		SetForceDirection(Vector2I.Zero);

		var currentTile = GameData.Map.GetTile2(Coordinate);
		if(GameData.Level.CurrentTick == 0)
		{
			return false;
		}

		if(currentTile is IceTile2 ice)
		{
			if(this is LynxPlayer2 player)
			{
				if(HasItem(GGConstants.ByteCodes.Entities.IceSkates)
					|| LynxPlayer2.IsStuck)
				{
					return false;
				}
			}
			if(Orientation.ToVector() == Vector2I.Zero)
			{
				return false;
			}
			SetForceDirection(Forward);
			return true;
		}
		else if(currentTile is ForceFloorTile2 forceFloor)
		{
			if(IsThePlayer && HasItem(GGConstants.ByteCodes.Entities.SuctionBoots))
			{
				return false;
			}
			SetForceDirection(forceFloor.GetInfluenceDirection());
			return (false == HasCreatureState(CreatureStateFlags.SlideToken));
		}
		else if(HasCreatureState(CreatureStateFlags.Teleported))
		{
			RemoveCreatureState(CreatureStateFlags.Teleported);
			SetForceDirection(Forward);
			return true;
		}

		return false;
	}


	public void SetForceDirection(Vector2I value)
	{
		int forceDirection = 0;
		if (value.Y == -1)
		{
			forceDirection |= 0x01;
		}
		if (value.X == -1)
		{
			forceDirection |= 0x02;
		}
		if(value.Y == 1)
		{
			forceDirection |= 0x04;
		}
		if(value.X == 1)
		{
			forceDirection |= 0x08;
		}
		if(forceDirection != ForceDirection)
		{
			FocusCreatureLog($"Forced direction changed to {value} ({forceDirection})");
		}
		ForceDirection = forceDirection;
	}


	public Vector2I GetForceDirection()
	{
		Vector2I result = Vector2I.Zero;
		int forceDirection = ForceDirection;
		if((forceDirection & 0x01) != 0)
		{
			result += EntityOrientation.North.ToVector();
		}
		if((forceDirection & 0x02) != 0)
		{
			result += EntityOrientation.West.ToVector();
		}
		if((forceDirection & 0x04) != 0)
		{
			result += EntityOrientation.South.ToVector();
		}
		if((forceDirection & 0x08) != 0)
		{
			result += EntityOrientation.East.ToVector();
		}
		if(result != Vector2I.Zero)
		{
			FocusCreatureLog($"Force direction is {result}");
		}
		return result;
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

	public void TurnRight()
	{
		Orientation = Right();
	}


	public void TurnLeft()
	{
		Orientation = Left();
	}


	public void AboutFace()
	{
		FocusCreatureLog($"About face");
		Orientation = Back();
		FocusCreatureLog($"New orientation: {Orientation}");
	}


	public void SetOrientation(Vector2I direction)
	{
		if (direction == new Vector2I(0, -1))
		{
			Orientation = EntityOrientation.North;
			return;
		}
		if (direction == new Vector2I(0, 1))
		{
			Orientation = EntityOrientation.South;
			return;
		}
		if (direction == new Vector2I(1, 0))
		{
			Orientation = EntityOrientation.East;
			return;
		}
		if (direction == new Vector2I(-1, 0))
		{
			Orientation = EntityOrientation.West;
			return;
		}

		Global.AssertTrue(false, $"Oh no! Direction {direction} does not have an orientation!");
	}

	#endregion


	#region Teleporter Methods

	public void BlueTeleport()
	{
		FocusCreatureLog($"Encountered blue teleporter at {Coordinate}");
		var currentTile = GameData.Map.GetTile2(Coordinate);
		Global.AssertTrue(currentTile is BlueTeleporterTile2, $"Wuh oh! This tile is a {currentTile.MapObjectName}, not a blue teleporter!");

		Vector2I originalCoordinate = Coordinate.Clone();
		Vector2I searchCoordinate = Coordinate.Clone();

		int x = searchCoordinate.X;
		int y = searchCoordinate.Y;
		while(true)
		{
			x--;
			if(x < 0)
			{
				x = GameData.Map.Dimensions.X;
				y--;
				if(y < 0)
				{
					y = GameData.Map.Dimensions.Y;
				}
			}
			searchCoordinate = new(x, y);
			var searchTile = GameData.Map.GetTile2(searchCoordinate);
			if(searchTile is BlueTeleporterTile2)
			{
				if(false == IsThePlayer)
				{
					ReleaseTileClaim();
				}
				Coordinate = searchCoordinate;
				if(false == GameData.Map.IsTileClaimed(searchCoordinate)
					&& CanMakeMove(Forward, 0))
				{
					FocusCreatureLog($"Successfully teleporting to {searchCoordinate}");
					break;
				}
				if(searchCoordinate == originalCoordinate)
				{
					FocusCreatureLog("Teleport failed");
					if(IsThePlayer)
					{
						// ChipStuck = true
					}
                    ForceSetCoordinate(originalCoordinate);
                    FocusCreatureLog($"Exiting teleporter at {Coordinate} (fail)");
                    return; // false
				}
			}
		}

		if(IsThePlayer)
		{
			// Teleporter sound effect
		}
		SetCreatureState(CreatureStateFlags.Teleported);
        FocusCreatureLog($"Exiting teleporter at {Coordinate} (success)");
		Coordinate = originalCoordinate;
		ForceSetCoordinate(searchCoordinate);
	}

	#endregion


	private bool SetCoordinate(Vector2I newCoordinate)
	{
		if(Coordinate == newCoordinate)
		{
			FocusCreatureLog("Coordinate has not moved");
			return false;
		}

		ForceSetCoordinate(newCoordinate);
		return true;
	}


	private void ForceSetCoordinate(Vector2I newCoordinate)
    {
        FocusCreatureLog($"Moving from {Coordinate} to {newCoordinate}");
        GameData.Map.RemoveCreature(Coordinate);
        GameData.Map.LynxCreatures2[Coordinate.X, Coordinate.Y] = null;
        ReleaseTileClaim(); // Needed?
        Coordinate = newCoordinate;
        GameData.Map.LynxCreatures2[newCoordinate.X, newCoordinate.Y] = this;
        if (IsThePlayer)
        {
            GameData.Level.PlayerCoordinate = newCoordinate;
        }
		else
		{
			SetTileClaim();
		}
    }


	public void SetTileClaim()
	{
		// FocusCreatureLog($"Setting claim to tile at {Coordinate}");
		var mapTile = GameData.Map.GetTile2(Coordinate);
		mapTile.SetFloorState(FloorStateFlags.Claimed);
	}


	public void ReleaseTileClaim()
	{
		// FocusCreatureLog($"Releasing claim to tile at {Coordinate}");
		var mapTile = GameData.Map.GetTile2(Coordinate);
		mapTile.RemoveFloorState(FloorStateFlags.Claimed);
	}


	private void FocusCreatureLog(string message)
	{
		if (CreatureOfFocus != CreatureID)
		{
			return;
		}

		GD.Print($"Creature {CreatureID}: {message}");
	}
}
