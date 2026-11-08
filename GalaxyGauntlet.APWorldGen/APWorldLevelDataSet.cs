using GalaxyGauntlet.Common;
using System.Text.Json.Serialization;

namespace GalaxyGauntlet.APWorldGen
{
    public class APWorldLevelDataSet
    {
        [JsonPropertyName("game_items")]
        public string[] LevelItems { get; set; } = ["Red Key", "Blue Key", "Green Key", "Yellow Key", "Suction Boots", "Fire Boots", "Ice Skates", "Flippers", "Secret Eye", "Helmet"];

        [JsonPropertyName("levels")]
        public APLevelData[] Levels { get; set; } = [];
    }


    public class APLevelData
    {
        [JsonPropertyName("level_name")]
        public string LevelName { get; set; } = string.Empty;

        [JsonPropertyName("level_number")]
        public int LevelNumber { get; set; }

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
                GGConstants.ByteCodes.Entities.RedKey => "Red Key",
                GGConstants.ByteCodes.Entities.BlueKey => "Blue Key",
                GGConstants.ByteCodes.Entities.YellowKey => "Yellow Key",
                GGConstants.ByteCodes.Entities.GreenKey => "Green Key",
                GGConstants.ByteCodes.Entities.IceSkates => "Ice Skates",
                GGConstants.ByteCodes.Entities.SuctionBoots => "Suction Boots",
                GGConstants.ByteCodes.Entities.FireBoots => "Fire Boots",
                GGConstants.ByteCodes.Entities.Flippers => "Flippers",
                GGConstants.ByteCodes.Entities.HikingBoots => "Hiking Boots",
                GGConstants.ByteCodes.Entities.Helmet => "Helmet",
                GGConstants.ByteCodes.Entities.SecretEye => "Secret Eye",
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
