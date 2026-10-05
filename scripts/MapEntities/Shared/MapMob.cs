using System;

namespace GalaxyGauntlet.scripts.MapEntities.Shared
{
	public class MapMob(Vector2I coordinate) : MapEntity(coordinate)
	{
		public int MobID = GameData.GetNextMobID();

		public override bool NeedsOrientation => true;

		public override bool NeedsTileSpecification => true;

		public bool CanProcess = true;

		public MapTile LowerLayer = new FloorTile(coordinate);

		public EntityOrientation Orientation = EntityOrientation.North;


		#region Movement Behavior

		public virtual bool CanMoveSelf => true;

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
			Exception e = new NotImplementedException($"Cannot create copy of {Name}");
			FileLogger.LogException("Entity clone failed", e);
			throw e;
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

		public virtual void ProcessTick()
		{
			try
			{
				if (CheckIfOnTrap(Forward))
				{
					return;
				}
				
				var currentTile = GameData.GetMapTile(Coordinate);
				if(currentTile is CloneMachineTile)
				{
					FileLogger.QuietLogMessage($"SAFETY NET: Display mob in clone machines should not move");
					return;
				}

				bool isPlayer = (this is Player);
				
				#region Force Floor
				
				if (currentTile is MapForceFloorTile forceFloor
					&& (false == isPlayer || false == GameData.PlayerHasGameItem(Constants.ByteCodes.Entities.SuctionBoots)))
				{
					GameData.AddToSlipList(this);
					var forceFloorInfluence = forceFloor.GetInfluence();
					var forceCoordinate = ProposeMove(forceFloorInfluence);
					SetOrientationAndCoordinate(forceFloorInfluence, forceCoordinate);
					return;
				}

				#endregion

				#region Ice Floor

				if(currentTile is MapIceTile iceTile
					&& (false == isPlayer || false == GameData.PlayerHasGameItem(Constants.ByteCodes.Entities.IceSkates)))
				{
					GameData.AddToSlipList(this);
					Orientation = iceTile.SetEntityOrientation(Orientation);
					var iceCoordinate = ProposeMove(Forward);
					if(iceCoordinate == Coordinate)
					{
						ReverseOrientation();
						Orientation = iceTile.SetEntityOrientation(Orientation);
						iceCoordinate = ProposeMove(Forward);
                    }
                    SetCoordinate(iceCoordinate);
					return;
				}

				#endregion

				GameData.RemoveFromSlipList(this);

				if(CanMoveSelf)
				{
					var forward = Forward;
					var newCoordinate = ProposeMove(forward);
					if(newCoordinate != Coordinate)
					{
						SetOrientationAndCoordinate(forward, newCoordinate);
						currentTile = GameData.GetMapTile(Coordinate);
						if(currentTile is MapIceTile || currentTile is MapForceFloorTile)
						{
							GameData.AddToSlipList(this);
						}
					}
				}
			}
			catch(Exception e)
			{
				FileLogger.LogException($"Error processing {Name} tick", e);
			}
		}


