using GalaxyGauntlet.Common;

namespace GalaxyGauntlet.APWorldGen.CodeGen
{
    public static partial class APWorldCodeGen
    {
        public static void GenerateRulesClass()
        {
            FileLogger.QuietLogMessage("Creating rules.py...");
            string firstLevelName = LevelNames[0];
            List<string> fileLines = [
                "from __future__ import annotations",
                "from typing import TYPE_CHECKING",
                "from rule_builder.rules import HasAll, CanReachLocation",
                "from .level_data import level_data",
                "",
                "if TYPE_CHECKING:",
                $"    from .world import {ClassPrefix}World",
                "",
                $"def set_all_rules(world: {ClassPrefix}World) -> None:",
                "    set_entrance_rules(world)",
                "    set_location_rules(world)",
                "    set_completion_condition(world)",
                "",
                $"def set_entrance_rules(world: {ClassPrefix}World) -> None:",
                "    for level in level_data[\"levels\"]:",
                "        level_name = level[\"level_name\"]",
                "        items_required:list[str] = level[\"items_required\"]",
                "        items_required.append(level_name)",
                "        entrance_rule = HasAll(*items_required)",
                "        main_to_level = world.get_entrance(f\"Main to {level_name}\")",
                "        world.set_rule(main_to_level, entrance_rule)",
                "",
                $"def set_location_rules(world: {ClassPrefix}World) -> None:",
                "    pass",
                "",
                $"def set_completion_condition(world: {ClassPrefix}World) -> None:",
                $"    rule:CanReachLocation = CanReachLocation(\"{firstLevelName}\")",
                "    for level in level_data[\"levels\"]:",
                "        level_name = level[\"level_name\"]",
                $"        if level_name != \"{firstLevelName}\":",
                "            rule = rule & CanReachLocation(level_name)",
                "    world.set_completion_rule(rule)"
            ];

            WriteFileContents("rules.py", fileLines);
            FileLogger.QuietLogMessage("File created");
        }
    }
}
