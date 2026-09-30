using System.Text.Json.Serialization;

namespace GalaxyGauntlet.APWorldGen
{
    public class LevelOverrides
    {
        [JsonPropertyName("level_overrides")]
        public LevelOverride[] Overrides { get; set; }
    }

    public class LevelOverride
    {
        [JsonPropertyName("level_name")]
        public string LevelName { get; set; }

        [JsonPropertyName("ignore_chips")]
        public string[] IgnoreChips { get; set; }
    }
}
