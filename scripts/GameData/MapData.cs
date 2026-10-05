using GalaxyGauntlet.scripts.MapEntities;
using GalaxyGauntlet.scripts.MapEntities.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GalaxyGauntlet.scripts
{
    /// <summary>
    /// Data pertaining to the currently loaded map
    /// </summary>
    public static partial class GameData
    {
        public static int TimeLimit = -1;
        public static Vector2I MapDimensions = -Vector2I.One;
        public static byte[] MapData = [];
        public static MapEntity[,] MapScenery = new MapEntity[0, 0];
        public static MapMob[,] MapMobs = new MapMob[0, 0];
        public static MapMob[,] MapPlayers = new MapMob[0, 0];
        public static MapItem[,] MapItems = new MapItem[0, 0];
        public static MapTile[,] MapTiles = new MapTile[0, 0];
        public static List<MapMob> PlayerProcessList = [];
        public static List<MapMob> PlayerSliplist = [];
        public static List<MapMob> MobProcessList = [];
        public static List<MapMob> MobSlipList = [];
        public static List<MapButtonItem> ButtonsToProcess = [];

        /// <summary>
        /// Alternates between 0 and 1 each process tick, mimicking the odd- and even-step mechanics in Chip's Challenge.
        /// </summary>
        public static int PingPongStep = 0;

        /// <summary>
        /// Rotates from 0 to 3, allowing entities with 1.25 moves per second to be processed accurately.
        /// </summary>
        public static int SquareStep = 0;

        /// <summary>
        /// Flags the level map to be rendered on the next frame.
        /// </summary>
        public static bool RerenderMap = false;
        private static int NextMobID = 0;

        public static void ResetMapData()
        {
            TimeLimit = -1;
            RerenderMap = false;
            MapDimensions = -Vector2I.One;
            NextMobID = 0;
            PlayerProcessList = [];
            PlayerSliplist = [];
            MobProcessList = [];
            MobSlipList = [];
            ButtonsToProcess = [];
            MapData = [];
            MapScenery = new MapEntity[0, 0];
            MapMobs = new MapMob[0, 0];
            MapPlayers = new MapMob[0, 0];
            MapItems = new MapItem[0, 0];
            MapTiles = new MapTile[0, 0];
        }


        public static void LoadCurrentLevel()
        {
            ResetMapData();
            ResetLevelData();
            string mapPath = $"./levels/map{CurrentLevelNumber:000}.c2m";
            LevelLoader.LoadLevel(mapPath);
            RerenderMap = true;
        }


        /// <summary>
        /// Populate entity lists based off of unpacked C2M file data
        /// </summary>
        public static void PopulateFromMapBytes()
        {
            if (MapData.Length == 0)
            {
                FileLogger.QuietLogMessage("No map data to process.");
                return;
            }

            int x = 0;
            int y = 0;
            for (int i = 0; i < MapData.Length; i++)
            {
                Vector2I mapCoordinate = new(x, y);
                var entity = MapEntity.GetMapEntity(MapData[i], mapCoordinate);
                if(entity is Player)
                {
                    PlayerCoordinate = mapCoordinate;
                }
                if (entity is MapTile tile)
                {
                    MapTiles[x, y] = tile;
                }
                if(entity is ThinWallOrCanopyTile thinWall)
                {
                    i++;
                    if (MapData[i-1] == Constants.ByteCodes.Entities.ThinWallOrCanopy)
                    {
                        // Parse Thin Wall as described in CC2
                        thinWall.SetOrientation(MapData[i]);
                        if (MapData[i] == Constants.ByteCodes.ThinWallBits.Canopy)
                        {
                            // TODO: Set canopy scenery
                        }
                        i++;
                    }
                    MapTiles[x, y] = thinWall;
                }
                else if (entity is MapDoorItem doorItem)
                {
                    MapItems[x, y] = doorItem;
                    MapTiles[x, y] = new WallTile(mapCoordinate);
                }
                else if (entity is MapItem item)
                {
                    if (entity.NeedsTileSpecification)
                    {
                        i++;
                        if (MapData[i] == Constants.ByteCodes.Entities.DirtBlock)
                        {
                            // Item is hidden under a dirt block.
                            // Chip's Challenge treats dirt blocks like tiles, but it's better to treat it like a mob.
                            DirtBlockEntity dirtBlock = new(mapCoordinate);
                            i++;
                            dirtBlock.Orientation = (EntityOrientation)MapData[i++];
                            MobProcessList.Add(dirtBlock);
                            MapMobs[x, y] = dirtBlock;
                        }
                        // TODO: Generalize to all monsters
                        if (MapData[i] == Constants.ByteCodes.Entities.Teeth)
                        {
                            // Item is hidden under a Teeth
                            TeethMonster teeth = new(mapCoordinate);
                            i++;
                            teeth.Orientation = (EntityOrientation)MapData[i++];
                            MobProcessList.Add(teeth);
                            MapMobs[x, y] = teeth;
                        }
                        item.LowerLayer = (MapTile)MapEntity.GetMapEntity(MapData[i], mapCoordinate);
                    }
                    MapTiles[x, y] = item.LowerLayer;
                    MapItems[x, y] = item;

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
                        mob.Orientation = (EntityOrientation)MapData[i];
                    }
                    if (mob.NeedsTileSpecification)
                    {
                        i++;
                        var mapEntity = MapEntity.GetMapEntity(MapData[i], mapCoordinate);
                        if(mapEntity is MapButtonItem mapButton)
                        {
                            MapItems[x, y] = mapButton;
                            ButtonsToProcess.Add(mapButton);
                            mob.LowerLayer = new FloorTile(mapCoordinate);
                        }
                        else if(mapEntity is MapItem mapItem)
                        {
                            MapItems[x, y] = mapItem;
                            mob.LowerLayer = new FloorTile(mapCoordinate);
                        }
                        else
                        {
                            mob.LowerLayer = (MapTile)mapEntity;
                        }
                    }
                    if(mob is not Player
                        && mob.LowerLayer is not CloneMachineTile)
                    {
                        // Entities in the clone machine do not get processed.
                        MobProcessList.Add(mob);
                    }
                    if (entity is Player player)
                    {
                        PlayerProcessList.Add(player);
                        MapPlayers[x, y] = mob;
                    }
                    else
                    {
                        MapMobs[x, y] = mob;
                    }
                    MapTiles[x, y] = mob.LowerLayer;
                }

                // Move to next coordinate
                x++;
                if (x >= MapDimensions.X)
                {
                    x = 0;
                    y++;
                }
                if (y >= MapDimensions.Y)
                {
                    return;
                }
            }
        }


        public static void SetMapDimensions(int x, int y)
        {
            MapDimensions = new(x, y);
            MapScenery = new MapEntity[x, y];
            MapMobs = new MapMob[x, y];
            MapPlayers = new MapMob[x, y];
            MapItems = new MapItem[x, y];
            MapTiles = new MapTile[x, y];

            for (int i = 0; i < MapDimensions.X; i++)
            {
                for (int j = 0; j < MapDimensions.Y; j++)
                {
                    MapScenery[i, j] = null;
                    MapMobs[i, j] = null;
                    MapPlayers[i, j] = null;
                    MapItems[i, j] = null;
                    MapTiles[i, j] = null;
                }
            }
        }


        public static int GetNextMobID()
        {
            NextMobID++;
            return NextMobID;
        }


        public static void ProcessMobList(List<MapMob> mobs)
        {
            foreach(var mob in mobs)
            {
                if(mob.CanProcess)
                {
                    mob.ProcessTick();
                }
                if(false == PlayerAlive)
                {
                    break;
                }
            }
        }


        /// <summary>
        /// Moves a mob from the regular processing queue to the slip list queue
        /// </summary>
        public static void AddToSlipList(MapMob mob)
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
        public static void RemoveFromSlipList(MapMob mob)
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


        public static void FlipGreenToggles()
        {
            foreach(var tile in MapTiles)
            {
                if(tile is GreenToggleTile toggleTile)
                {
                    toggleTile.IsWall = !toggleTile.IsWall;
                }
            }
            RerenderMap = true;
        }


        public static void FlipBlueTanks()
        {
            foreach(var mob in MapMobs)
            {
                if(mob is BlueTankMonster blueTank)
                {
                    blueTank.AboutFace();
                }
            }
        }

        #region Map Tile Management

        #region Scenery

        public static MapEntity GetMapScenery(Vector2I coordinate)
        {
            return MapScenery[coordinate.X, coordinate.Y];
        }


        public static void RemoveMapScenery(Vector2I coordinate)
        {
            MapScenery[coordinate.X, coordinate.Y] = null;
            RerenderMap = true;
        }

        #endregion


        #region Mobs

        public static void AddMapMob(MapMob mob)
        {
            var coordinate = mob.Coordinate;
            if (MapMobs[coordinate.X, coordinate.Y] != null)
            {
                return;
            }

            MapMobs[coordinate.X, coordinate.Y] = mob;
            MobProcessList.Add(mob);
            RerenderMap = true;
        }

        public static MapMob GetMapMob(Vector2I coordinate)
        {
            return MapMobs[coordinate.X, coordinate.Y];
        }


        public static void RemoveMapMob(Vector2I coordinate)
        {
            MapMobs[coordinate.X, coordinate.Y] = null;
            RerenderMap = true;
        }

        #endregion


        #region Player

        public static MapMob GetMapPlayer(Vector2I coordinate)
        {
            return MapPlayers[coordinate.X, coordinate.Y];
        }


        public static void RemoveMapPlayer(Vector2I coordinate)
        {
            MapPlayers[coordinate.X, coordinate.Y] = null;
            RerenderMap = true;
        }

        #endregion


        #region Item

        public static MapItem GetMapItem(Vector2I coordinate)
        {
            return MapItems[coordinate.X, coordinate.Y];
        }


        public static void RemoveMapItem(Vector2I coordinate)
        {
            MapItems[coordinate.X, coordinate.Y] = null;
            RerenderMap = true;
        }

        #endregion


        #region Tile

        public static MapTile GetMapTile(Vector2I coordinate)
        {
            try
            {
                return MapTiles[coordinate.X, coordinate.Y];
            }
            catch(IndexOutOfRangeException)
            {
                return new WallTile(coordinate);
            }
        }


        public static void SetMapTile(Vector2I coordinate, MapTile newTile)
        {
            MapTiles[coordinate.X, coordinate.Y] = newTile;
            RerenderMap = true;
        }


        public static void RemoveMapTile(Vector2I coordinate)
        {
            MapTiles[coordinate.X, coordinate.Y] = null;
            RerenderMap = true;
        }

        #endregion

        #endregion
    }
}
