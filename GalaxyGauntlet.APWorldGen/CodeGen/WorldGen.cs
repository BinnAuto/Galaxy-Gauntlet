using GalaxyGauntlet.Common;

namespace GalaxyGauntlet.APWorldGen.CodeGen
{
    public static partial class APWorldCodeGen
    {
        public static void GenerateWorldClass(string stageHash)
        {
            FileLogger.QuietLogMessage("Creating world.py...");
            List<string> fileLines = [
                "from collections.abc import Mapping",
                "from typing import Any",
                "import random",
                "from BaseClasses import Tutorial",
                "from worlds.AutoWorld import World, WebWorld",
                "from .locations import all_locations",
                "from . import items, locations, regions, rules, options as cc_options",
                "",
                $"class {ClassPrefix}WebWorld(WebWorld):",
                $"    game = \"{GameName.Replace("\"", "\\\"")}\"",
                "    theme = \"grassFlowers\"",
                "    setup_en = Tutorial(",
                "        \"Multiworld Setup Guide\",",
                $"        \"A guide to setting up {GameName.Replace("\"", "\\\"")} for Multiworld.\",",
                "        \"English\",",
                "        \"setup_en.md\",",
                "        \"setup/en\",",
                $"        [\"{AuthorName.Replace("\"", "\\\"")}\", \"BinnAuto\"]",
                "    )",
                "",
                $"class {ClassPrefix}World(World):",
                "    # Chip's Challenge is a top-down tile-based puzzle video game originally published in 1989",
                "    # by Epyx for the Atari Lynx, but has been ported to other systems.",
                "    # This implementation approximates the version of the game released for Windows 3.1",
                $"    game = \"{GameName.Replace("\"", "\\\"")}\"",
                $"    web = {ClassPrefix}WebWorld()",
                $"    options_dataclass = cc_options.{ClassPrefix}Options",
                $"    options: cc_options.{ClassPrefix}Options",
                "    origin_region_name = \"Main Game\"",
                "    location_name_to_id = locations.location_name_to_id",
                "    item_name_to_id = items.item_name_to_id",
                "",
                "    def create_regions(self) -> None:",
                "        regions.create_and_connect_regions(self)",
                "        locations.create_all_locations(self)",
                "",
                "    def set_rules(self) -> None:",
                "        rules.set_all_rules(self)",
                "",
                "    def create_items(self) -> None:",
                "        items.create_all_items(self)",
                "        item_name, _ = random.choice(list(items.base_levels.items()))",
                "        self.push_precollected(self.create_item(item_name))",
                "",
                $"    def create_item(self, item_name:str) -> items.{ClassPrefix}Item:",
                "        item = items.all_items[item_name]",
                "        return items.create_item(self, item_name, item.classification, item.code)",
                "",
                "    def get_filler_item_name(self) -> str:",
                "        return \"Filler\"",
                "",
                "    def fill_slot_name(self) -> Mapping[str, Any]:", 
                "        return {}",
                "",
                "    def fill_slot_data(self):",
                $"        slot_options:list[str] = [\"{Constants.Archipelago.SlotDataKeys.PlayerSprite}\", \"{Constants.Archipelago.SlotDataKeys.DeathLink}\"]",
                "        slot_data = {option_name: getattr(self.options, option_name).value for option_name in slot_options}",
                $"        slot_data[\"{Constants.Archipelago.SlotDataKeys.LevelHash}\"] = \"{stageHash}\"",
                "        return slot_data"
            ];

            WriteFileContents("world.py", fileLines);
            FileLogger.QuietLogMessage("File created");
        }
    }
}
