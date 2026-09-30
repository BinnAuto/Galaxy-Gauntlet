using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.Models;
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
        public static bool ProcessingDeathLink = false;
        private static List<long> LocationIDsChecked = [];
        public static List<string> ItemsReceived = [];
        public static string GameName;
        private static string Server;
        private static int? Port;
        private static string SlotName;
        private static string Password;
        private static int? SlotId;
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
                Session.Items.ItemReceived += OnAPItemReceived;
                Session.Locations.CheckedLocationsUpdated += OnAPLocationChecked;

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
                GD.Print("Connected!");
            }
            catch (Exception)
            {
                Session = null;
                throw;
            }
        }


        public static async Task DisconnectFromArchipelago()
        {
            await Session.Socket.DisconnectAsync();
            ResetArchipelagoData();
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
                    GD.Print("Sending death link");
                    DeathLinkData deathLinkData = new(SlotName);
                    DeathLinkService.SendDeathLink(deathLinkData);
                }
            }
            catch(Exception e)
            {
                GD.Print(e.Message);
                GD.Print(e.StackTrace);
            }
        }


        public static async Task ScoutAPLocations()
        {
            GD.Print("Scouting locations...");
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
            GameName = string.Empty;
            Server = string.Empty;
            Port = null;
            SlotName = string.Empty;
            SlotId = null;
            Password = string.Empty;
            SlotData = [];
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


        #region Event Handlers

        private static void OnAPSocketError(Exception e, string message)
        {
            GD.Print($"Socket error: {e.Message}");
        }


        private static void OnAPSocketClosed(string reason)
        {
            GD.Print("Socket closed");
            IsConnected = false;
        }


        private static void OnAPSocketOpened()
        {
            GD.Print("Socket opened");
            IsConnected = true;
        }


        private static void OnAPItemReceived(ReceivedItemsHelper helper)
        {
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
            GD.Print("Deathlink packet received");
            if(DeathLinkEnabled)
            {
                ProcessingDeathLink = true;
                DeathMessage = $"Ooops! {deathLink.Source} has died!";
            }
            else
            {
                GD.Print("Deathlink disabled");
            }
        }

        #endregion
    }
}
