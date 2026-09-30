using GalaxyGauntlet.Common;

namespace GalaxyGauntlet.APWorldGen.CodeGen
{
    public static partial class APWorldCodeGen
    {
        public static void GenerateLevelData()
        {
            FileLogger.QuietLogMessage("Creating level_data.py...");
            List<string> levelData = [
                "level_data = \\"
            ];
            List<string> levelItems = [
                "level_items = {"
            ];
            List<string> requiredItems = [
                "required_items = {"
            ];
            List<string> chipItems = [
                "chip_items = {"
            ];

            int itemId = 1000000;
            List<string> requiredKeysOrBoots = [];
            foreach(var level in LevelDataSet.Levels)
            {
                requiredItems.AddRange([
                    $"    \"{level.LevelName}\": {itemId++},",
                    $"    \"{level.LevelName} (No Resets)\": {itemId++},"
                ]);
                foreach(var requiredItem in level.ItemsPresent)
                {
                    if(false == requiredKeysOrBoots.Contains(requiredItem))
                    {
                        levelItems.AddRange([
                            $"    \"{requiredItem}\": {itemId++},"
                        ]);
                        requiredKeysOrBoots.Add(requiredItem);
                    }
                }

                foreach(var chip in level.ChipLocations)
                {
                    chipItems.Add($"    \"{level.LevelName} Chip {chip}\": {itemId++},");
                }
            }

            string json = SCGlobal.ToJson(LevelDataSet);
            levelData.Add(json);

            // Remove trailing comma off of last item in list
            levelData[^1] = levelData[^1][..^1];
            levelItems[^1] = levelItems[^1][..^1];
            requiredItems[^1] = requiredItems[^1][..^1];
            chipItems[^1] = chipItems[^1][..^1];

            // Close dict objects
            levelData.AddRange(["}", ""]);
            levelItems.AddRange(["}", ""]);
            requiredItems.AddRange(["}", ""]);
            chipItems.Add("}");

            List<string> fileLines = [];
            fileLines.AddRange(levelData);
            fileLines.AddRange(levelItems);
            fileLines.AddRange(requiredItems);
            fileLines.AddRange(chipItems);
            WriteFileContents("level_data.py", fileLines);
            FileLogger.QuietLogMessage("File created");
        }
    }
}
