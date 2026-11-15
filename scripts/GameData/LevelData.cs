using System.Collections.Generic;
using System.Linq;

namespace GalaxyGauntlet.scripts
{
	public static partial class GameData
	{
		public static LevelData Level = new();
    }

	/// <summary>
	/// Data pertaining to the current level and gameplay
	/// </summary>
	public class LevelData
	{
		public int MapViewportSize = 9;
        public int StandardMapViewportSize = 9;
        public int ChipsRequired = 0;
		public int ChipsCollected = 0;
		public int CurrentLevelIndex = 1;
		public int CurrentTick = 0;
		public int ChipTick = 0;
		public string Name = string.Empty;
		public string Hint = string.Empty;
		public int RestartCount = 0;
		public double Timer = 0;
		public bool TimerEnabled = false;
		public bool AcceptingPlayerInput = true;
		public bool ProcessMobs = false;
		public bool PlayerAlive = true;
		public string DeathMessage = string.Empty;
		public int SteppingOffset = 1; // Odd or Even Step
		public Vector2I PlayerCoordinate = Vector2I.Zero;
		public string[] LevelSet = [];

		// Controlled by Archipelago items
		public double ConfusionTimer = 0;
		public bool IsConfused
		{
			get
			{
				return (ConfusionTimer > 0);
			}
		}

		public bool ChipRequirementMet
		{
			get
			{
				return ChipsCollected >= ChipsRequired;
			}
		}


		public bool XRayMode
		{
			get
			{
				return PlayerHasGameItem(GGConstants.ByteCodes.Entities.SecretEye);
			}
		}


		public int TimerDisplay
		{
			get
			{
				return (int)Timer;
			}
        }

        public delegate void OnLoadCurrentLevelHandler();

        public event OnLoadCurrentLevelHandler OnLoadCurrentLevel = () => { };

		private List<MapItem2> PlayerItems = [];


        public void Reset()
		{
			PlayerAlive = true;
			DeathMessage = string.Empty;
			Name = string.Empty;
			Hint = string.Empty;
			ProcessMobs = false;
			TimerEnabled = false;
			AcceptingPlayerInput = true;
			ConfusionTimer = 0;
			MapViewportSize = StandardMapViewportSize;
			CurrentTick = 0;
			ChipTick = 0;
			PlayerItems = [];

			ChipsRequired = 0;
			ChipsCollected = 0;
			PlayerCoordinate = Vector2I.Zero;
		}


		public bool OnNextLevel()
		{
			var autosave = SaveData.SaveData.GetSaveSlot(GGConstants.SaveSlotNames.Autosave);
			RestartCount = 0;
			CurrentLevelIndex++;
			autosave.CurrentLevelNumber = CurrentLevelIndex;
			SaveData.SaveData.WriteSaveSlot(autosave);
			if(CurrentLevelIndex > autosave.LevelCount)
			{
				// All levels have been beaten
				return true;
			}

			OnLoadCurrentLevel.Invoke();
			return false;
		}


		public void UpdatePlayerItems(List<MapItem2> playerItems)
		{
			PlayerItems = playerItems;
		}


		public bool PlayerHasGameItem(int entityID)
		{
			return PlayerItems.Any(e => e.EntityID == entityID);
		}


		public int ItemCount(int entityID)
		{
			return PlayerItems.Count(e => e.EntityID == entityID);
		}
	}
}
