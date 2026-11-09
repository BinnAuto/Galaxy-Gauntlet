using Godot;

namespace GalaxyGauntlet.scripts
{
    public static partial class GameData
    {
        public static double TickRate = 100;

        public static double TickRateNumeric
        {
            get
            {
                return TickRate / 100.0;
            }
        }

        // Audio Settings
        public static double MasterVolume = 50;

        public static double MusicVolume = 50;

        public static double WindowMagnifier = 1;

        private const string GameConfigFilePath = "user://gamedata.config";

        private const string GameDataConfigSection = "GameData";

        private static ConfigFile ConfigFile = new();

        public static Vector2I? InitialWindowResolution;

        public static void LoadSettings()
        {
            var loadResult = ConfigFile.Load(GameConfigFilePath);
            if(loadResult == Error.Ok)
            {
                TickRate = (double)ConfigFile.GetValue(GameDataConfigSection, Constants.Settings.TickRate, 1);
                Archipelago.GameName = (string)ConfigFile.GetValue(GameDataConfigSection, Constants.Settings.ArchipelagoGameName, "Chip's Challenge");
                Archipelago.Server = (string)ConfigFile.GetValue(GameDataConfigSection, Constants.Settings.ArchipelagoServer, "archipelago.gg");
                Archipelago.Port = (int)ConfigFile.GetValue(GameDataConfigSection, Constants.Settings.ArchipelagoPort, string.Empty);
                Archipelago.SlotName = (string)ConfigFile.GetValue(GameDataConfigSection, Constants.Settings.ArchipelagoSlotName, string.Empty);
                Archipelago.Password = (string)ConfigFile.GetValue(GameDataConfigSection, Constants.Settings.ArchipelagoPassword, string.Empty);
                MasterVolume = (double)ConfigFile.GetValue(GameDataConfigSection, Constants.Settings.MasterVolume, 50);
                MusicVolume = (double)ConfigFile.GetValue(GameDataConfigSection, Constants.Settings.MusicVolume, 50);
                WindowMagnifier = (int)ConfigFile.GetValue(GameDataConfigSection, Constants.Settings.WindowMagnifier, 1);
            }
            SaveSettings();
        }


        public static void SaveSettings()
        {
            ConfigFile.SetValue(GameDataConfigSection, Constants.Settings.TickRate, TickRate);
            ConfigFile.SetValue(GameDataConfigSection, Constants.Settings.ArchipelagoGameName, Archipelago.GameName);
            ConfigFile.SetValue(GameDataConfigSection, Constants.Settings.ArchipelagoServer, Archipelago.Server);
            ConfigFile.SetValue(GameDataConfigSection, Constants.Settings.ArchipelagoPort, Archipelago.Port.ToString());
            ConfigFile.SetValue(GameDataConfigSection, Constants.Settings.ArchipelagoSlotName, Archipelago.SlotName);
            ConfigFile.SetValue(GameDataConfigSection, Constants.Settings.ArchipelagoPassword, Archipelago.Password);
            ConfigFile.SetValue(GameDataConfigSection, Constants.Settings.MasterVolume, MasterVolume);
            ConfigFile.SetValue(GameDataConfigSection, Constants.Settings.MusicVolume, MusicVolume);
            ConfigFile.SetValue(GameDataConfigSection, Constants.Settings.WindowMagnifier, WindowMagnifier);
            ConfigFile.Save(GameConfigFilePath);

            // Apply settings
            GGG.SetBusVolume(Constants.AudioBuses.Master, MasterVolume);
            GGG.SetBusVolume(Constants.AudioBuses.Music, MusicVolume);
        }
    }
}
