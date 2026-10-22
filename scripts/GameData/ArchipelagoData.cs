using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.Models;
using GalaxyGauntlet.scripts.MapEntities;
using GalaxyGauntlet.scripts.MapEntities.Shared;
using GalaxyGauntlet.scripts.SaveData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using DeathLinkData = Archipelago.MultiClient.Net.BounceFeatures.DeathLink.DeathLink;

namespace GalaxyGauntlet.scripts
{
	public static partial class GameData
	{
		public static ArchipelagoData Archipelago = new();
	}

	public class ArchipelagoData
	{
		/*
		 * Indicates if a given game item unlock has been received. For non-Archipelago 
		 * gamemodes, these are always set to true
		 */
		#region Item Unlock Flags

		public bool RedKeyUnlocked = true;
		public bool BlueKeyUnlocked = true;
		public bool YellowKeyUnlocked = true;
		public bool GreenKeyUnlocked = true;
		public bool IceSkatesUnlocked = true;
		public bool FireBootsUnlocked = true;
		public bool SuctionBootsUnlocked = true;
		public bool FlippersUnlocked = true;
		public bool HelmetUnlocked = true;
		public bool SecretEyeUnlocked = true;

		#endregion


		/*
		 * Contains information about the current connection to the Archipelago server.
		 */
		#region Archipelago Connection

		private ArchipelagoSession Session = null;

		public string GameName { get; set; } = string.Empty;

		public string Server { get; set; } = string.Empty;

		public int? Port { get; set; } = null;

		public string SlotName { get; set; } = string.Empty;

		public string Password { get; set; } = string.Empty;

		public int? SlotId { get; set; } = null;

		#endregion


		/*
		 * Contains gameplay-relevant Archipelago data such as settings, items, and 
		 * locations.
		 */
		#region Archipelago Data

		/// <summary>
		/// Is deathlink enabled for the current Archipelago session
		/// </summary>
		public bool DeathLinkEnabled = false;

		/// <summary>
		/// Is the game currently processing an incoming deathlink command? Prevents deathlink loops.
		/// </summary>
		public bool IsProcessingDeathLink = false;

		public long PlayerSpriteIndex = Constants.Archipelago.PlayerSpriteIndexes.Player;

		/// <summary>
		/// A list of level file names to ignore when displaying the level select list.
		/// This list is defined in the APWorld file and stored in game's slot data.
		/// </summary>
		public string[] LevelFilesToIgnore = [];

		/// <summary>
		/// The list of single-use items the player has received. The player receives
		/// these items when they are in a level.
		/// </summary>
		public List<MapItem> PendingItems = [];

		private bool InitialProcessingComplete = false;
		private DeathLinkService DeathLinkService = null;
		private List<string> ItemsReceived = [];
		private List<long> LocationsChecked = [];
		private Dictionary<string, object> SlotData = [];
		private Dictionary<long, ScoutedItemInfo> ScoutedItems = [];

		/// <summary>
		/// Returns the total score required to meet the goal condition. A goal of 0 means this
		/// goal is disabled.
		/// </summary>
		public long ScoreGoal
		{
			get
			{
				return GetSlotDataKey<long>(Constants.Archipelago.SlotDataKeys.ScoreGoal);
			}
		}


		/// <summary>
		/// Returns the number of completed levels required to meet the goal condition.
		/// A goal of 0 means this goal is disabled.
		/// </summary>
		public long LevelCompletionGoal
		{
			get
			{
				return GetSlotDataKey<long>(Constants.Archipelago.SlotDataKeys.LevelCompletionGoal);
			}
		}


		/// <summary>
		/// Returns the number of levels currently completed by the player.
		/// </summary>
		public int CurrentLevelCompletedCount
		{
			get
			{
				var archipelagoData = SaveData.SaveData.GetSaveSlot(Constants.SaveSlotNames.Archipelago);
				return archipelagoData?.HighScores?.Count ?? 0;
			}
		}


		/// <summary>
		/// Returns the player's current total score.
		/// </summary>
		public long CurrentTotalScore
		{
			get
			{
				if (CurrentLevelCompletedCount == 0)
				{
					return 0;
				}


				var archipelagoData = SaveData.SaveData.GetSaveSlot(Constants.SaveSlotNames.Archipelago);
				return archipelagoData.HighScores.Sum(e => e.Value);
			}
		}


		#endregion


