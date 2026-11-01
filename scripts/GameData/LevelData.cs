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

	public static partial class GameData
	{
		public static LevelData Level = new();
    }

	/// <summary>
	/// Data pertaining to the current level and gameplay
	/// </summary>
	public class LevelData
	{
		public GameMode GameMode = GameMode.Campaign;
		public int MapViewportSize = 9;
        public int StandardMapViewportSize = 9;
        public int ChipsRequired = 0;
		public int ChipsCollected = 0;
		public int CurrentLevelIndex = 1;
		public string Name = string.Empty;
		public string Hint = string.Empty;
		public int RestartCount = 0;
		public double Timer = 0;
		public bool TimerEnabled = false;
		public bool AcceptingPlayerInput = true;
		public bool ProcessMobs = false;
		public bool PlayerAlive = true;
		public string DeathMessage = string.Empty;
		public Vector2I PlayerCoordinate = Vector2I.Zero;
		public string[] LevelSet = [];
		public List<MapItem> PlayerItems = [];

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
				return PlayerHasGameItem(Constants.ByteCodes.Entities.SecretEye);
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

			ChipsRequired = 0;
			ChipsCollected = 0;
			PlayerItems = [];
			PlayerCoordinate = Vector2I.Zero;
		}


		public bool OnNextLevel()
		{
			var autosave = SaveData.SaveData.GetSaveSlot(Constants.SaveSlotNames.Autosave);
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


		public void StealEquipment()
		{
			int[] bootCodes = [
				Constants.ByteCodes.Entities.IceSkates
				, Constants.ByteCodes.Entities.Flippers
				, Constants.ByteCodes.Entities.FireBoots
				, Constants.ByteCodes.Entities.SuctionBoots
			];
			PlayerItems = PlayerItems.Where(e =>
				false == bootCodes.Contains(e.DataCode)
			).ToList();
		}


		public void StealKeys()
		{
			int[] keyCodes = [
				Constants.ByteCodes.Entities.RedKey
				, Constants.ByteCodes.Entities.BlueKey
				, Constants.ByteCodes.Entities.YellowKey
				, Constants.ByteCodes.Entities.GreenKey
			];
			PlayerItems = PlayerItems.Where(e =>
				false == keyCodes.Contains(e.DataCode)
			).ToList();
		}


		public void AddItem(MapItem item)
		{
			PlayerItems.Add(item);
		}


		public bool PlayerHasGameItem(int dataCode)
		{
			return PlayerItems.Any(e => e.DataCode == dataCode);
		}


		public int ItemCount(int dataCode)
		{
			return PlayerItems.Count(e => e.DataCode == dataCode);
		}


		public bool ConsumeItem(int dataCode)
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
