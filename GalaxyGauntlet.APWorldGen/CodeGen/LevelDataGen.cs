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
            List<string> ingameItems = [
                "game_items = {"
            ];
            List<string> levelNames = [
                "level_names = {"
            ];
            List<string> requiredItems = [
                "required_items = {"
            ];
            List<string> chipItems = [
                "chip_items = {"
            ];

            int itemId = 1000000;
            List<string> requiredKeysOrBoots = [];
            int levelIndex = 1;
            foreach(var level in LevelDataSet.Levels)
            {
                requiredItems.AddRange([
                    $"    \"Complete Map {levelIndex} - {level.LevelName}\": {itemId++},",
                    $"    \"Complete Map {levelIndex} - {level.LevelName} (No Resets)\": {itemId++},"
                ]);
                foreach(var requiredItem in level.ItemsPresent)
                {
                    if(false == requiredKeysOrBoots.Contains(requiredItem))
                    {
                        ingameItems.AddRange([
                            $"    \"{requiredItem}\": {itemId++},"
                        ]);
                        requiredKeysOrBoots.Add(requiredItem);
                    }
                }

                foreach(var chip in level.ChipLocations)
                {
                    chipItems.Add($"    \"Map {levelIndex} - {level.LevelName} Chip {chip}\": {itemId++},");
                }
                levelNames.Add($"    \"Map {levelIndex} - {level.LevelName}\": {itemId++},");
                levelIndex++;
            }

            string json = GGG.ToJson(LevelDataSet);
            levelData.Add(json);

            // Remove trailing comma off of last item in list
            levelData[^1] = levelData[^1][..^1];
            ingameItems[^1] = ingameItems[^1][..^1];
            levelNames[^1] = levelNames[^1][..^1];
            requiredItems[^1] = requiredItems[^1][..^1];
            chipItems[^1] = chipItems[^1][..^1];

            // Close dict objects
            levelData.AddRange(["}", ""]);
            ingameItems.AddRange(["}", ""]);
            levelNames.AddRange(["}", ""]);
            requiredItems.AddRange(["}", ""]);
            chipItems.Add("}");

            List<string> fileLines = [];
            fileLines.AddRange(levelData);
            fileLines.AddRange(ingameItems);
            fileLines.AddRange(levelNames);
            fileLines.AddRange(requiredItems);
            fileLines.AddRange(chipItems);
            WriteFileContents("level_data.py", fileLines);
            FileLogger.QuietLogMessage("File created");
        }
    }
}
