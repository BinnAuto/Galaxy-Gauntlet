using GalaxyGauntlet.Common;

namespace GalaxyGauntlet.APWorldGen.CodeGen
{
    public static partial class APWorldCodeGen
    {
        public static void GenerateLocationsClass()
        {
            FileLogger.QuietLogMessage("Creating locations.py...");
            // TODO: Python code needs to be updated to mimic itemsgen, allowing
            // for location_name_to_id to work correctly
            List<string> fileLines = [
                "from __future__ import annotations",
                "from typing import TYPE_CHECKING",
                "from BaseClasses import Location",
                "from .level_data import level_data, required_items, chip_items",
                "import random",
                "",
                "if TYPE_CHECKING:",
                $"    from .world import {ClassPrefix}World",
                "",
                "all_locations: dict[str, int] = {}",
                "location_name_to_id:dict[str, int] = { key: value for key, value in {**required_items, **chip_items}.items()}",
                "",
                $"class {ClassPrefix}Location(Location):",
                $"    game = \"{GameName.Replace("\"", "\\\"")}\"",
                "",
                "def get_location_names_with_ids(location_names: list[str]) -> dict[str, int | None]:",
                "    return {l: all_locations[l] for l in location_names}",
                "",
                $"def create_all_locations(world: {ClassPrefix}World) -> None:",
                "    all_locations.clear()",
                "    main_world = world.get_region(\"Main Game\")",
                "    ## Add level completion locations",
                "    for level in level_data[\"levels\"]:",
                "        level_name = level[\"level_name\"]",
                "",
                "        ## Add check for completing the level",
                "        level_region = world.get_region(level_name)",
                "        all_locations[level_name] = location_name_to_id[level_name]",
                "        level_region.add_locations(get_location_names_with_ids([level_name]))",
                "",
                "        ## Add check for completing the level with no death or reset",
                "        level_no_reset_name = f\"{level_name} (No Resets)\"",
                "        all_locations[level_no_reset_name] = location_name_to_id[level_no_reset_name]",
                "        level_region.add_locations(get_location_names_with_ids([level_no_reset_name]))",
                "",
                "        ## Add chip checks from each level",
                "        if world.options.chip_as_check_probability.value > 0:",
                "            for chip_location in level[\"chip_locations\"]:",
                "                include_chip: bool = True",
                "                if world.options.chip_as_check_probability < 100:",
                "                    rand: int = random.randint(1, 100)",
                "                    include_chip = (rand < world.options.chip_as_check_probability)",
                "                if include_chip:",
                "                    chip_location_name = f\"{level_name} Chip {chip_location}\"",
                "                    all_locations[chip_location_name] = location_name_to_id[chip_location_name]",
                "                    level_region.add_locations(get_location_names_with_ids([chip_location_name]))"
            ];

            WriteFileContents("locations.py", fileLines);
            FileLogger.QuietLogMessage("File created");
        }
    }
}
