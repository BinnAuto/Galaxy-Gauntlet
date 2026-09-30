using GalaxyGauntlet.Common;
using System.Text.Json;

namespace GalaxyGauntlet.APWorldGen.CodeGen
{
    public static partial class APWorldCodeGen
    {
        public static LevelOverrides LevelOverrides { get; set; }

        public static void LoadLevelOverrides()
        {
            LevelOverrides = new()
            {
                Overrides = []
            };
            string overrideFile = "./leveloverrides.json";
            if(false == File.Exists(overrideFile))
            {
                FileLogger.QuietLogMessage($"Override file {overrideFile} not found");
                return;
            }

            string json = File.ReadAllText(overrideFile);
            LevelOverrides = JsonSerializer.Deserialize<LevelOverrides>(json);
        }
    }
}
