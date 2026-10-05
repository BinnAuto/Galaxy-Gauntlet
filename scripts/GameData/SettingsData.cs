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
            }
            SaveSettings();
        }


        public static void SaveSettings()
        {
            ConfigFile.SetValue(GameDataConfigSection, nameof(TickRate), TickRate);
            ConfigFile.Save(GameConfigFilePath);
        }
    }
}
