using GalaxyGauntlet.scripts.MapEntities.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

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


    public static class PlayerState
    {
        public const int Okay = 0;
        public const int Drowned = 1;
        public const int Burned = 2;
        public const int Bombed = 3;
        public const int OutOfTime = 4;
        public const int Collided = 5;
        public const int Squished = 6;
        public const int SquishedDeath = 7;
        public const int NotOkay = 8;
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
		public int CurrentTick = 0;
		public int ChipWaitTick = 0;
		public string Name = string.Empty;
		public string Hint = string.Empty;
		public int RestartCount = 0;
		public double Timer = 0;
		public bool TimerEnabled = false;
		public bool AcceptingPlayerInput = true;
		public bool ProcessMobs = false;
		public bool PlayerAlive = true;
		public int CurrentPlayerState = PlayerState.Okay;
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

			ChipsRequired = 0;
			ChipsCollected = 0;
			PlayerItems = [];
			PlayerCoordinate = Vector2I.Zero;
			CurrentPlayerState = PlayerState.Okay;
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


		public void StealEquipment()
		{
			int[] bootCodes = [
				GGConstants.ByteCodes.Entities.IceSkates
				, GGConstants.ByteCodes.Entities.Flippers
				, GGConstants.ByteCodes.Entities.FireBoots
				, GGConstants.ByteCodes.Entities.SuctionBoots
			];
			PlayerItems = PlayerItems.Where(e =>
				false == bootCodes.Contains(e.DataCode)
			).ToList();
		}


		public void StealKeys()
		{
			int[] keyCodes = [
				GGConstants.ByteCodes.Entities.RedKey
				, GGConstants.ByteCodes.Entities.BlueKey
				, GGConstants.ByteCodes.Entities.YellowKey
				, GGConstants.ByteCodes.Entities.GreenKey
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
        
		public Vector2I GetPlayerInput()
        {
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
            return input;
        }
    }
}
