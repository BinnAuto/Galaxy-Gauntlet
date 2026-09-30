using System;

namespace GalaxyGauntlet.scripts.MapEntities.Shared
{
	public class MapMob(Vector2I coordinate) : MapEntity(coordinate)
	{
		public int MobID = GameData.GetNextMobID();

		public override bool NeedsOrientation => true;

		public override bool NeedsTileSpecification => true;

		public bool CanProcess = true;

		protected DateTime _lastProcessTime = DateTime.Parse("01/01/2000");

		public MapTile LowerLayer = new FloorTile(coordinate);

		public EntityOrientation Orientation = EntityOrientation.North;


		#region Movement Behavior

		public virtual bool CanWalkOnDirt => false;

		public virtual bool CanWalkOnGravel => false;

		public virtual bool AvoidsFire => false;

		public virtual bool DiesToWater => true;

		public virtual bool DiesToFire => true;

		#endregion



		#region Relative Move Vectors

		public Vector2I Forward
		{
			get
			{
				return Orientation switch
				{
					EntityOrientation.North => new(0, -1),
					EntityOrientation.South => new(0, 1),
					EntityOrientation.East => new(1, 0),
					EntityOrientation.West => new(-1, 0),
					_ => new(0, -1)
				};
			}
		}

		public Vector2I Left
		{
			get
			{
				return Orientation switch
				{
					EntityOrientation.North => new(-1, 0),
					EntityOrientation.South => new(1, 0),
					EntityOrientation.East => new(0, -1),
					EntityOrientation.West => new(0, 1),
					_ => new(0, 1)
				};
			}
		}

		public Vector2I Backward => -Forward;

		public Vector2I Right => -Left;

		#endregion


		public virtual MapMob CreateCopy()
		{
			throw new NotImplementedException($"Cannot create copy of {Name}");
		}


		public virtual void SetCoordinate(Vector2I newCoordinate)
		{
			if(Coordinate == newCoordinate)
			{
				return;
			}
			if(false == CanProcess)
			{
				return;
			}

			GameData.RemoveMapMob(Coordinate);
			Coordinate = newCoordinate;
			CheckForPlayer(Coordinate); // I don't like this here
			GameData.MapMobs[newCoordinate.X, newCoordinate.Y] = this;
			GameData.RerenderMap = true;
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
			GameData.RerenderMap = true;
		}


		public void ReverseOrientation()
		{
			Orientation = Orientation switch
			{
				EntityOrientation.North => EntityOrientation.South,
				EntityOrientation.South => EntityOrientation.North,
				EntityOrientation.East => EntityOrientation.West,
				EntityOrientation.West => EntityOrientation.East,
				_ => EntityOrientation.South
			};
			GameData.RerenderMap = true;
		}

		public void RemoveSelf()
		{
			CanProcess = false;
			GameData.RemoveMapMob(Coordinate);
		}


		public virtual void SetOrientationAndCoordinate(Vector2I direction, Vector2I coordinate)
		{
			SetOrientation(direction);
			SetCoordinate(coordinate);
		}

		public virtual void ProcessTick(DateTime timestamp)
		{
			try
			{
				var currentTile = GameData.GetMapTile(Coordinate);
				if(currentTile is CloneMachineTile)
				{
					// SAFETYNET: This is a display mob, it should not move.
					// This mob should have never been put in the process list.
					GD.Print("WARNING: Clone machine display mob has been put in the process list."); 
					return;
				}

				ProposeMove(Forward);
			}
			catch
			{

			}
			finally
			{
				_lastProcessTime = timestamp;
			}
		}


