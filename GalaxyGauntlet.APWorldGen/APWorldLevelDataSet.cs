using System.Text.Json.Serialization;

namespace GalaxyGauntlet.APWorldGen
{
    public class APWorldLevelDataSet
    {
        [JsonPropertyName("level_items")]
        public string[] LevelItems { get; set; } = ["Red Key", "Blue Key", "Green Key", "Yellow Key", "Suction Boots", "Fire Boots", "Ice Skates", "Flippers"];

        [JsonPropertyName("levels")]
        public APLevelData[] Levels { get; set; } = [];
    }


    public class APLevelData
    {
        [JsonPropertyName("level_name")]
        public string LevelName { get; set; } = string.Empty;

        [JsonPropertyName("items_present")]
        public string[] ItemsPresent { get; set; } = [];

        [JsonPropertyName("items_required")]
        public string[] ItemsRequired { get; set; } = [];

        [JsonPropertyName("chip_locations")]
        public string[] ChipLocations { get; set; } = [];


        public void AddItem(string item)
        {
            if(ItemsPresent.Contains(item))
            {
                return;
            }

            ItemsPresent = ItemsPresent.Append(item).ToArray();
        }
    }
}