		/// <summary>
		/// Resets the object's data state.
		/// </summary>
		public void Reset()
		{
			Session = null;
			GameName = string.Empty;
			Server = string.Empty;
			Port = null;
			SlotName = string.Empty;
			Password = string.Empty;

			DeathLinkService = null;
			LocationsChecked = [];
			LevelFilesToIgnore = [];
			PlayerSpriteIndex = Constants.Archipelago.PlayerSpriteIndexes.Player;

			SlotId = null;
			SlotData = [];
			PendingItems = [];
			SaveData.SaveData.DeleteSaveSlot(Constants.SaveSlotNames.Archipelago);
			SaveData.SaveData.WriteSaveSlot(new(Constants.SaveSlotNames.Archipelago));
		}


		#region Server Connect Methods

		/// <summary>
		/// Attempts to connect to the Archipelago server using the given parameters
		/// </summary>
		public async Task ConnectToServer(string gameName, string server, int port, string slotName, string password)
		{
			try
			{
				Reset();
				BuildSession(server, port);
				ConnectToServer(gameName, slotName, password);

				GameName = gameName;
				Server = server;
				Port = port;
				SlotName = slotName;
				Password = password;
				// TODO: Save successful Archipelago connection settings to Game Settings

				// Load from Slot Data
				await LoadGameSaveFromSlotData();
				BuildFileIgnoreList();
				GetPlayerSprite();
				ConfigureDeathLink();
				await ScoutAPLocations();
				FileLogger.QuietLogMessage("Successfully connected to Archipelago server");
				InitialProcessingComplete = true;
			}
			catch (Exception e)
			{
				FileLogger.LogException("Error attempting to connect to Archipelago server", e);
				Session = null;
				throw;
			}
		}


		/// <summary>
		/// Disconnects from the Archipelago server.
		/// </summary>
		public async Task DisconnectFromServer()
		{
			try
			{
				await Session.Socket.DisconnectAsync();
				Reset();
			}
			catch (Exception e)
			{
				FileLogger.LogException("Error disconnecting from Archipelago server", e);
			}
		}


		#region Socket Events

		private void OnSocketOpened()
		{
			FileLogger.QuietLogMessage("Socket opened");
		}


		private void OnSocketClosed(string reason)
		{
			FileLogger.QuietLogMessage($"Socket closed with reason {reason}");
		}


		private void OnSocketError(Exception e, string message)
		{
			FileLogger.LogException($"Socket error occurred: {message}", e);
		}

		#endregion


		#region Connection Helper Methods

		private void BuildSession(string server, int port)
		{
			Session = ArchipelagoSessionFactory.CreateSession(server, port);
			Session.Socket.SocketOpened += OnSocketOpened;
			Session.Socket.SocketClosed += OnSocketClosed;
			Session.Socket.ErrorReceived += OnSocketError;
			Session.Locations.CheckedLocationsUpdated += OnLocationChecked;
			Session.Items.ItemReceived += OnItemAddedToQueue;
		}


		private void ConnectToServer(string gameName, string slotName, string password)
		{
			var loginResult = Session.TryConnectAndLogin(gameName, slotName, ItemsHandlingFlags.AllItems, password: password, tags: ["AP"]);
			if (false == loginResult.Successful)
			{
				var loginFailure = (LoginFailure)loginResult;
				string exceptionMessage = $"The following errors occurred while attempting to connect:";
				if (loginFailure.Errors.Length > 0)
				{
					exceptionMessage += $"\r\n{loginFailure.Errors.Join("\r\n")}";
					FileLogger.LogException("Error connecting to Archipelago server", new(exceptionMessage));

					throw new(loginFailure.Errors[0]);
				}

				throw new("Unexpected error occurred while connecting to Archipelago server.");
			}

			SlotData = ((LoginSuccessful)loginResult).SlotData;
		}


		private async Task LoadGameSaveFromSlotData()
		{
			SaveSlot apSaveSlot = new(Constants.SaveSlotNames.Archipelago);
			var saveData = Session.DataStorage[Constants.Archipelago.SlotDataKeys.SaveData];
			string saveDataJson = $"{await saveData.GetAsync()}";
			if (false == string.IsNullOrEmpty(saveDataJson))
			{
				apSaveSlot = JsonSerializer.Deserialize<SaveSlot>(saveDataJson);
			}
			SaveData.SaveData.WriteSaveSlot(apSaveSlot);
		}


