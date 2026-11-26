using System;
using System.Collections.Generic;
using System.Linq;

namespace GalaxyGauntlet.scripts
{
	public static partial class GameData
	{
		public static MapData Map = new();
	}


	public class MapData
	{
		private int NextMobID = 0;
		public int TimeLimit = -1;
		public Vector2I Dimensions = -Vector2I.One;
		public byte[] FileBytes = [];
		private List<MapObject2> NewMapObjects = [];
		private List<MapCreature2> CreaturesToProcess = [];
		private List<MapLynxCreature2> LynxCreaturesToProcess = [];

		#region Map Layout Array

		// 2
		public MapCreature2[,] Creatures2 = new MapCreature2[0, 0];
		public MapLynxCreature2[,] LynxCreatures2 = new MapLynxCreature2[0, 0];
		public MapItem2[,] Items2 = new MapItem2[0, 0];
		public ThinWallOrCanopy2[,] ThinWalls2 = new ThinWallOrCanopy2[0,0];
		public MapTile2[,] Tiles2 = new MapTile2[0, 0];

		#endregion

		/// <summary>
		/// Flags the level map to be rendered on the next frame.
		/// </summary>
		public bool Rerender = false;

		/*
		 * Contains event handlers to allow Map events to communicate with
		 * other areas in GameData
		 */
		#region External Event Emitters

		public delegate void OnPlayerCoordinateSetHandler(Vector2I coordinate);

		#endregion

		public void Reset()
		{
			TimeLimit = -1;
			Rerender = false;
			Dimensions = -Vector2I.One;
			NextMobID = 0;
			FileBytes = [];
			Tiles2 = new MapTile2[0, 0];
			Items2 = new MapItem2[0, 0];
			ThinWalls2 = new ThinWallOrCanopy2[0, 0];
			Creatures2 = new MapCreature2[0, 0];
			LynxCreatures2 = new MapLynxCreature2[0, 0];
			CreaturesToProcess.Clear();
			LynxCreaturesToProcess.Clear();
		}


		#region Map Load Methods

		/// <summary>
		/// Populate entity lists based off of unpacked C2M file data
		/// </summary>
		public void PopulateFromFileBytes2()
		{
			if(FileBytes.Length == 0)
			{
				FileLogger.QuietLogMessage("No map data to process");
				return;
			}

			int x = 0;
			int y = 0;
			for(int i = 0; i < FileBytes.Length; i++)
			{
				Vector2I mapCoordinate = new(x, y);
				var mapObject = GetMapObject(ref i, mapCoordinate);
				AddMapObject(mapObject);
				if(mapObject is MapItem2 mapItem)
				{
					Items2[x, y] = mapItem;
					if(mapItem.RequiresLowerLayerTile)
					{
						i++;
						mapObject = GetMapObject(ref i, mapCoordinate);
					}
					else
					{
						mapObject = new FloorTile2(mapCoordinate);
					}
					AddMapObject(mapObject);
				}
				if(mapObject is ThinWallOrCanopy2 thinWallCanopy)
				{
					if(thinWallCanopy.WallBitmask is null)
					{
						i++;
						byte bitmaskByte = FileBytes[i];
						thinWallCanopy.WallBitmask = bitmaskByte;
					}
					ThinWalls2[x, y] = thinWallCanopy;
					if (thinWallCanopy.RequiresLowerLayerTile)
					{
						i++;
						mapObject = GetMapObject(ref i, mapCoordinate);
						AddMapObject(mapObject);
					}
				}
				if(mapObject is MapCreature2 creature)
				{
					if (creature.RequiresDirectionByte)
					{
						i++;
						byte directionByte = FileBytes[i];
						creature.Orientation = (EntityOrientation)directionByte;
					}
					if(creature is Player2)
					{
						GameData.Level.PlayerCoordinate = mapCoordinate;
						creature.Orientation = EntityOrientation.South;
						CreaturesToProcess = CreaturesToProcess.Prepend(creature).ToList();
					}
					else
					{
						AddCreatureToProcess(creature);
					}
					Creatures2[x, y] = creature;

					if(creature.RequiresLowerLayerTile)
					{
						i++;
						mapObject = GetMapObject(ref i, mapCoordinate);
					}
					else
					{
						// Shouldn't be reached, but assume creature stands on floor tile.
						mapObject = new FloorTile2(mapCoordinate);
					}
					AddMapObject(mapObject);
				}
				if (mapObject is MapLynxCreature2 lynxCreature)
				{
					if (lynxCreature.RequiresDirectionByte)
					{
						i++;
						byte directionByte = FileBytes[i];
						lynxCreature.Orientation = (EntityOrientation)directionByte;
					}
					if (lynxCreature is LynxPlayer2)
					{
						GameData.Level.PlayerCoordinate = mapCoordinate;
						lynxCreature.Orientation = EntityOrientation.South;
						LynxCreaturesToProcess = LynxCreaturesToProcess.Prepend(lynxCreature).ToList();
					}
					else
					{
						AddLynxCreatureToProcess(lynxCreature);
					}
					LynxCreatures2[x, y] = lynxCreature;

					if (lynxCreature.RequiresLowerLayerTile)
					{
						i++;
						mapObject = GetMapObject(ref i, mapCoordinate);
					}
					else
					{
						// Shouldn't be reached, but assume creature stands on floor tile.
						mapObject = new FloorTile2(mapCoordinate);
					}
					AddMapObject(mapObject);
				}
				if (mapObject is MapTile2 mapTile)
				{
					Tiles2[x, y] = mapTile;
					if (mapObject is CloneMachineTile2 cloneTile)
					{
						if (Creatures2[x, y] is not null)
						{
							Creatures2[x, y].Orientation = cloneTile.CreatureOrientation 
								?? Creatures2[x, y].Orientation;
							RemoveCreatureFromProcessList(Creatures2[x, y].CreatureID);
						}
						if (LynxCreatures2[x, y] is not null)
						{
							LynxCreatures2[x, y].Orientation = cloneTile.CreatureOrientation 
								?? LynxCreatures2[x, y].Orientation;
							RemoveLynxCreatureFromProcessList(LynxCreatures2[x, y].CreatureID);
						}
					}
				}
				if (LynxCreatures2[x, y] is not null 
					&& LynxCreatures2[x, y] is not LynxRedBombCreature2
					&& LynxCreatures2[x, y] is not LynxPlayer2)
				{
					LynxCreatures2[x, y].SetTileClaim();
				}

				#region Iterate Coordinate

				x++;
				if (x >= Dimensions.X)
				{
					x = 0;
					y++;
				}
				if (y >= Dimensions.Y)
				{
					return;
				}

				#endregion
			}
		}


