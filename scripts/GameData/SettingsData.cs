namespace GalaxyGauntlet.scripts
{
    public static partial class GameData
    {
        public static double TimeModifier = 100;

        public static double TimeModifierNumeric
        {
            get
            {
                return TimeModifier / 100.0;
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
                TimeModifier = (double)ConfigFile.GetValue(GameDataConfigSection, nameof(TimeModifier), 1);
            }
            GD.Print($"Loaded time modifier: {TimeModifier}");
            SaveSettings();
        }


        public static void SaveSettings()
        {
            ConfigFile.SetValue(GameDataConfigSection, nameof(TimeModifier), TimeModifier);
            GD.Print($"Saved time modifier: {TimeModifier}");
            ConfigFile.Save(GameConfigFilePath);
        }
    }
}
