using GalaxyGauntlet.Common;

namespace GalaxyGauntlet.APWorldGen.CodeGen
{
    public static partial class APWorldCodeGen
    {
        public static void GenerateItemClass()
        {
            FileLogger.QuietLogMessage("Creating items.py...");
            List<string> fileLines = [
                "from __future__ import annotations",
                "from .level_data import level_data, required_items, level_items",
                "from typing import TYPE_CHECKING",
                "from BaseClasses import Item, ItemClassification",
                "",
                "if TYPE_CHECKING:",
                $"    from .world import {ClassPrefix}World",
                "",
                $"class {ClassPrefix}Item(Item):",
                $"    game = \"{GameName.Replace("\"", "\\\"")}\"",
                "",
                $"base_levels:dict[str, {ClassPrefix}Item] = {{}}",
                $"all_items:dict[str, {ClassPrefix}Item] = {{}}",
                "item_name_to_id:dict[str, int] = { key: value for key, value in {**required_items, **level_items, **{\"Filler\": 1}}.items() }",
                "",
                $"def create_item(world:{ClassPrefix}World, item_name:str, classification:ItemClassification, item_id:int) -> {ClassPrefix}Item:",
                "    item_from_table = all_items.get(item_name)",
                $"    return {ClassPrefix}Item(item_name, item_from_table.classification, item_id, world.player)",
                "",
                $"def create_all_items(world:{ClassPrefix}World) -> None:",
                "    items:list[Item] = []",
                "",
                "    ## Add level unlock items",
                "    for level in level_data[\"levels\"]:",
                "        level_name = level[\"level_name\"]",
                $"        cc_item:{ClassPrefix}Item = {ClassPrefix}Item(level_name, ItemClassification.progression, item_name_to_id[level_name], world.player)",
                "        all_items[level_name] = cc_item",
                "        item_name_to_id[cc_item.name] = cc_item.code",
                "        item = world.create_item(level_name)",
                "        items.append(item)",
                "        level_required_items:list[str] = level[\"items_required\"]",
                "        ## Add level to base item pool if it does not require game items to complete",
                "        if len(level_required_items) == 0:",
                "            base_levels[level_name] = cc_item",
                "",
                "    ## Add key and boot items",
                "    game_items = [\"Blue Key\", \"Green Key\", \"Red Key\", \"Yellow Key\", \"Fire Boots\", \"Flippers\", \"Ice Skates\", \"Suction Boots\"]",
                "    for game_item in game_items:",
                $"        cc_item:{ClassPrefix}Item = {ClassPrefix}Item(game_item, ItemClassification.progression, item_name_to_id[game_item], world.player)",
                "        all_items[game_item] = cc_item",
                "        item_name_to_id[cc_item.name] = cc_item.code",
                "        item = world.create_item(game_item)",
                "        items.append(item)",
                "",
                "    ## Add filler item",
                $"    filler_item:{ClassPrefix}Item = {ClassPrefix}Item(\"Filler\", ItemClassification.filler, item_name_to_id[\"Filler\"], world.player)",
                "    all_items[\"Filler\"] = filler_item",
                "    item_name_to_id[filler_item.name] = filler_item.code",
                "    item = world.create_item(\"Filler\")",
                "    items.append(item)",
                "",
                "    num_items:int = len(items)",
                "    num_unfilled_locations:int = len(world.multiworld.get_unfilled_locations(world.player))",
                "    if num_unfilled_locations > 0:",
                "        print(f\"Adding filler for {num_unfilled_locations} location(s)\")",
                "        filler_item_count:int = num_unfilled_locations - num_items",
                "        items += [world.create_filler() for _ in range(filler_item_count)]",
                "",
                "    world.multiworld.itempool += items"
            ];

            WriteFileContents("items.py", fileLines);
            FileLogger.QuietLogMessage("File created");
        }
    }
}
