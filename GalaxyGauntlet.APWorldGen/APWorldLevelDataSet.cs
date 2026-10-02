using GalaxyGauntlet.Common;
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


        public void AddItem(int byteCode)
        {
            string item = byteCode switch
            {
                Constants.ByteCodes.Entities.RedKey => "Red Key",
                Constants.ByteCodes.Entities.BlueKey => "Blue Key",
                Constants.ByteCodes.Entities.YellowKey => "Yellow Key",
                Constants.ByteCodes.Entities.GreenKey => "Green Key",
                Constants.ByteCodes.Entities.IceSkates => "Ice Skates",
                Constants.ByteCodes.Entities.SuctionBoots => "Suction Boots",
                Constants.ByteCodes.Entities.FireBoots => "Fire Boots",
                Constants.ByteCodes.Entities.Flippers => "Flippers",
                Constants.ByteCodes.Entities.HikingBoots => "Hiking Boots",
                _ => throw new($"Item code {byteCode} not recognized")
            };
            if (ItemsPresent.Contains(item))
            {
                return;
            }

            ItemsPresent = ItemsPresent.Append(item).ToArray();
        }
    }
}
