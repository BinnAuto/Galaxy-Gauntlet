using GalaxyGauntlet.scripts.MapEntities.Shared;
using System;

namespace GalaxyGauntlet.scripts.MapEntities
{
	public class Player(Vector2I coordinate) : MapMob(coordinate)
	{
		public override int DataCode => Constants.ByteCodes.Entities.Player;

		public override string Name => "Player";

		public override Vector2I TextureCoordinate
		{
			get
			{
				return Orientation switch
				{
					EntityOrientation.North => Constants.SpriteCoordinates.Player_N,
					EntityOrientation.East => Constants.SpriteCoordinates.Player_E,
					EntityOrientation.South => Constants.SpriteCoordinates.Player_S,
					EntityOrientation.West => Constants.SpriteCoordinates.Player_W,
					_ => throw new NotImplementedException()
				};
			}
		}

		public override bool CanWalkOnDirt => true;

		public override bool CanWalkOnGravel => true;

		public override void ProcessTick(DateTime timestamp)
		{
			_lastProcessTime = timestamp;
			if(false == GameData.AcceptingPlayerInput)
			{
				return;
			}

			Vector2I input = Vector2I.Zero;
			if (Input.IsActionPressed("player_up"))
			{
				input = new(0, -1);
			}
			if (Input.IsActionPressed("player_down"))
			{
				input = new(0, 1);
			}
			if (Input.IsActionPressed("player_left"))
			{
				input = new(-1, 0);
			}
			if (Input.IsActionPressed("player_right"))
			{
				input = new(1, 0);
			}

			Vector2I newCoordinate = Coordinate;

			#region Process Item collection

			var currentItem = GameData.GetMapItem(Coordinate);
			if ((currentItem is RedKeyItem && GameData.RedKeyUnlocked)
				|| (currentItem is BlueKeyItem && GameData.BlueKeyUnlocked)
				|| (currentItem is YellowKeyItem && GameData.YellowKeyUnlocked)
				|| (currentItem is GreenKeyItem && GameData.GreenKeyUnlocked)
				|| (currentItem is IceSkatesItem && GameData.IceSkatesUnlocked)
				|| (currentItem is FireBootsItem && GameData.FireBootsUnlocked)
				|| (currentItem is FlippersItem && GameData.FlippersUnlocked)
				|| (currentItem is SuctionBootsItem && GameData.SuctionBootsUnlocked)
			)
			{
				GameData.RemoveMapItem(Coordinate);
				GameData.PlayerItems.Add(currentItem);
			}

			if (currentItem is ChipItem chip1)
			{
				GameData.OnChipCollected(chip1);
			}

			#endregion

			var currentTile = GameData.GetMapTile(Coordinate);

			#region Force Floor 

			if (currentTile is MapForceFloorTile forceFloor && false == GameData.PlayerHasGameItem(Constants.ByteCodes.Entities.SuctionBoots))
			{
				GameData.MoveToSlipList(this);
				var forceFloorInfluence = forceFloor.GetInfluence();
				var aggregateInput = forceFloorInfluence + input;
				aggregateInput = new(
					Mathf.Max(Mathf.Min(aggregateInput.X, 1), -1)
					, Mathf.Max(Mathf.Min(aggregateInput.Y, 1), -1)
				);
				newCoordinate = ProposeMove(aggregateInput);
				if(newCoordinate == Coordinate)
				{
					newCoordinate = ProposeMove(forceFloorInfluence);
					if(newCoordinate == Coordinate)
					{
						newCoordinate = ProposeMove(input);
					}
				}
				SetOrientationAndCoordinate(aggregateInput, newCoordinate);
				return;
			}

			#endregion

			#region Ice

			if(currentTile is MapIceTile iceTile && false == GameData.PlayerHasGameItem(Constants.ByteCodes.Entities.IceSkates))
			{
				GameData.MoveToSlipList(this);
				Orientation = iceTile.SetOrientation(Orientation);
				newCoordinate = ProposeMove(Forward);
				if(newCoordinate == Coordinate)
				{
					ReverseOrientation();
					newCoordinate = ProposeMove(Forward);
				}
				SetCoordinate(newCoordinate);
				return;
			}

			#endregion

			GameData.RemoveFromSlipList(this);
			newCoordinate = ProposeMove(input);
			if(newCoordinate == Coordinate)
			{
				return;
			}

			SetOrientationAndCoordinate(input, newCoordinate);
			var item = GameData.GetMapItem(Coordinate);

			if((item is RedKeyItem && GameData.RedKeyUnlocked)
				|| (item is BlueKeyItem && GameData.BlueKeyUnlocked)
				|| (item is YellowKeyItem && GameData.YellowKeyUnlocked)
				|| (item is GreenKeyItem && GameData.GreenKeyUnlocked)
				|| (item is IceSkatesItem && GameData.IceSkatesUnlocked)
				|| (item is FireBootsItem && GameData.FireBootsUnlocked)
				|| (item is FlippersItem && GameData.FlippersUnlocked)
				|| (item is SuctionBootsItem && GameData.SuctionBootsUnlocked)
			)
			{
				GameData.RemoveMapItem(Coordinate);
				GameData.PlayerItems.Add(item);
			}

			if(item is ChipItem chip2)
			{
				GameData.OnChipCollected(chip2);
			}
		}


