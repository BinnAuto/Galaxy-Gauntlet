using GalaxyGauntlet.scripts.MapEntities;
using GalaxyGauntlet.scripts.MapEntities.Shared;
using System.Collections.Generic;
using System.Linq;

namespace GalaxyGauntlet.scripts
{
    /*
     * Contains communication methods between different properties of GameData
     */
    public static partial class GameData
    {
        public static void Initialize()
        {
            Archipelago = new();
            Archipelago.OnDeathMessage += OnSetDeathMessage;
            Archipelago.OnItemReceived += OnArchipelagoItemReceived;
            Map = new();
            Map.OnPlayerCoordinateSet += OnPlayerCoordinateSet;
            Level = new();
            LoadSettings();
        }

        #region Settings

        // Audio Settings
        public static double MasterVolume = 50;

        public static double MusicVolume = 50;

        private const string GameConfigFilePath = "user://gamedata.config";

        private const string GameDataConfigSection = "GameData";

        private static ConfigFile ConfigFile = new();

        public static void LoadSettings()
        {
            var loadResult = ConfigFile.Load(GameConfigFilePath);
            if (loadResult == Error.Ok)
            {
                Archipelago.GameName = (string)ConfigFile.GetValue(GameDataConfigSection, Constants.Settings.ArchipelagoGameName, "Chip's Challenge");
                Archipelago.Server = (string)ConfigFile.GetValue(GameDataConfigSection, Constants.Settings.ArchipelagoServer, "archipelago.gg");
                Archipelago.Port = (int)ConfigFile.GetValue(GameDataConfigSection, Constants.Settings.ArchipelagoPort, string.Empty);
                Archipelago.SlotName = (string)ConfigFile.GetValue(GameDataConfigSection, Constants.Settings.ArchipelagoSlotName, string.Empty);
                Archipelago.Password = (string)ConfigFile.GetValue(GameDataConfigSection, Constants.Settings.ArchipelagoPassword, string.Empty);
                MasterVolume = (double)ConfigFile.GetValue(GameDataConfigSection, Constants.Settings.MasterVolume, 50);
                MusicVolume = (double)ConfigFile.GetValue(GameDataConfigSection, Constants.Settings.MusicVolume, 50);
            }
            SaveSettings();
        }


        public static void SaveSettings()
        {
            ConfigFile.SetValue(GameDataConfigSection, Constants.Settings.ArchipelagoGameName, Archipelago.GameName);
            ConfigFile.SetValue(GameDataConfigSection, Constants.Settings.ArchipelagoServer, Archipelago.Server);
            ConfigFile.SetValue(GameDataConfigSection, Constants.Settings.ArchipelagoPort, Archipelago.Port.ToString());
            ConfigFile.SetValue(GameDataConfigSection, Constants.Settings.ArchipelagoSlotName, Archipelago.SlotName);
            ConfigFile.SetValue(GameDataConfigSection, Constants.Settings.ArchipelagoPassword, Archipelago.Password);
            ConfigFile.SetValue(GameDataConfigSection, Constants.Settings.MasterVolume, MasterVolume);
            ConfigFile.SetValue(GameDataConfigSection, Constants.Settings.MusicVolume, MusicVolume);
            ConfigFile.Save(GameConfigFilePath);
            GGG.SetBusVolume(Constants.AudioBuses.Master, MasterVolume);
            GGG.SetBusVolume(Constants.AudioBuses.Music, MusicVolume);
        }

        #endregion

        #region Global Methods

        public static void LoadCurrentLevel()
        {
            Map.Reset();
            Level.Reset();
            string mapPath = Level.LevelSet[Level.CurrentLevelIndex];
            LevelLoader.LoadLevel(mapPath);
            Map.Rerender = true;
        }


        public static void ProcessMobList(List<MapMob> mobs)
        {
            foreach (var mob in mobs)
            {
                if (mob.CanProcess)
                {
                    mob.ProcessTick();
                }
                if (false == Level.PlayerAlive)
                {
                    break;
                }
            }
        }

        public static void RemoveMapItem(Vector2I coordinate)
        {
            Map.RemoveItem(coordinate);
        }


        private static void OnSetDeathMessage(string message)
        {
            Level.DeathMessage = message;
        }




        public static void OnLevelComplete()
        {
            Level.ProcessMobs = false;
            Level.AcceptingPlayerInput = false;
            Level.TimerEnabled = false;
            if (Level.GameMode == GameMode.Archipelago)
            {
                string location = $"Complete Map {Level.CurrentLevelIndex + 1} - {Level.Name}";
                Archipelago.SendLocationCheck(location);
                if (Level.RestartCount == 0)
                {
                    location += " (No Resets)";
                    Archipelago.SendLocationCheck(location);
                }
            }
        }


        public static void OnChipCollected(ChipItem chip)
        {
            Level.ChipsCollected++;
            if (Level.GameMode is GameMode.Archipelago)
            {
                string locationName = chip.GetArchipelagoLocationName();
                Archipelago.SendLocationCheck(locationName);
            }
            RemoveMapItem(chip.Coordinate);
        }


        public static void OnPlayerDeath()
        {
            Level.ProcessMobs = false;
            Level.AcceptingPlayerInput = false;
            Level.TimerEnabled = false;
            Level.PlayerAlive = false;
            Archipelago.SendDeathLinkMessage();
        }


        public static bool PlayerHasPendingItem(int dataCode)
        {
            return Archipelago.PendingItems.Any(e => e.DataCode == dataCode);
        }


        public static int PendingItemCount(int dataCode)
        {
            return Archipelago.PendingItems.Count(e => e.DataCode == dataCode);
        }


        private static void OnArchipelagoItemReceived(string itemName)
        {
            switch (itemName)
            {
                case Constants.Archipelago.TrapNames.ConfusionTrap:
                    RandomNumberGenerator rng = new();
                    Level.ConfusionTimer = rng.RandiRange(20, 40);
                    break;

                case Constants.Archipelago.ItemNames.TimeBonus:
                    if (Map.TimeLimit == 0)
                    {
                        // Turn untimed levels into timed levels (hurry up!!!)
                        Map.TimeLimit = 10;
                    }
                    Level.Timer += 10;
                    break;

                case Constants.Archipelago.TrapNames.TimePenalty:
                    if (Map.TimeLimit == 0)
                    {
                        // Instant death on untimed levels
                        Level.DeathMessage = Constants.DeathMessages.OutOfTime;
                        return;
                    }

                    Level.Timer -= 10;
                    break;

                case Constants.Archipelago.TrapNames.WalkerTrap:
                    Map.AddMobAtRandom(new WalkerMonster(Vector2I.Zero));
                    break;

                case Constants.Archipelago.TrapNames.BlobTrap:
                    Map.AddMobAtRandom(new BlobMonster(Vector2I.Zero));
                    break;

                case Constants.Archipelago.TrapNames.TeethTrap:
                    Map.AddMobAtRandom(new TeethMonster(Vector2I.Zero));
                    break;

                case Constants.Archipelago.ItemNames.BigMap:
                    Level.MapViewportSize = Mathf.Min(15, Level.MapViewportSize + 6);
                    break;

                case Constants.Archipelago.TrapNames.SmallMapTrap:
                    Level.MapViewportSize = Mathf.Max(5, Level.MapViewportSize - 4);
                    break;

                default:
                    // Item is either a progression item or has already
                    // been processed in the Archipelago subclass.
                    break;
            }
        }


        private static void OnPlayerCoordinateSet(Vector2I coordinate)
        {
            Level.PlayerCoordinate = coordinate;
        }

        #endregion
    }
}
