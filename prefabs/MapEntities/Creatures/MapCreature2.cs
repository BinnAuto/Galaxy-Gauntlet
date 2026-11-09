using GalaxyGauntlet.prefabs.MapEntities;
using GalaxyGauntlet.scripts;
using System.Collections.Generic;

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
						// TODO: Shuffle choices
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
						// TODO: Shuffle choices
                        break;

                    case GGConstants.ByteCodes.Entities.Blob:
                        moveChoices = [
                            Forward
                            , Left().ToVector()
                            , Back().ToVector()
                            , Right().ToVector()
                        ];
                        // TODO: Shuffle choices
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
				if(CanMakeMove(TDir))
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
            // This check is not in the base code, but it should be to
            // accomplish the correct behavior. Maybe it's elsewhere.
            if (GameData.Level.ChipTick < 3)
			{
				return false;
			}

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
		if(false == CanMakeMove(direction))
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
		if(IsPlayer)
		{

		}

		SetCoordinateAndOrientation(destinationCoordinate);
	}


	public virtual void UpdateCreature()
	{
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
	}


	public virtual bool CanMakeMove(Vector2I direction)
	{
		var newCoordinate = Coordinate + direction;
		SetOrientation(direction);
		if(false == GameData.Map.IsWithinDimensions(newCoordinate))
		{
			return false;
		}

		var destinationTile = GameData.Map.GetTile2(newCoordinate);
		if(destinationTile is CloneMachineTile2)
		{
			return false;
		}

		// TODO: Check for thin wall exit

		if (IsPlayer)
		{
			#region Move Law check

			var entryLaw = EntryLaws.Laws.GetValueOrDefault(destinationTile.EntityID);
			if (entryLaw is null)
			{
                // GD.Print($"Entry law not defined for tile {destinationTile.Name} (0x{destinationTile.EntityID:X2})");
				return false;
			}
			else
			{
				if (false == entryLaw.CanPlayerEnter(direction))
				{
					// GD.Print("Entry law does not allow entry");
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

			// TODO: Key Door check

			// TODO: Creature check

			// TODO: Hidden/Blue wall check

			// TODO: Dirt Block check
		}
		else if(EntityID == GGConstants.ByteCodes.Entities.DirtBlock)
		{
			// TODO
		}
		else
		{

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
}