		private void BuildFileIgnoreList()
		{
			LevelFilesToIgnore = [];
			string ignoreFiles = SlotData[Constants.Archipelago.SlotDataKeys.IgnoreLevelFiles].ToString();
			if (false == string.IsNullOrEmpty(ignoreFiles))
			{
				LevelFilesToIgnore = ignoreFiles.Split('|');
			}
		}


		private void GetPlayerSprite()
		{
			try
			{
				PlayerSpriteIndex = (long)SlotData[Constants.Archipelago.SlotDataKeys.PlayerSprite];
			}
			catch (Exception e)
			{
				GD.Print(e.Message);
				PlayerSpriteIndex = Constants.Archipelago.PlayerSpriteIndexes.Player;
			}
		}


		private void ConfigureDeathLink()
		{
			DeathLinkService = Session.CreateDeathLinkService();
			DeathLinkService.OnDeathLinkReceived += OnDeathLinkReceived;
			long deathLinkSetting = GetSlotDataKey<long>(Constants.Archipelago.SlotDataKeys.DeathLink);
			DeathLinkEnabled = (deathLinkSetting == 1);
			if (DeathLinkEnabled)
			{
				DeathLinkService.EnableDeathLink();
			}
			else
			{
				DeathLinkService.DisableDeathLink();
			}
		}


		private async Task ScoutAPLocations()
		{
			long[] locationIDs = [.. Session.Locations.AllLocations];
			ScoutedItems = await Session.Locations.ScoutLocationsAsync(locationIDs);
		}

		#endregion

		#endregion


		/// <summary>
		/// Sets a value on the Archipelago server for a given DataStorage key.
		/// </summary>
		public void SetDataStorageKey(string key, string value)
		{
			Session.DataStorage[key] = value;
		}


		/// <summary>
		/// Returns a slot data value by its corresponding key name.
		/// </summary>
		public T GetSlotDataKey<T>(string key)
		{
			return (T)SlotData.GetValueOrDefault(key);
		}


		/// <summary>
		/// Checks if the player has reached their specified goal, and releases
		/// the game if appropriate.
		/// </summary>
		public void CheckForGoal()
		{
			if(ScoreGoal == 0 && LevelCompletionGoal == 0)
			{
				// No automatic goal.
				// TODO: Implement "complete all levels" fallback goal
				return;
			}

			if(ScoreGoal > 0 && CurrentTotalScore < ScoreGoal)
			{
				return;
			}

			if(LevelCompletionGoal > 0 && CurrentLevelCompletedCount < LevelCompletionGoal)
			{
				return;
			}

			ReleaseGame();
		}


		/// <summary>
		/// Marks the game as released, completing it and sending out all unchecked
		/// locations.
		/// </summary>
		public void ReleaseGame()
		{
			Session.SetGoalAchieved();
		}


		#region Archipelago Location Methods

		public bool HasLocationBeenChecked(string locationName)
		{
			long locationId = GetLocationIdByName(locationName);
			return IsLocationChecked(locationId);
		}


		public bool IsLocationChecked(long locationId)
		{
			return LocationsChecked.Contains(locationId);
		}


		public bool DoesLocationExist(string locationName)
		{
			return GetLocationIdByName(locationName) > -1;
		}


		public long GetLocationIdByName(string locationName)
		{
			return Session.Locations.GetLocationIdFromName(GameName, locationName);
		}


		public void SendLocationCheck(string locationName)
		{
			long locationId = GetLocationIdByName(locationName);
			if (locationId == -1)
			{
				FileLogger.QuietLogMessage($"Location \"{locationName}\" not recognized");
				return;
			}

			Session.Locations.CompleteLocationChecks(locationId);
		}

		private void OnLocationChecked(ReadOnlyCollection<long> newLocations)
		{
			LocationsChecked.AddRange(newLocations);
			LocationsChecked = LocationsChecked.Distinct().ToList();
		}

		#endregion


		#region Archipelago Item Methods

		public bool PlayerReceivedItem(string itemName)
		{
			return ItemsReceived.Contains(itemName);
		}


		public ScoutedItemInfo GetItemAtLocationId(long locationId)
		{
			return ScoutedItems.GetValueOrDefault(locationId);
		}


		public ItemFlags? GetItemTypeAtLocation(long locationId)
		{
			return ScoutedItems.GetValueOrDefault(locationId)?.Flags;
		}


