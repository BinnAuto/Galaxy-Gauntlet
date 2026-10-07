using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.Models;
using GalaxyGauntlet.scripts.MapEntities;
using GalaxyGauntlet.scripts.MapEntities.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using DeathLinkData = Archipelago.MultiClient.Net.BounceFeatures.DeathLink.DeathLink;

namespace GalaxyGauntlet.scripts
{
    public static partial class GameData
    {
        // Item unlock flags
        public static bool RedKeyUnlocked = true;
        public static bool BlueKeyUnlocked = true;
        public static bool YellowKeyUnlocked = true;
        public static bool GreenKeyUnlocked = true;
        public static bool IceSkatesUnlocked = true;
        public static bool FireBootsUnlocked = true;
        public static bool SuctionBootsUnlocked = true;
        public static bool FlippersUnlocked = true;

        // Archipelago connection/game data
        private static ArchipelagoSession Session = null;
        private static DeathLinkService DeathLinkService = null;
        public static bool IsConnected { get; private set; } = false;
        public static bool DeathLinkEnabled = false;
        public static bool LynxBehavior = false;
        public static bool ProcessingDeathLink = false;
        public static long PlayerSpriteIndex = Constants.Archipelago.PlayerSpriteIndexes.Player;
        private static List<long> LocationIDsChecked = [];
        public static List<string> ItemsReceived = [];
        public static List<MapItem> PendingItems = [];
        public static string GameName;
        private static string Server;
        private static int? Port;
        private static string SlotName;
        private static string Password;
        private static int? SlotId;
        private static bool InitialProcessingComplete = false;
        private static Dictionary<string, object> SlotData = [];
        private static Dictionary<long, ScoutedItemInfo> ScoutedItems = [];

        public static List<string> LocationsChecked
        {
            get
            {
                return LocationIDsChecked.Select(e => Session.Locations.GetLocationNameFromId(e)).ToList();
            }
        }

