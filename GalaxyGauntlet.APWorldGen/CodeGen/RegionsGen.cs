using GalaxyGauntlet.Common;

namespace GalaxyGauntlet.APWorldGen.CodeGen
{
    public static partial class APWorldCodeGen
    {
        public static void GenerateRegionsClass()
        {
            FileLogger.QuietLogMessage("Creating regions.py...");
            List<string> fileLines = [
                "from __future__ import annotations",
                "from typing import TYPE_CHECKING",
                "from BaseClasses import Region",
                "from .level_data import level_data",
                "",
                "if TYPE_CHECKING:",
                $"    from .world import {ClassPrefix}World",
                "",
                $"def create_and_connect_regions(world: {ClassPrefix}World) -> None:",
                "    create_all_regions(world)",
                "    connect_regions(world)",
                "",
                $"def create_all_regions(world: {ClassPrefix}World) -> None:",
                "    world.multiworld.regions += [Region(\"Main Game\", world.player, world.multiworld)]",
                "    for level in level_data[\"levels\"]:",
                "        level_name = level[\"level_name\"]",
                "        world.multiworld.regions += [Region(level_name, world.player, world.multiworld)]",
                "",
                $"def connect_regions(world: {ClassPrefix}World) -> None:",
                "    main_game = world.get_region(\"Main Game\")",
                "    for level in level_data[\"levels\"]:",
                "        level_name = level[\"level_name\"]",
                "        level_region = world.get_region(level_name)",
                "        main_game.connect(level_region, f\"Main to {level_name}\")"
            ];

            WriteFileContents("regions.py", fileLines);
            FileLogger.QuietLogMessage("File created");
        }
    }
}