		private MapObject2 GetMapObject(ref int fileByteIndex, Vector2I mapCoordinate)
		{
			var modifierValue = GetModifierValue(ref fileByteIndex);
			var result = MapObject2.GetMapObject(FileBytes[fileByteIndex], mapCoordinate);
			if(result.AcceptsModifier)
			{
				result.AcceptModifier(modifierValue);
			}
			return result;
		}


		private byte[] GetModifierValue(ref int index)
		{
			// Get modifier size
			int modifierSize = FileBytes[index] switch
			{
				0x76 => 1,
				0x77 => 2,
				0x78 => 4,
				_ => 0
			};
			if(modifierSize == 0)
			{
				return [];
			}

			// Build modifier values
			byte[] result = new byte[modifierSize];
			for (int i = 0; i < modifierSize; i++)
			{
				index++;
				result[i] = FileBytes[index];
			}
			index++;
			return result;
		}

		#endregion

		public bool IsWithinBoundaries(Vector2I coordinate)
		{
			if(coordinate.X < 0 || coordinate.Y < 0)
			{
				return false;
			}
			if(coordinate.X >= Dimensions.X || coordinate.Y >= Dimensions.Y)
			{
				return false;
			}

			return true;
		}


		public void SetDimensions(int x, int y)
		{
			Dimensions = new(x, y);
			Tiles2 = new MapTile2[x, y];
			Items2 = new MapItem2[x, y];
			ThinWalls2 = new ThinWallOrCanopy2[x, y];
			Creatures2 = new MapCreature2[x, y];
			LynxCreatures2 = new MapLynxCreature2[x, y];

			for (int i = 0; i < Dimensions.X; i++)
			{
				for (int j = 0; j < Dimensions.Y; j++)
				{
					Tiles2[i, j] = null;
					Items2[i, j] = null;
					ThinWalls2[i, j] = null;
					Creatures2[i, j] = null;
					LynxCreatures2[i, j] = null;
				}
			}
		}


		public int GetNextCreatureID()
		{
			NextMobID++;
			return NextMobID;
		}


