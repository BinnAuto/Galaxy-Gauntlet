using GalaxyGauntlet.scripts.MapEntities;
using GalaxyGauntlet.scripts.MapEntities.Mobs;
using GalaxyGauntlet.scripts.MapEntities.Shared;
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


        #region Map Layout Array

        public MapEntity[,] Scenery = new MapEntity[0, 0];
        public MapMob[,] Mobs = new MapMob[0, 0];
        public MapMob[,] Players = new MapMob[0, 0];
        public MapItem[,] Items = new MapItem[0, 0];
        public MapTile[,] Tiles = new MapTile[0, 0];

        #endregion


        #region Entity Process Lists

        public List<MapMob> PlayerProcessList = [];
        public List<MapMob> PlayerSliplist = [];
        public List<MapMob> MobProcessList = [];
        public List<MapMob> MobSlipList = [];
		public List<MapButtonItem> ButtonsToProcess = [];

		#endregion

		/// <summary>
		/// Alternates between 0 and 1 each process tick, mimicking the odd- and even-step mechanics in Chip's Challenge.
		/// </summary>
		public int PingPongStep = 0;

        /// <summary>
        /// Rotates from 0 to 3, allowing entities with 1.25 moves per second to be processed accurately.
        /// </summary>
        public int SquareStep = 0;

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

		public event OnPlayerCoordinateSetHandler OnPlayerCoordinateSet = (coordinate) => { };

		#endregion

		public void Reset()
        {
            TimeLimit = -1;
            Rerender = false;
            Dimensions = -Vector2I.One;
            NextMobID = 0;
            PlayerProcessList = [];
            PlayerSliplist = [];
            MobProcessList = [];
            MobSlipList = [];
            ButtonsToProcess = [];
            FileBytes = [];
            Scenery = new MapEntity[0, 0];
            Mobs = new MapMob[0, 0];
            Players = new MapMob[0, 0];
            Items = new MapItem[0, 0];
            Tiles = new MapTile[0, 0];
        }


		/// <summary>
		/// Populate entity lists based off of unpacked C2M file data
		/// </summary>
		public void PopulateFromFileBytes()
		{
			if (FileBytes.Length == 0)
			{
				FileLogger.QuietLogMessage("No map data to process.");
				return;
			}

			int x = 0;
			int y = 0;
			for (int i = 0; i < FileBytes.Length; i++)
			{
				Vector2I mapCoordinate = new(x, y);
				var entity = MapEntity.GetMapEntity(FileBytes[i], mapCoordinate);
				if(entity is Player)
				{
					OnPlayerCoordinateSet.Invoke(mapCoordinate);
				}
				if (entity is MapTile tile)
				{
					Tiles[x, y] = tile;
				}
				if(entity is ThinWallOrCanopyTile thinWall)
				{
					i++;
					if (FileBytes[i-1] == Constants.ByteCodes.Entities.ThinWallOrCanopy)
					{
						// Parse Thin Wall as described in CC2
						thinWall.SetOrientation(FileBytes[i]);
						if (FileBytes[i] == Constants.ByteCodes.ThinWallBits.Canopy)
						{
							// TODO: Set canopy scenery
						}
						i++;
					}
					Tiles[x, y] = thinWall;
				}
				else if (entity is MapDoorItem doorItem)
				{
					Items[x, y] = doorItem;
					Tiles[x, y] = new WallTile(mapCoordinate);
				}
				else if (entity is MapItem item)
				{
					if (entity.NeedsTileSpecification)
					{
						i++;
						if (FileBytes[i] == Constants.ByteCodes.Entities.DirtBlock)
						{
							// Item is hidden under a dirt block.
							// Chip's Challenge treats dirt blocks like tiles, but it's better to treat it like a mob.
							DirtBlockEntity dirtBlock = new(mapCoordinate);
							i++;
							dirtBlock.Orientation = (EntityOrientation)FileBytes[i++];
							MobProcessList.Add(dirtBlock);
							Mobs[x, y] = dirtBlock;
						}
						// TODO: Generalize below to all monsters
						if (FileBytes[i] == Constants.ByteCodes.Entities.Teeth)
						{
							// Item is hidden under a Teeth
							TeethMonster teeth = new(mapCoordinate);
							i++;
							teeth.Orientation = (EntityOrientation)FileBytes[i++];
							MobProcessList.Add(teeth);
							Mobs[x, y] = teeth;
						}
						item.LowerLayer = (MapTile)MapEntity.GetMapEntity(FileBytes[i], mapCoordinate);
					}
					Tiles[x, y] = item.LowerLayer;
					Items[x, y] = item;

					if(item is MapButtonItem button)
					{
						ButtonsToProcess.Add(button);
					}
				}
				else if (entity is MapMob mob)
				{
					mob.Orientation = EntityOrientation.North;
					if (mob.NeedsOrientation)
					{
						i++;
						mob.Orientation = (EntityOrientation)FileBytes[i];
					}
					if (mob.NeedsTileSpecification)
					{
						i++;
						var mapEntity = MapEntity.GetMapEntity(FileBytes[i], mapCoordinate);
						if(mapEntity is MapButtonItem mapButton)
						{
							Items[x, y] = mapButton;
							ButtonsToProcess.Add(mapButton);
							mob.LowerLayer = new FloorTile(mapCoordinate);
						}
						else if(mapEntity is MapItem mapItem)
						{
							Items[x, y] = mapItem;
							mob.LowerLayer = new FloorTile(mapCoordinate);
						}
						else
						{
							mob.LowerLayer = (MapTile)mapEntity;
						}
						if(mob.LowerLayer is CloneMachineTile)
						{
							mob = new CloneMachineEntity(mapCoordinate, mob.DataCode)
							{
								LowerLayer = new CloneMachineTile(mapCoordinate),
								Orientation = mob.Orientation
							};
						}
					}
					if(mob is not Player)
					{
						MobProcessList.Add(mob);
					}
					if (entity is Player player)
					{
						mob.Orientation = EntityOrientation.South;
						PlayerProcessList.Add(player);
						Players[x, y] = mob;
					}
					else
					{
						Mobs[x, y] = mob;
					}
					Tiles[x, y] = mob.LowerLayer;
				}

				// Move to next coordinate
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
			}
		}


		public void SetDimensions(int x, int y)
		{
			Dimensions = new(x, y);
			Scenery = new MapEntity[x, y];
			Mobs = new MapMob[x, y];
			Players = new MapMob[x, y];
			Items = new MapItem[x, y];
			Tiles = new MapTile[x, y];

			for (int i = 0; i < Dimensions.X; i++)
			{
				for (int j = 0; j < Dimensions.Y; j++)
				{
					Scenery[i, j] = null;
					Mobs[i, j] = null;
					Players[i, j] = null;
					Items[i, j] = null;
					Tiles[i, j] = null;
				}
			}
		}


		public int GetNextMobID()
		{
			NextMobID++;
			return NextMobID;
		}


		/// <summary>
		/// Moves a mob from the regular processing queue to the slip list queue
		/// </summary>
		public void AddToSlipList(MapMob mob)
		{
			if(mob is Player)
			{
				PlayerProcessList = [..PlayerProcessList.Where(e => e.MobID != mob.MobID)];
				if(PlayerSliplist.Any(e => e.MobID == mob.MobID))
				{
					return;
				}

				PlayerSliplist.Add(mob);
			}
			else
			{
				MobProcessList = [..MobProcessList.Where(e => e.MobID != mob.MobID)];
				if(MobSlipList.Any(e => e.MobID == mob.MobID))
				{
					return;
				}
			
				MobSlipList.Add(mob);
			}
		}


		/// <summary>
		/// Moves a mob from the slip list processing queue to the regular queue
		/// </summary>
		public void RemoveFromSlipList(MapMob mob)
		{
			if(mob is Player)
			{
				PlayerSliplist = [..PlayerSliplist.Where(e => e.MobID != mob.MobID)];
				if(PlayerProcessList.Any(e => e.MobID == mob.MobID))
				{
					return;
				}

				PlayerProcessList.Add(mob);
			}
			else
			{
				MobSlipList = [..MobSlipList.Where(e => e.MobID != mob.MobID)];
				if(MobProcessList.Any(e => e.MobID == mob.MobID))
				{
					return;
				}

				MobProcessList.Add(mob);
			}
		}


		public void FlipGreenToggles()
		{
			foreach(var tile in Tiles)
			{
				if(tile is GreenToggleTile toggleTile)
				{
					toggleTile.IsWall = !toggleTile.IsWall;
				}
			}
			Rerender = true;
		}


		public void FlipBlueTanks()
		{
			foreach(var mob in Mobs)
			{
				if(mob is BlueTankMonster blueTank)
				{
					blueTank.AboutFace();
				}
			}
		}

		#region Map Tile Management

		#region Scenery

		public MapEntity GetSceneryAtCoordinate(Vector2I coordinate)
		{
			return Scenery[coordinate.X, coordinate.Y];
		}


		public void RemoveSceneryFromCoordinate(Vector2I coordinate)
		{
			Scenery[coordinate.X, coordinate.Y] = null;
			Rerender = true;
		}

		#endregion


		#region Mobs

		/// <summary>
		/// Adds a mob at a random valid location in the map
		/// </summary>
		public void AddMobAtRandom(MapMob mob)
		{
			int attempts = 0;
			RandomNumberGenerator rng = new();
			while(attempts < 1000)
			{
				attempts++;
				Vector2I mapCoordinate = new(
					rng.RandiRange(0, Dimensions.X - 1),
					rng.RandiRange(0, Dimensions.Y - 1)
				);
				var mapTile = GetTile(mapCoordinate);
				if(mapTile is not FloorTile
					&& mapTile is not MapForceFloorTile)
				{
					continue;
				}

				var mapItem = GetItem(mapCoordinate);
				if(mapItem is not null
					&& mapItem is not MapButtonItem
					&& mapItem is not TrapItem)
				{
					continue;
				}

				var mapMob = GetMob(mapCoordinate);
				if(mapMob is not null)
				{
					continue;
				}

				var mapPlayer = GetPlayer(mapCoordinate);
				if(mapPlayer is not null)
				{
					continue;
				}

				mob.Coordinate = mapCoordinate;
				AddMob(mob);
				break;
			}
		}


		public void AddMob(MapMob mob)
		{
			var coordinate = mob.Coordinate;
			if (Mobs[coordinate.X, coordinate.Y] != null)
			{
				return;
			}

			Mobs[coordinate.X, coordinate.Y] = mob;
			MobProcessList.Add(mob);
			Rerender = true;
		}

		public MapMob GetMob(Vector2I coordinate)
		{
			return Mobs[coordinate.X, coordinate.Y];
		}


		public void RemoveMob(Vector2I coordinate)
		{
			Mobs[coordinate.X, coordinate.Y] = null;
			Rerender = true;
		}

		#endregion


		#region Player

		public MapMob GetPlayer(Vector2I coordinate)
		{
			return Players[coordinate.X, coordinate.Y];
		}


		public void RemovePlayer(Vector2I coordinate)
		{
			Players[coordinate.X, coordinate.Y] = null;
			Rerender = true;
		}

		#endregion


		#region Item

		public MapItem GetItem(Vector2I coordinate)
		{
			return Items[coordinate.X, coordinate.Y];
		}


		public void RemoveItem(Vector2I coordinate)
		{
			Items[coordinate.X, coordinate.Y] = null;
			Rerender = true;
		}

		#endregion


		#region Tile

		public MapTile GetTile(Vector2I coordinate)
		{
			try
			{
				return Tiles[coordinate.X, coordinate.Y];
			}
			catch(IndexOutOfRangeException)
			{
				return new WallTile(coordinate);
			}
		}


		public void SetTile(Vector2I coordinate, MapTile newTile)
		{
			Tiles[coordinate.X, coordinate.Y] = newTile;
			Rerender = true;
		}


		public void RemoveTile(Vector2I coordinate)
		{
			Tiles[coordinate.X, coordinate.Y] = null;
			Rerender = true;
		}

		#endregion

		#endregion
	}
}
