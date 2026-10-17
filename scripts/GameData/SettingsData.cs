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

        private const string GameConfigFilePath = "user://gamedata.config";

        private const string GameDataConfigSection = "GameData";

        private static ConfigFile ConfigFile = new();

        public static void LoadSettings()
        {
            var loadResult = ConfigFile.Load(GameConfigFilePath);
            if(loadResult == Error.Ok)
            {
                TickRate = (double)ConfigFile.GetValue(GameDataConfigSection, nameof(TickRate), 1);
                ArchipelagoGameName = (string)ConfigFile.GetValue(GameDataConfigSection, nameof(ArchipelagoGameName), "Chip's Challenge");
                ArchipelagoServer = (string)ConfigFile.GetValue(GameDataConfigSection, nameof(ArchipelagoServer), "archipelago.gg");
                ArchipelagoPort = (int)ConfigFile.GetValue(GameDataConfigSection, nameof(ArchipelagoPort), string.Empty);
                SlotName = (string)ConfigFile.GetValue(GameDataConfigSection, nameof(SlotName), string.Empty);
                ArchipelagoPassword = (string)ConfigFile.GetValue(GameDataConfigSection, nameof(ArchipelagoPassword), string.Empty);
                GD.Print(ArchipelagoPort); GD.Print(SlotName); GD.Print(ArchipelagoPassword);
            }
            SaveSettings();
        }


        public static void SaveSettings()
        {
            ConfigFile.SetValue(GameDataConfigSection, nameof(TickRate), TickRate);
            ConfigFile.SetValue(GameDataConfigSection, nameof(ArchipelagoGameName), ArchipelagoGameName);
            ConfigFile.SetValue(GameDataConfigSection, nameof(ArchipelagoServer), ArchipelagoServer);
            ConfigFile.SetValue(GameDataConfigSection, nameof(ArchipelagoPort), ArchipelagoPort.ToString());
            ConfigFile.SetValue(GameDataConfigSection, nameof(SlotName), SlotName);
            ConfigFile.SetValue(GameDataConfigSection, nameof(ArchipelagoPassword), ArchipelagoPassword);
            ConfigFile.Save(GameConfigFilePath);
        }
    }
}