		/// <summary>
		/// Check if resulting move will escape the map boundaries.
		/// </summary>
		public Vector2I? SanitizeInput(Vector2I direction)
		{
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


		public virtual Vector2I ProposeMove(Vector2I direction)
		{
			if(false == CanProcess)
			{
				return Coordinate;
			}

			if (CheckIfOnTrap(Forward))
			{
				return Coordinate;
			}

			var processedCoordinate = SanitizeInput(direction);
			if(processedCoordinate is null)
			{
				return Coordinate;
			}

			bool isPlayer = (this is Player);

			Vector2I newCoordinate = processedCoordinate.Value;
			var destinationTile = GameData.GetMapTile(newCoordinate);
			var destinationItem = GameData.GetMapItem(newCoordinate);
			var destinationMob = GameData.GetMapMob(newCoordinate);

			#region Exit from Thin Wall tile
			
			var currentTile = GameData.GetMapTile(Coordinate);
			if(currentTile is ThinWallOrCanopyTile currentThinWall
				&& false == currentThinWall.AllowExit(direction))
			{
				return Coordinate;
			}

			#endregion

			#region Water Tile 

			if (destinationTile is WaterTile)
			{
				if(this is DirtBlockEntity)
				{
					// Dirt blocks turn water into dirt
					GameData.SetMapTile(newCoordinate, new DirtTile(newCoordinate));
					RemoveSelf();
					return newCoordinate;
				}
				if(isPlayer && false == GameData.PlayerHasGameItem(Constants.ByteCodes.Entities.Flippers))
				{
					GameData.DeathMessage = Constants.DeathMessages.Water;
					return newCoordinate;
				}
				if(DiesToWater)
				{
					RemoveSelf();
				}
				return newCoordinate;
			}

			#endregion

			#region Fire Tile

			if (destinationTile is FireTile)
			{
				if(AvoidsFire)
				{
					return Coordinate;
				}
				if(isPlayer && false == GameData.PlayerHasGameItem(Constants.ByteCodes.Entities.FireBoots))
				{
					GameData.DeathMessage = Constants.DeathMessages.Fire;
					return newCoordinate;
				}
				if(DiesToFire)
				{
					RemoveSelf();
				}

				return newCoordinate;
			}

            #endregion

            #region Key Doors

            if (destinationItem is MapDoorItem doorItem)
            {
                if (false == isPlayer)
                {
                    return Coordinate;
                }

                if (false == doorItem.CanOpenDoor())
                {
                    return Coordinate;
                }

                // Player opens door
                GameData.RemoveMapItem(newCoordinate);
                GameData.SetMapTile(newCoordinate, new FloorTile(newCoordinate));
                return newCoordinate;
            }

            #endregion

            #region Hard Stop tiles

            if (destinationTile is WallTile
                || destinationTile is CloneMachineTile
                || destinationTile is InvisibleWallTile
                || (destinationTile is GravelTile && false == CanWalkOnGravel)
                || (destinationTile is GreenToggleTile greenToggle && greenToggle.IsWall)
            )
            {
                return Coordinate;
            }

            #endregion

            #region Monsters 

            if (destinationMob is BugMonster
				|| destinationMob is FireballMonster
				|| destinationMob is WalkerMonster
				|| destinationMob is GliderMonster
				|| destinationMob is ParameciumMonster
				|| destinationMob is BallMonster
				|| destinationMob is BlobMonster
				|| destinationMob is BlueTankMonster
				|| destinationMob is TeethMonster
			)
			{
				if(isPlayer)
				{
					GameData.DeathMessage = Constants.DeathMessages.Monsters;
					return newCoordinate;
				}

				return Coordinate;
			}

			#endregion

			#region Blue Wall

			if (destinationTile is BlueWallTile blueWall)
			{
				if(false == isPlayer)
				{
					return Coordinate;
				}

				if(blueWall.IsReal)
				{
					GameData.SetMapTile(newCoordinate, new WallTile(newCoordinate));
					return Coordinate;
				}
				else
				{
					GameData.SetMapTile(newCoordinate, new FloorTile(newCoordinate));
					return newCoordinate;
				}
			}

			#endregion

			#region Hidden Wall 

			if(destinationTile is HiddenWallTile)
			{
				if(isPlayer)
				{
					GameData.SetMapTile(newCoordinate, new WallTile(newCoordinate));
				}
				return Coordinate;
			}

			#endregion

			#region Dirt Tile

			if(destinationTile is DirtTile)
			{
				if(isPlayer)
				{
					GameData.SetMapTile(newCoordinate, new FloorTile(newCoordinate));
					return newCoordinate;
				}

				return Coordinate;
			}

			#endregion

			#region Exit Tile

			if(destinationTile is ExitTile)
			{
				return isPlayer
					? newCoordinate
					: Coordinate;
			}

			#endregion

			#region Entry into Thin Wall tile

			if (destinationTile is ThinWallOrCanopyTile destinationThinWall)
			{
				return destinationThinWall.AllowEntry(direction)
					? newCoordinate
					: Coordinate;
			}

			#endregion

			#region Socket
			
			if(destinationItem is SocketItem)
			{
				if(isPlayer && GameData.ChipRequirementMet)
				{
					GameData.RemoveMapItem(newCoordinate);
					return newCoordinate;
				}

				return Coordinate;
			}

			#endregion

			#region Chip

			if(destinationItem is ChipItem
				&& false == isPlayer)
			{
				return Coordinate;
			}

			#endregion

			#region Recessed Wall

			if (destinationItem is RecessedWallItem)
			{
				if(false == isPlayer)
				{
					return Coordinate;
				}

				GameData.RemoveMapItem(newCoordinate);
				GameData.SetMapTile(newCoordinate, new WallTile(newCoordinate));
				return newCoordinate;
			}

			#endregion

			#region Blue Teleporter 

			if (destinationItem is BlueTeleporterItem blueTeleporter)
			{
				var oldCoordinate = Coordinate.Clone();
				var teleporterCoordinate = blueTeleporter.Coordinate;
				var currentCheckpoint = Coordinate + direction;
				while(true)
				{
					var teleportExit = blueTeleporter.SearchForExit(currentCheckpoint);
					if(teleportExit == teleporterCoordinate)
					{
						// EXIT: Search has covered the entire map
						if(isPlayer)
						{
							// Player will slide over or bounce from the teleporter
							Coordinate = teleporterCoordinate;
							newCoordinate = ProposeMove(direction);
							if(newCoordinate == Coordinate)
							{
								ReverseOrientation();
								return oldCoordinate;
							}

							GameData.RemoveMapPlayer(oldCoordinate);
							return newCoordinate;
						}
						return oldCoordinate;
						// TODO: Update this for specific entity behaviors where
						// they do not slide or bounce
					}

					Coordinate = teleportExit;
					var teleportExitStep = ProposeMove(direction);
					if(teleportExitStep != teleportExit)
					{
						// EXIT: Step is valid, teleport complete
						if(isPlayer)
						{
							GameData.RemoveMapPlayer(oldCoordinate);
						}
						else
						{
							GameData.RemoveMapMob(oldCoordinate);
						}
						newCoordinate = teleportExitStep;
						return newCoordinate;
					}

					// Exit step is invalid. Reset and look for the next exit.
					Coordinate = oldCoordinate;
					currentCheckpoint = teleportExit;
				}
			}

			#endregion

			#region Hard Stop mobs

			if (destinationMob is IceBlockEntity)
			{
				return Coordinate;
			}

			#endregion

			#region Dirt Block

			if(destinationMob is DirtBlockEntity dirtBlock)
			{
				if(false == isPlayer)
				{
					return Coordinate;
				}

				var dirtBlockCoordinate = dirtBlock.ProposeMove(direction);
				if(dirtBlockCoordinate == newCoordinate)
				{
					return Coordinate;
				}

				dirtBlock.SetOrientationAndCoordinate(direction, dirtBlockCoordinate);
				var dirtBlockTile = GameData.GetMapTile(dirtBlockCoordinate);
				if(dirtBlockTile is MapIceTile || dirtBlockTile is MapForceFloorTile)
				{
					GameData.AddToSlipList(dirtBlock);
				}
				return newCoordinate;
			}

			#endregion

			#region Red Bomb

			if (destinationMob is RedBombMonster redBomb)
			{
				if(isPlayer)
				{
					GameData.DeathMessage = Constants.DeathMessages.Bombs;
					return newCoordinate;
				}

				redBomb.RemoveSelf();
				RemoveSelf();
			}

			#endregion

			#region Tool Thief

			if(destinationItem is ToolThief)
			{
				if(isPlayer)
				{
					GameData.StealEquipment();
					return newCoordinate;
				}

				return Coordinate; // TODO: Can non-players walk over thieves?
			}

			#endregion

			return newCoordinate;
		}



		#region Specific Entity Check Methods

		/// <summary>
		/// Checks if the mob is currently on an active trap, preventing
		/// it from moving.
		/// </summary>
		public bool CheckIfOnTrap(Vector2I direction)
		{
			var currentItem = GameData.GetMapItem(Coordinate);
			if (currentItem is TrapItem trapItem && trapItem.IsActive)
			{
				SetOrientation(direction);
				return true;
			}

			return false;
		}


		public static void CheckForPlayer(Vector2I coordinate)
		{
			var mapMob = GameData.GetMapPlayer(coordinate);
			if (mapMob is Player)
			{
				GameData.DeathMessage = Constants.DeathMessages.Monsters;
			}
		}

		#endregion
	}
}
