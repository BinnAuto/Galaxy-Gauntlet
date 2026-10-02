using GalaxyGauntlet.Common;

namespace GalaxyGauntlet.APWorldGen.CodeGen
{
    public static partial class APWorldCodeGen
    {
        public static string OutputPath = "./apworldout";

        public static string GameName = string.Empty;

        public static string AuthorName = string.Empty;

        public static List<string> LevelNames = [];

        public static List<string> ChipItems = [];

        public static APWorldLevelDataSet LevelDataSet = new();


        #region Derived Values

        public static int MaximumScore
        {
            get
            {
                return (int)(0.8 * 250 * LevelNames.Count * (LevelNames.Count + 1));
            }
        }


        public static bool IncludeExtendedScoreChecks
        {
            get
            {
                return (MaximumScore > 600000);
            }
        }


        public static string ClassPrefix
        {
            get
            {
                string alphaChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
                string result = string.Empty;
                foreach(char c in GameName)
                {
                    if (alphaChars.Contains(c))
                    {
                        result += c;
                    }
                }
                return result;
            }
        }

        #endregion


        public static void GenerateClassFiles(string stageHash)
        {
            FileLogger.LogMessage("Generating Python classes...");
            GenerateInitFile();
            GenerateItemClass();
            GenerateLevelData();
            GenerateLocationsClass();
            GenerateOptionClass();
            GenerateRegionsClass();
            GenerateRulesClass();
            GenerateWorldClass(stageHash);
        }


        public static void WriteFileContents(string fileName, List<string> fileLines)
        {
            Directory.CreateDirectory(OutputPath);
            string filePath = $"{OutputPath}/{fileName}";
            File.WriteAllLines(filePath, fileLines);
        }


        public static void BuildLevelData(byte[] mapData)
        {
            FileLogger.QuietLogMessage("Processing map data");
            List<string> chipCoordinates = [];
            APLevelData levelData = new()
            {
                LevelName = FileLoader.LevelName
            };
            int x = 0;
            int y = 0;
            var levelOverride = LevelOverrides.Overrides.FirstOrDefault(e => string.Equals(e.LevelName, FileLoader.LevelName, StringComparison.InvariantCultureIgnoreCase));
            for(int i = 0; i < mapData.Length; i++)
            {
                byte mapByte = mapData[i];
                switch(mapByte)
                {
                    case Constants.ByteCodes.Entities.Floor:
                    case Constants.ByteCodes.Entities.Wall:
                        break;

                    case Constants.ByteCodes.Entities.Chip:
                    case Constants.ByteCodes.Entities.ExtraChip:
                        chipCoordinates.Add($"({x}, {y})");
                        i++;
                        if (mapData[i] == 0x17)
                        {
                            i += 2;
                        }
                        break;

                    // Direction and Tile specification
                    case Constants.ByteCodes.Entities.Player:
                    case Constants.ByteCodes.Entities.DirtBlock:
                    case Constants.ByteCodes.Entities.Walker:
                    case Constants.ByteCodes.Entities.Glider:
                    case Constants.ByteCodes.Entities.IceBlock:
                    case Constants.ByteCodes.Entities.BlueTank:
                    case Constants.ByteCodes.Entities.Bug:
                    case Constants.ByteCodes.Entities.Paramecium:
                    case Constants.ByteCodes.Entities.Ball:
                    case Constants.ByteCodes.Entities.Blob:
                    case Constants.ByteCodes.Entities.Teeth:
                    case Constants.ByteCodes.Entities.Fireball:
                    case Constants.ByteCodes.Entities.Melinda:
                    case Constants.ByteCodes.Entities.TimidTeeth:
                    case Constants.ByteCodes.Entities.Explosion:
                    case Constants.ByteCodes.Entities.YellowTank:
                    case Constants.ByteCodes.Entities.MirrorPlayer:
                    case Constants.ByteCodes.Entities.MirrorMelinda:
                    case Constants.ByteCodes.Entities.Rover:
                    case Constants.ByteCodes.Entities.ThinWallOrCanopy:
                        i += 2;
                        break;

                    case Constants.ByteCodes.Entities.ThinWall_E:
                    case Constants.ByteCodes.Entities.ThinWall_S:
                    case Constants.ByteCodes.Entities.ThinWall_SE:
                    case Constants.ByteCodes.Entities.RedBomb:
                    case Constants.ByteCodes.Entities.TimeBonus:
                    case Constants.ByteCodes.Entities.Stopwatch:
                    case Constants.ByteCodes.Entities.TimeBomb:
                    case Constants.ByteCodes.Entities.Helmet:
                    case Constants.ByteCodes.Entities.LightningBolt:
                    case Constants.ByteCodes.Entities.BowlingBall:
                    case Constants.ByteCodes.Entities.TimePenalty:
                        i++;
                        break;

                    case Constants.ByteCodes.Entities.RedKey:
                    case Constants.ByteCodes.Entities.BlueKey:
                    case Constants.ByteCodes.Entities.YellowKey:
                    case Constants.ByteCodes.Entities.GreenKey:
                    case Constants.ByteCodes.Entities.IceSkates:
                    case Constants.ByteCodes.Entities.SuctionBoots:
                    case Constants.ByteCodes.Entities.FireBoots:
                    case Constants.ByteCodes.Entities.Flippers:
                    case Constants.ByteCodes.Entities.HikingBoots:
                        levelData.AddItem(mapByte);
                        i++;
                        if (mapData[i] == Constants.ByteCodes.Entities.DirtBlock)
                        {
                            i += 2; // Dirt block "orientation" and lower layer
                        }
                        break;

                    default:
                        break;
                }
                x++;
                if(x >= FileLoader.MapDimensions.X)
                {
                    x = 0;
                    y++;
                    if(y >= FileLoader.MapDimensions.Y)
                    {
                        FileLogger.QuietLogMessage("Y-coordinate exceeded map dimensions.");
                        break;
                    }
                }
            }
            levelData.ChipLocations = chipCoordinates.ToArray();
            if(levelOverride != null)
            {
                if(levelOverride.IgnoreChips != null && levelOverride.IgnoreChips.Length > 0)
                {
                    levelData.ChipLocations = levelData.ChipLocations.Where(e =>
                    {
                        return !levelOverride.IgnoreChips.Contains(e);
                    }).ToArray();
                }
            }
            levelData.ItemsRequired = levelData.ItemsPresent;
            LevelDataSet.Levels = LevelDataSet.Levels.Append(levelData).ToArray();
        }
    }
}