		public override Vector2I ProposeMove(Vector2I direction)
		{
			var processedCoordinate = SanitizeInput(direction);
			if (processedCoordinate is null)
			{
				return Coordinate;
			}
			Vector2I newCoordinate = processedCoordinate.Value;

			if(CheckIfOnTrap(direction))
			{
				return Coordinate;
			}

			var currentTile = GameData.GetMapTile(Coordinate);

			#region Thin Wall exit

			if (currentTile is ThinWallOrCanopyTile currentThinWall
				&& false == currentThinWall.AllowExit(direction))
			{
				return Coordinate;
			}

			#endregion

			var destinationTile = GameData.GetMapTile(newCoordinate);
			var destinationItem = GameData.GetMapItem(newCoordinate);
			var destinationMob = GameData.GetMapMob(newCoordinate);

			#region Clone Machine

			if(destinationTile is CloneMachineTile)
			{
				return Coordinate;
			}

			#endregion

			#region Death via Water or Fire

			if(destinationTile is WaterTile
				&& false == GameData.PlayerHasGameItem(Constants.ByteCodes.Entities.Flippers))
			{
				GameData.DeathMessage = Constants.DeathMessages.Water;
				return newCoordinate;
			}

			if(destinationTile is FireTile
				&& false == GameData.PlayerHasGameItem(Constants.ByteCodes.Entities.FireBoots))
			{
				GameData.DeathMessage = Constants.DeathMessages.Fire;
				return newCoordinate;
			}

			#endregion

			#region Death via Monster

			if (destinationMob is BugMonster
				|| destinationMob is FireballMonster
				|| destinationMob is WalkerMonster
				|| destinationMob is GliderMonster
				|| destinationMob is ParameciumMonster
				|| destinationMob is BallMonster
				|| destinationMob is BlobMonster
				|| destinationMob is BlueTankMonster)
			{
				GameData.DeathMessage = Constants.DeathMessages.Monsters;
				return newCoordinate;
			}

			if(destinationMob is RedBombMonster)
			{
				GameData.DeathMessage = Constants.DeathMessages.Bombs;
				return newCoordinate;
			}

			#endregion

			#region Key Doors

			if (destinationItem is RedDoorItem)
			{
				if(GameData.ConsumeItem(Constants.ByteCodes.Entities.RedKey))
				{
					GameData.RemoveMapItem(newCoordinate);
					GameData.SetMapTile(newCoordinate, new FloorTile(newCoordinate));
					return newCoordinate;
				}

				return Coordinate;
			}

			if(destinationItem is BlueDoorItem)
			{
				if (GameData.ConsumeItem(Constants.ByteCodes.Entities.BlueKey))
				{
					GameData.RemoveMapItem(newCoordinate);
					GameData.SetMapTile(newCoordinate, new FloorTile(newCoordinate));
					return newCoordinate;
				}

				return Coordinate;
			}

			if (destinationItem is YellowDoorItem)
			{
				if (GameData.ConsumeItem(Constants.ByteCodes.Entities.YellowKey))
				{
					GameData.RemoveMapItem(newCoordinate);
					GameData.SetMapTile(newCoordinate, new FloorTile(newCoordinate));
					return newCoordinate;
				}

				return Coordinate;
			}

			if (destinationItem is GreenDoorItem)
			{
				if (GameData.PlayerHasGameItem(Constants.ByteCodes.Entities.GreenKey))
				{
					GameData.RemoveMapItem(newCoordinate);
					GameData.SetMapTile(newCoordinate, new FloorTile(newCoordinate));
					return newCoordinate;
				}

				return Coordinate;
			}

			#endregion

			#region Dirt Block

			if(destinationMob is DirtBlockEntity dirtBlockEntity)
			{
				var dirtBlockCoordinate = dirtBlockEntity.ProposeMove(direction);
				if(dirtBlockCoordinate == newCoordinate)
				{
					// Push block cannot be pushed
					return Coordinate;
				}

				dirtBlockEntity.SetCoordinate(dirtBlockCoordinate);
				return newCoordinate;
			}

			#endregion

			#region Wall

			if (destinationTile is WallTile
				|| destinationTile is InvisibleWallTile)
			{
				return Coordinate;
			}

			#endregion

			#region Hidden Wall

			if(destinationTile is HiddenWallTile)
			{
				GameData.SetMapTile(newCoordinate, new WallTile(newCoordinate));
				return Coordinate;
			}

			#endregion

			#region Blue wall

			if (destinationTile is BlueWallTile blueWall)
			{
				if(blueWall.IsReal)
				{
					GameData.SetMapTile(newCoordinate, new WallTile(newCoordinate));
					return Coordinate;
				}

				GameData.SetMapTile(newCoordinate, new FloorTile(newCoordinate));
				return newCoordinate;
			}

			#endregion

			#region Green Toggle Wall

			if (destinationTile is GreenToggleTile toggleTile && toggleTile.IsWall)
			{
				return Coordinate;
			}

			#endregion

			#region Thin Wall entry

			if (destinationTile is ThinWallOrCanopyTile destinationThinWall)
			{
				return destinationThinWall.AllowEntry(direction)
					? newCoordinate
					: Coordinate;
			}

			#endregion

			#region Dirt Tile

			if (destinationTile is DirtTile)
			{
				GameData.SetMapTile(newCoordinate, new FloorTile(newCoordinate));
				return newCoordinate;
			}

			#endregion

			#region Socket

			if (destinationItem is SocketItem)
			{
				if(GameData.ChipRequirementMet)
				{
					GameData.RemoveMapItem(newCoordinate);
					return newCoordinate;
				}

				return Coordinate;
			}

			#endregion

			#region Blue Teleporter

			if(destinationItem is BlueTeleporterItem blueTeleporter)
			{
				var oldCoordinate = Coordinate.Clone();
				var teleporterCoordinate = blueTeleporter.Coordinate;
				var currentCheckpoint = Coordinate + direction;
				while(true)
				{
					var teleportExit = blueTeleporter.SearchForExit(currentCheckpoint);
					if(teleportExit == teleporterCoordinate)
					{
						// EXIT: Search has covered the entire map.
						// Player will slide over or bounce from the teleporter.
						Coordinate = teleporterCoordinate;
						newCoordinate = ProposeMove(direction);
						if(newCoordinate == Coordinate)
						{
							// Bounce back from teleporter
							ReverseOrientation();
							return oldCoordinate;
						}

						GameData.RemoveMapPlayer(oldCoordinate);
						return newCoordinate;
					}

					Coordinate = teleportExit;
					var teleportExitStep = ProposeMove(direction);
					if(teleportExitStep != teleportExit)
					{
						// EXIT: Step is valid, teleport complete.
						GameData.RemoveMapPlayer(oldCoordinate);
						newCoordinate = teleportExitStep;
						return newCoordinate;
					}

					// Exit step is invalid. Reset and look for the next exit.
					Coordinate = oldCoordinate;
					currentCheckpoint = teleportExit;
				}
			}

			#endregion

			#region Recessed Wall

			if(destinationItem is RecessedWallItem)
			{
				GameData.RemoveMapItem(newCoordinate);
				GameData.SetMapTile(newCoordinate, new WallTile(newCoordinate));
				return newCoordinate;
			}

			#endregion

			#region Tool Thief

			if(destinationItem is ToolThief)
			{
				GameData.StealEquipment();
			}

			#endregion

			return newCoordinate;
		}


		public override void SetCoordinate(Vector2I newCoordinate)
		{
			if(Coordinate == newCoordinate)
			{
				return;
			}

			GameData.RemoveMapPlayer(Coordinate);
			Coordinate = newCoordinate;
			GameData.MapPlayers[newCoordinate.X, newCoordinate.Y] = this;
			GameData.PlayerCoordinate = newCoordinate;
			GameData.RerenderMap = true;
		}
	}
}