        public static async Task ConnectToArchipelago(string gameName, string server, int port, string slotName, string password)
        {
            try
            {
                ResetArchipelagoData();
                GameName = gameName;

                // Build Session
                Session = ArchipelagoSessionFactory.CreateSession(server, port);
                Session.Socket.SocketOpened += OnAPSocketOpened;
                Session.Socket.SocketClosed += OnAPSocketClosed;
                Session.Socket.ErrorReceived += OnAPSocketError;
                Session.Locations.CheckedLocationsUpdated += OnAPLocationChecked;
                Session.Items.ItemReceived += OnAPItemReceived;

                var loginResult = Session.TryConnectAndLogin(GameName, slotName, ItemsHandlingFlags.AllItems, password: password, tags: ["AP", "Deathlink"]);
                if(false == loginResult.Successful)
                {
                    var loginFailure = (LoginFailure)loginResult;
                    if(loginFailure.Errors.Length > 0)
                    {
                        throw new(loginFailure.Errors[0]);
                    }
                }

                Server = server;
                Port = port;
                SlotName = slotName;
                Password = password;
                SlotId = Session.Players.ActivePlayer.Slot;

                SlotData = ((LoginSuccessful)loginResult).SlotData;

                // Get Player Sprite
                try
                {
                    PlayerSpriteIndex = (long)SlotData[Constants.Archipelago.SlotDataKeys.PlayerSprite];
                }
                catch(Exception e) {
                    GD.Print(e.Message);
                    PlayerSpriteIndex = Constants.Archipelago.PlayerSpriteIndexes.Player; 
                }

                // Configure death link
                DeathLinkService = Session.CreateDeathLinkService();
                DeathLinkService.OnDeathLinkReceived += OnDeathLinkReceived;
                long deathLinkSetting = GetAPSlotDataKey<long>(Constants.Archipelago.SlotDataKeys.DeathLink);
                DeathLinkEnabled = (deathLinkSetting == 1);
                if(DeathLinkEnabled)
                {
                    DeathLinkService.EnableDeathLink();
                }
                else
                {
                    DeathLinkService.DisableDeathLink();
                }

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


        public static async Task DisconnectFromArchipelago()
        {
            try
            {
                await Session.Socket.DisconnectAsync();
                ResetArchipelagoData();
            }
            catch(Exception e)
            {
                FileLogger.LogException("Error disconnecting from Archipelago server", e);
            }
        }


        public static void SendDeathlink()
        {
            try
            {
                if(false == DeathLinkEnabled)
                {
                    ProcessingDeathLink = false;
                    return;
                }

                if(false == ProcessingDeathLink)
                {
                    FileLogger.QuietLogMessage("Sending deathlink message");
                    DeathLinkData deathLinkData = new(SlotName);
                    DeathLinkService.SendDeathLink(deathLinkData);
                }
            }
            catch(Exception e)
            {
                FileLogger.LogException("Error sending deathlink message", e);
            }
        }


        public static async Task ScoutAPLocations()
        {
            long[] locationIDs = [.. Session.Locations.AllLocations];
            ScoutedItems = await Session.Locations.ScoutLocationsAsync(locationIDs);
        }


        public static ItemFlags? GetAPItemTypeAtLocation(long locationId)
        {
            if(false == ScoutedItems.ContainsKey(locationId))
            {
                return null;
            }

            return ScoutedItems[locationId].Flags;
        }


        public static T GetAPSlotDataKey<T>(string key)
        {
            if(false == SlotData.ContainsKey(key))
            {
                return default;
            }

            return (T)SlotData[key];
        }


        public static ScoutedItemInfo GetAPItemAtLocation(long locationId)
        {
            if(false == ScoutedItems.ContainsKey(locationId))
            {
                return null;
            }

            return ScoutedItems[locationId];
        }


        public static void SendAPLocationCheck(string locationName)
        {
            long locationId = GetAPLocationId(locationName);
            if(locationId == -1)
            {
                return;
            }

            Session.Locations.CompleteLocationChecks(locationId);
        }


        public static bool IsAPLocationChecked(string locationName)
        {
            long locationId = GetAPLocationId(locationName);
            return IsAPLocationChecked(locationId);
        }


        public static bool IsAPLocationChecked(long locationId)
        {
            return LocationIDsChecked.Contains(locationId);
        }


        public static bool PlayerHasAPItem(string itemName)
        {
            return ItemsReceived.Contains(itemName);
        }


        public static bool DoesAPLocationExist(string locationName)
        {
            return GetAPLocationId(locationName) > -1;
        }


        public static long GetAPLocationId(string locationName)
        {
            long id = Session.Locations.GetLocationIdFromName(GameName, locationName);
            return id;
        }


        private static void ResetArchipelagoData()
        {
            Session = null;
            DeathLinkService = null;
            LocationIDsChecked = [];
            PlayerSpriteIndex = Constants.Archipelago.PlayerSpriteIndexes.Player;
            GameName = string.Empty;
            Server = string.Empty;
            Port = null;
            SlotName = string.Empty;
            SlotId = null;
            Password = string.Empty;
            SlotData = [];
            PendingItems = [];
        }


        private static void ProcessAPItemUnlocks()
        {
            RedKeyUnlocked = PlayerHasAPItem("Red Key");
            BlueKeyUnlocked = PlayerHasAPItem("Blue Key");
            YellowKeyUnlocked = PlayerHasAPItem("Yellow Key");
            GreenKeyUnlocked = PlayerHasAPItem("Green Key");
            IceSkatesUnlocked = PlayerHasAPItem("Ice Skates");
            FireBootsUnlocked = PlayerHasAPItem("Fire Boots");
            SuctionBootsUnlocked = PlayerHasAPItem("Suction Boots");
            FlippersUnlocked = PlayerHasAPItem("Flippers");
            RerenderMap = true;
        }


        private static void ProcessNewItem(string itemName)
        {
            switch(itemName)
            {
                case Constants.Archipelago.ItemNames.SingleUseRedKey:
                    PendingItems.Add(new RedKeyItem(Vector2I.Zero));
                    break;

                case Constants.Archipelago.ItemNames.SingleUseBlueKey:
                    PendingItems.Add(new BlueKeyItem(Vector2I.Zero));
                    break;

                case Constants.Archipelago.ItemNames.SingleUseYellowKey:
                    PendingItems.Add(new YellowKeyItem(Vector2I.Zero));
                    break;

                case Constants.Archipelago.ItemNames.SingleUseGreenKey:
                    PendingItems.Add(new GreenKeyItem(Vector2I.Zero));
                    break;

                case Constants.Archipelago.ItemNames.SingleUseIceSkates:
                    PendingItems.Add(new IceSkatesItem(Vector2I.Zero));
                    break;

                case Constants.Archipelago.ItemNames.SingleUseFireBoots:
                    PendingItems.Add(new FireBootsItem(Vector2I.Zero));
                    break;

                case Constants.Archipelago.ItemNames.SingleUseSuctionBoots:
                    PendingItems.Add(new SuctionBootsItem(Vector2I.Zero));
                    break;

                case Constants.Archipelago.ItemNames.SingleUseFlippers:
                    PendingItems.Add(new FlippersItem(Vector2I.Zero));
                    break;

                case Constants.Archipelago.ItemNames.ConfusionTrap:
                    RandomNumberGenerator rng = new();
                    ConfusionTimer = rng.RandiRange(20, 40);
                    break;

                case Constants.Archipelago.ItemNames.TimeBonus:
                    if(TimeLimit == 0)
                    {
                        // Turn untimed levels into timed levels
                        TimeLimit += 10;
                    }
                    Timer += 10;
                    break;

                case Constants.Archipelago.ItemNames.TimePenalty:
                    Timer -= 10;
                    break;

                case Constants.Archipelago.ItemNames.TeethTrap:
                    AddMobAtRandom(new TeethMonster(Vector2I.Zero));
                    break;

                case Constants.Archipelago.ItemNames.WalkerTrap:
                    AddMobAtRandom(new WalkerMonster(Vector2I.Zero));
                    break;

                case Constants.Archipelago.ItemNames.Helmet:
                case Constants.Archipelago.ItemNames.SecretEye:
                    // TODO
                    break;
            }
        }


        #region Event Handlers

        private static void OnAPSocketError(Exception e, string message)
        {
            FileLogger.QuietLogMessage($"Socket error: {e.Message}");
        }


        private static void OnAPSocketClosed(string reason)
        {
            FileLogger.QuietLogMessage("Socket closed");
            IsConnected = false;
        }


        private static void OnAPSocketOpened()
        {
            FileLogger.QuietLogMessage("Socket opened");
            IsConnected = true;
        }


        private static void OnAPItemReceived(ReceivedItemsHelper helper)
        {
            if(InitialProcessingComplete)
            {
                var itemInfo = helper.DequeueItem();
                while(itemInfo is not null)
                {
                    GD.Print($"Item received: {itemInfo.ItemName}");
                    ProcessNewItem(itemInfo.ItemName);
                    itemInfo = helper.DequeueItem();
                }
            }
            else
            {
                GD.Print("Dumping items");
                var itemInfo = helper.DequeueItem();
                while(itemInfo is not null)
                {
                    itemInfo = helper.DequeueItem();
                }
            }
            ItemsReceived = helper.AllItemsReceived
                .Select(e => Session.Items.GetItemName(e.ItemId))
                .ToList();
            ProcessAPItemUnlocks();
        }


        private static void OnAPLocationChecked(ReadOnlyCollection<long> newLocations)
        {
            LocationIDsChecked.AddRange(newLocations);
            LocationIDsChecked = LocationIDsChecked.Distinct().ToList();
        }


        private static void OnDeathLinkReceived(DeathLinkData deathLink)
        {
            FileLogger.QuietLogMessage("Deathlinkn packet received");
            if(false == DeathLinkEnabled)
            {
                FileLogger.QuietLogMessage("Deathlink is disabled");
                return;
            }

            ProcessingDeathLink = true;
            DeathMessage = $"Ooops! {deathLink.Source} has died!";
        }

        #endregion
    }
}