		public void AddMapObject(MapObject2 mapObject)
		{
			mapObject.SetDisplayPosition();
			lock(NewMapObjects)
			{
				NewMapObjects.Add(mapObject);
			}
		}


		public List<MapObject2> GetNewMapObjects()
		{
			lock(NewMapObjects)
			{
				List<MapObject2> result = [.. NewMapObjects];
				NewMapObjects.Clear();
				return result;
			}
		}


		#region Map Tile Management


		#region Lynx Creatures

		public MapLynxCreature2 GetLynxCreature(Vector2I coordinate)
		{
			var creature = LynxCreatures2[coordinate.X, coordinate.Y];
			if (creature != null)
			{
				string errorMessage = $"Uh oh! The game thinks {creature.MapObjectName} is at {coordinate} but it says its at {creature.Coordinate}";
				Global.AssertEqual(coordinate, creature.Coordinate, errorMessage);
			}
			return creature;
		}


		public void AddLynxCreatureToProcess(MapLynxCreature2 creature)
		{
			LynxCreaturesToProcess.Add(creature);
		}


		public List<MapLynxCreature2> GetLynxCreaturesToProcess()
		{
			LynxCreaturesToProcess = LynxCreaturesToProcess.Where(e => false == e.IsDead).ToList();
			return [.. LynxCreaturesToProcess];
		}


		public void RemoveLynxCreatureFromProcessList(int creatureID)
		{
			LynxCreaturesToProcess = LynxCreaturesToProcess.Where(e => e.CreatureID != creatureID).ToList();
		}

		#endregion


		#region Creatures

		public MapCreature2 GetCreature(Vector2I coordinate)
		{
			var creature = Creatures2[coordinate.X, coordinate.Y];
			if(creature != null)
			{
				string errorMessage = $"Uh oh! The game thinks {creature.MapObjectName} is at {coordinate} but it says its at {creature.Coordinate}";
				Global.AssertEqual(coordinate, creature.Coordinate, errorMessage);
			}
			return creature;
		}


		public void RemoveCreature(Vector2I coordinate)
		{
			Creatures2[coordinate.X, coordinate.Y] = null;
		}


		public void AddCreatureToProcess(MapCreature2 creature)
		{
			CreaturesToProcess.Add(creature);
		}


		public List<MapCreature2> GetCreaturesToProcess()
		{
			CreaturesToProcess = CreaturesToProcess.Where(e => false == e.IsDead).ToList();
			return [.. CreaturesToProcess];
		}


		public void RemoveCreatureFromProcessList(int creatureID)
		{
			CreaturesToProcess = CreaturesToProcess.Where(e => e.CreatureID != creatureID).ToList();
		}


		public bool CloneCreature(Vector2I sourceCoordinate)
		{
			if(false == IsWithinBoundaries(sourceCoordinate))
			{
				return false;
			}

			var mapTile = GetTile2(sourceCoordinate);
			if(mapTile is not CloneMachineTile2 cloneMachine)
			{
				FileLogger.QuietLogMessage($"Huh? The tile at {sourceCoordinate} is not a cloning machine! ({mapTile.MapObjectName})");
				return false;
			}

			var creature = GetLynxCreature(sourceCoordinate);
			if(creature is null)
			{
				FileLogger.QuietLogMessage($"Whoops! There is no creature in the cloning machine at {sourceCoordinate}!");
				return false;
			}

			var cloneCreature = (MapLynxCreature2)MapObject2.GetMapObject(creature.EntityID, sourceCoordinate);
			cloneCreature.Orientation = cloneMachine.CreatureOrientation ?? cloneMachine.CreatureToClone?.Orientation ?? cloneMachine.LynxCreatureToClone.Orientation;
			if(cloneCreature is null)
			{
				return creature.AdvanceCreature(true) != 0;
			}

			if(cloneCreature.AdvanceCreature(true) <= 0)
			{
				cloneCreature.IsHidden = true;
				return false;
			}

			AddMapObject(cloneCreature);
			LynxCreatures2[cloneCreature.Coordinate.X, cloneCreature.Coordinate.Y] = cloneCreature;
			AddLynxCreatureToProcess(cloneCreature);
			LynxCreatures2[sourceCoordinate.X, sourceCoordinate.Y] = creature; // Replace display creature
			return true;
		}