		public void ProcessItemUnlocks()
		{
			RedKeyUnlocked = PlayerReceivedItem("Red Key");
			BlueKeyUnlocked = PlayerReceivedItem("Blue Key");
			YellowKeyUnlocked = PlayerReceivedItem("Yellow Key");
			GreenKeyUnlocked = PlayerReceivedItem("Green Key");
			IceSkatesUnlocked = PlayerReceivedItem("Ice Skates");
			FireBootsUnlocked = PlayerReceivedItem("Fire Boots");
			SuctionBootsUnlocked = PlayerReceivedItem("Suction Boots");
			FlippersUnlocked = PlayerReceivedItem("Flippers");
			HelmetUnlocked = PlayerReceivedItem("Helmet");
			SecretEyeUnlocked = PlayerReceivedItem("Secret Eye");
		}


		private void ProcessNewItem(string itemName)
		{
			MapItem pendingItem = itemName switch
			{
				Constants.Archipelago.ItemNames.SingleUseRedKey => new RedKeyItem(Vector2I.Zero),
				Constants.Archipelago.ItemNames.SingleUseBlueKey => new BlueKeyItem(Vector2I.Zero),
				Constants.Archipelago.ItemNames.SingleUseYellowKey => new YellowKeyItem(Vector2I.Zero),
				Constants.Archipelago.ItemNames.SingleUseGreenKey => new GreenKeyItem(Vector2I.Zero),
				Constants.Archipelago.ItemNames.SingleUseIceSkates => new IceSkatesItem(Vector2I.Zero),
				Constants.Archipelago.ItemNames.SingleUseFireBoots => new FireBootsItem(Vector2I.Zero),
				Constants.Archipelago.ItemNames.SingleUseSuctionBoots => new SuctionBootsItem(Vector2I.Zero),
				Constants.Archipelago.ItemNames.SingleUseFlippers => new FlippersItem(Vector2I.Zero),
				Constants.Archipelago.ItemNames.SingleUseHelmet => new HelmetItem(Vector2I.Zero),
				Constants.Archipelago.ItemNames.SingleUseSecretEye => new SecretEyeItem(Vector2I.Zero),
				_ => null
			};
			if(pendingItem is not null)
			{
				PendingItems.Add(pendingItem);
				return;
			}

			OnItemReceived.Invoke(itemName);
		}


		private void OnItemAddedToQueue(ReceivedItemsHelper helper)
		{
			var itemInfo = helper.DequeueItem();
			while(itemInfo is not null) {
				if(InitialProcessingComplete)
				{
					ProcessNewItem(itemInfo.ItemName);
				}
				itemInfo = helper.DequeueItem();
			}
			ItemsReceived = helper.AllItemsReceived
				.Select(e => Session.Items.GetItemName(e.ItemId))
				.ToList();
			ProcessItemUnlocks();
		}

		#endregion


		#region DeathLink Methods

		/// <summary>
		/// Sends a death link message to the Archipelago server.
		/// </summary>
		public void SendDeathLinkMessage()
		{
			try
			{
				if (false == DeathLinkEnabled)
				{
					IsProcessingDeathLink = false;
					return;
				}

				// Do not send a deathlink message if player is dying from a previous deathlink.
				if (IsProcessingDeathLink)
				{
					return;
				}

				FileLogger.QuietLogMessage("Sending deathlink message to server");
				DeathLinkData deathLinkData = new(SlotName);
				DeathLinkService.SendDeathLink(deathLinkData);
			}
			catch (Exception e)
			{
				FileLogger.LogException("Error sending deathlink message to server", e);
			}
		}


		private void OnDeathLinkReceived(DeathLinkData deathLink)
		{
			FileLogger.QuietLogMessage("Deathlinkn packet received");
			if (false == DeathLinkEnabled)
			{
				FileLogger.QuietLogMessage("Deathlink is disabled");
				return;
			}

			IsProcessingDeathLink = true;
			string deathMessage = $"Ooops! {deathLink.Source} has died!";
			OnDeathMessage.Invoke(deathMessage);
		}


		#endregion


		#region External Event Emitters

		public delegate void OnItemReceivedHandler(string itemName);

		public event OnItemReceivedHandler OnItemReceived = (itemName) => { };

		public delegate void OnDeathMessageHandler(string message);

		public event OnDeathMessageHandler OnDeathMessage = (message) => { };

		#endregion
	}
}
