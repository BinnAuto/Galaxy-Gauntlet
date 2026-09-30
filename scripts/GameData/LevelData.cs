using GalaxyGauntlet.scripts.MapEntities;
using GalaxyGauntlet.scripts.MapEntities.Shared;
using System.Collections.Generic;
using System.Linq;

namespace GalaxyGauntlet.scripts
{
    public enum GameMode
    {
        /// <summary>
        /// Standard gameplay mode, sequentially playing through every level in the directory.
        /// </summary>
        Campaign,

        /// <summary>
        /// Playing a selected level.
        /// </summary>
        QuickPlay,

        /// <summary>
        /// Playing in Archipelago mode.
        /// </summary>
        Archipelago
    }

    /// <summary>
    /// Data pertaining to the current level and gameplay
    /// </summary>
    public static partial class GameData
    {
        public static GameMode GameMode = GameMode.Campaign;
        public static int MapViewportSize = 9;
        public static int ChipsRequired = 0;
        public static int ChipsCollected = 0;
        public static int CurrentLevelNumber = 1;

        public static string LevelName = string.Empty;
        public static int LevelRestarts = 0;
        public static float Timer = 0;
        public static bool TimerEnabled = false;
        public static bool AcceptingPlayerInput = true;
        public static bool ProcessMobs = false;
        public static bool PlayerAlive = true;
        public static string DeathMessage = string.Empty;
        public static Vector2I PlayerCoordinate = Vector2I.Zero;
        public static List<MapItem> PlayerItems = [];

        public static bool ChipRequirementMet
        {
            get
            {
                return ChipsCollected >= ChipsRequired;
            }
        }


        public static int TimerDisplay
        {
            get
            {
                return (int)Timer;
            }
        }


        public static void ResetLevelData()
        {
            PlayerAlive = true;
            DeathMessage = string.Empty;
            LevelName = string.Empty;
            ProcessMobs = false;
            TimerEnabled = false;
            AcceptingPlayerInput = true;

            ChipsRequired = 0;
            ChipsCollected = 0;
            PlayerItems = [];
            PlayerCoordinate = Vector2I.Zero;
        }


        public static void OnLevelComplete()
        {
            ProcessMobs = false;
            AcceptingPlayerInput = false;
            TimerEnabled = false;
            if (GameMode == GameMode.Archipelago)
            {
                SendAPLocationCheck(LevelName);
                if(LevelRestarts == 0)
                {
                    string location = $"{LevelName} (No Resets)";
                    SendAPLocationCheck(location);
                }
            }
        }


        public static bool OnNextLevel()
        {
            var autosave = SaveData.SaveData.GetSaveSlot(SaveData.SaveData.AutosaveSlotName);
            LevelRestarts = 0;
            CurrentLevelNumber++;
            autosave.CurrentLevelNumber = CurrentLevelNumber;
            SaveData.SaveData.WriteSaveSlot(autosave);
            if(CurrentLevelNumber > autosave.LevelCount)
            {
                // All levels have been beaten
                return true;
            }

            LoadCurrentLevel();
            return false;
        }


        public static void OnChipCollected(ChipItem chip)
        {
            ChipsCollected++;
            if(GameMode is GameMode.Archipelago)
            {
                string locationName = chip.GetArchipelagoLocationName();
                SendAPLocationCheck(locationName);
            }
            RemoveMapItem(chip.Coordinate);
        }


        public static void OnPlayerDeath()
        {
            ProcessMobs = false;
            AcceptingPlayerInput = false;
            TimerEnabled = false;
            PlayerAlive = false;
            SendDeathlink();
        }


        public static void StealEquipment()
        {
            PlayerItems = PlayerItems.Where(e =>
                e is not IceSkatesItem
                || e is not FlippersItem
                || e is not FireBootsItem
                || e is not SuctionBootsItem
            ).ToList();
        }


        public static void StealKeys()
        {
            PlayerItems = PlayerItems.Where(e =>
                e is not RedKeyItem
                || e is not BlueKeyItem
                || e is not YellowKeyItem
                || e is not GreenKeyItem
            ).ToList();
        }


        public static bool PlayerHasGameItem(int dataCode)
        {
            return PlayerItems.Any(e => e.DataCode == dataCode);
        }


        public static bool ConsumeItem(int dataCode)
        {
            bool result = false;
            List<MapItem> newPlayerItems = [];
            foreach(var playerItem in PlayerItems)
            {
                if(result || playerItem.DataCode != dataCode)
                {
                    newPlayerItems.Add(playerItem);
                    continue;
                }

                if(playerItem.DataCode == dataCode)
                {
                    result = true;
                    continue;
                }

                newPlayerItems.Add(playerItem);
            }
            PlayerItems = newPlayerItems;
            return result;
        }
    }
}