		/// <summary>
		/// Limit the directional input to 1-unit maximums, and check if
		/// resulting move will escape the map boundaries.
		/// </summary>
		public Vector2I? SanitizeInput(Vector2I direction)
		{
			// Clamp direction to maximum ABS to 1
			direction.X = Math.Min(Math.Max(direction.X, -1), 1);
			direction.Y = Math.Min(Math.Max(direction.Y, -1), 1);

			Vector2I newCoordinate = Coordinate + direction;

			// Check map bounds
			if (newCoordinate.X < 0 || newCoordinate.Y < 0)
			{
				// Leaving minimal bounds of map
				return null;
			}
			if (newCoordinate.X >= GameData.MapDimensions.X
				|| newCoordinate.Y >= GameData.MapDimensions.Y)
			{
				// Leaving maximal bounds of map
				return null;
			}

			return newCoordinate;
		}


		#region Specific Entity Check Methods
		public bool CheckIfOnTrap(Vector2I direction)
		{
			var currentItem = GameData.GetMapItem(Coordinate);
			if(currentItem is TrapItem trapItem && trapItem.IsActive)
			{
				SetOrientation(direction);
				return true;
			}

			return false;
		}


		public static void CheckForPlayer(Vector2I coordinate)
		{
			var mapMob = GameData.GetMapPlayer(coordinate);
			if(mapMob is Player)
			{
				GameData.DeathMessage = Constants.DeathMessages.Monsters;
			}
		}

		#endregion


		public virtual Vector2I ProposeMove(Vector2I direction)
		{
			if(false == CanProcess)
			{
				return Coordinate;
			}

			var processedCoordinate = SanitizeInput(direction);
			if(processedCoordinate is null)
			{
				return Coordinate;
			}

			if (CheckIfOnTrap(direction))
			{
				return Coordinate;
			}

			var currentTile = GameData.GetMapTile(Coordinate);
			if(currentTile is ThinWallOrCanopyTile currentThinWall
				&& false == currentThinWall.AllowExit(direction))
			{
				return Coordinate;
			}

			Vector2I newCoordinate = processedCoordinate.Value;
			var destinationTile = GameData.GetMapTile(newCoordinate);
			var destinationItem = GameData.GetMapItem(newCoordinate);
			var destinationMob = GameData.GetMapMob(newCoordinate);

			if(destinationTile is WallTile
				|| destinationTile is BlueWallTile
				|| (destinationTile is DirtTile && false == CanWalkOnDirt)
				|| (destinationTile is GravelTile && false == CanWalkOnGravel)
				|| destinationItem is RedDoorItem
				|| destinationItem is BlueDoorItem
				|| destinationItem is YellowDoorItem
				|| destinationItem is GreenDoorItem
				|| (destinationTile is GreenToggleTile greenToggle && greenToggle.IsWall)
				|| destinationTile is ExitTile
			)
			{
				return Coordinate;
			}

			if(destinationTile is WaterTile)
			{
				if(this is DirtBlockEntity)
				{
					GameData.SetMapTile(newCoordinate, new DirtTile(newCoordinate));
					RemoveSelf();
					return newCoordinate;
				}
				if(DiesToWater)
				{
					RemoveSelf();
				}
				return newCoordinate;
			}

			if(destinationTile is FireTile)
			{
				if(AvoidsFire)
				{
					return Coordinate;
				}
				if(DiesToFire)
				{
					RemoveSelf();
				}

				return newCoordinate;
			}

			if(destinationTile is ThinWallOrCanopyTile destinationThinWall)
			{
				return destinationThinWall.AllowEntry(direction)
					? newCoordinate
					: Coordinate;
			}

			if(destinationItem is SocketItem
				|| destinationItem is ChipItem
				|| destinationItem is RecessedWallItem
			)
			{
				return Coordinate;
			}

			if(destinationMob is DirtBlockEntity
				|| destinationMob is IceBlockEntity
				|| destinationMob is BlueTankMonster
				|| destinationMob is BugMonster
				|| destinationMob is TeethMonster
				|| destinationMob is GliderMonster
				|| destinationMob is WalkerMonster
			)
			{
				return Coordinate;
			}

			if(destinationMob is RedBombMonster redBomb)
			{
				GD.Print("Kaboom");
				redBomb.RemoveSelf();
				RemoveSelf();
				return newCoordinate;
			}

			return newCoordinate;
		}
	}
}