		public void AddCreatureAtRandom(MapLynxCreature2 creature)
		{
			int attempts = 0;
			RandomNumberGenerator rng = new();
			while(attempts < GameData.Map.Dimensions.X * GameData.Map.Dimensions.Y)
			{
				attempts++;
				Vector2I mapCoordinate = new(
					rng.RandiRange(0, Dimensions.X - 1),
					rng.RandiRange(0, Dimensions.Y - 1)
				);
				var mapTile = GetTile2(mapCoordinate);
				if (mapTile is not FloorTile2
					&& mapTile is not ForceFloorTile2)
				{
					// Creatures can only be added on floors or force floors
					continue;
				}

				var mapItem = GetItem2(mapCoordinate);
				if(mapItem is not null)
				{
					// Creatures cannot be added on top of items.
					continue;
				}

				var mapCreature = GetLynxCreature(mapCoordinate);
				if(mapCreature is not null)
				{
					// Creatures cannot be added on top of other creatures.
					continue;
				}

				creature.Coordinate = mapCoordinate;
				AddLynxCreatureToProcess(creature);
				LynxCreatures2[mapCoordinate.X, mapCoordinate.Y] = creature;
				AddMapObject(creature);
				break;
			}
		}


		public void AddCreatureAtRandom(MapCreature2 creature)
		{
			int attempts = 0;
			RandomNumberGenerator rng = new();
			while (attempts < GameData.Map.Dimensions.X * GameData.Map.Dimensions.Y)
			{
				attempts++;
				Vector2I mapCoordinate = new(
					rng.RandiRange(0, Dimensions.X - 1),
					rng.RandiRange(0, Dimensions.Y - 1)
				);
				var mapTile = GetTile2(mapCoordinate);
				if (mapTile is not FloorTile2
					&& mapTile is not ForceFloorTile2)
				{
					// Creatures can only be added on floors or force floors
					continue;
				}

				var mapItem = GetItem2(mapCoordinate);
				if (mapItem is not null)
				{
					// Creatures cannot be added on top of an item 
					continue;
				}

				var mapCreature = GetCreature(mapCoordinate);
				if (mapCreature is not null)
				{
					// Creatures cannot be added on top of another creature
					continue;
				}

				creature.Coordinate = mapCoordinate;
				AddCreatureToProcess(creature);
				Creatures2[mapCoordinate.X, mapCoordinate.Y] = creature;
				AddMapObject(creature);
				break;
			}
		}

		#endregion


		#region Thin Walls

		public ThinWallOrCanopy2 GetThinWall(Vector2I coordinate)
		{
			if (false == IsWithinBoundaries(coordinate))
			{
				return null;
			}
			
			return ThinWalls2[coordinate.X, coordinate.Y];
		}

		#endregion


		#region Item

		public MapItem2 GetItem2(Vector2I coordinate)
		{
			if(false == IsWithinBoundaries(coordinate))
			{
				return null;
			}

			return Items2[coordinate.X, coordinate.Y];
		}


		public void SetItem2(MapItem2 item)
		{
			var itemCoordinate = item.Coordinate;
			if (false == IsWithinBoundaries(itemCoordinate))
			{
				return;
			}
			if (Items2[itemCoordinate.X, itemCoordinate.Y] is not null)
			{
				return;
			}

			AddMapObject(item);
			Items2[itemCoordinate.X, itemCoordinate.Y] = item;
		}


		public void RemoveItem2(Vector2I coordinate)
		{
			if(Items2[coordinate.X, coordinate.Y] is null)
			{
				return;
			}
			if(false == IsWithinBoundaries(coordinate))
			{
				return;
			}

			Items2[coordinate.X, coordinate.Y].Kill();
			Items2[coordinate.X, coordinate.Y] = null;
		}

		#endregion


		#region Tile

		public MapTile2 GetTile2(Vector2I coordinate)
		{
			if (false == IsWithinBoundaries(coordinate))
			{
				return new WallTile2(coordinate);
			}

			return Tiles2[coordinate.X, coordinate.Y];
		}


		public void SetTile(MapTile2 newTile)
		{
			var coordinate = newTile.Coordinate;
			if (false == IsWithinBoundaries(coordinate))
			{
				return;
			}

			Tiles2[coordinate.X, coordinate.Y].Kill();
			AddMapObject(newTile);
			Tiles2[coordinate.X, coordinate.Y] = newTile;
		}


		public bool IsTileClaimed(Vector2I coordinate)
		{
			var tile = GetTile2(coordinate);
			if(tile is null)
			{
				return true;
			}

			return tile.HasFloorState(FloorStateFlags.Claimed);
		}

		#endregion

		#endregion
	}
}
