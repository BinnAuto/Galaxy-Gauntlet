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
                int maximumLevelScore = (int)(0.8 * 250 * LevelNames.Count * (LevelNames.Count + 1));
                return (int)(1.2f * maximumLevelScore); // Conservative score extension
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


        public static void BuildLevelData(byte[] mapData, int levelNumber)
        {
            FileLogger.QuietLogMessage("Processing map data");
            List<string> chipCoordinates = [];
            APLevelData levelData = new()
            {
                LevelName = FileLoader.LevelName,
                LevelNumber = levelNumber
            };
            int x = 0;
            int y = 0;
            var levelOverride = LevelOverrides.Overrides.FirstOrDefault(e => string.Equals(e.LevelName, FileLoader.LevelName, StringComparison.InvariantCultureIgnoreCase));
            for(int i = 0; i < mapData.Length; i++)
            {
                byte mapByte = mapData[i];
                switch(mapByte)
                {
                    case GGConstants.ByteCodes.Entities.Floor:
                    case GGConstants.ByteCodes.Entities.Wall:
                        break;

                    case GGConstants.ByteCodes.Entities.Chip:
                    case GGConstants.ByteCodes.Entities.ExtraChip:
                        chipCoordinates.Add($"({x}, {y})");
                        i++;
                        if (mapData[i] == GGConstants.ByteCodes.Entities.DirtBlock
                            || mapData[i] == GGConstants.ByteCodes.Entities.Teeth)
                        {
                            i += 2; // Entity orientation and lower layer
                        }
                        break;

                    // Direction and Tile specification
                    case GGConstants.ByteCodes.Entities.Player:
                    case GGConstants.ByteCodes.Entities.DirtBlock:
                    case GGConstants.ByteCodes.Entities.Walker:
                    case GGConstants.ByteCodes.Entities.Glider:
                    case GGConstants.ByteCodes.Entities.IceBlock:
                    case GGConstants.ByteCodes.Entities.BlueTank:
                    case GGConstants.ByteCodes.Entities.Bug:
                    case GGConstants.ByteCodes.Entities.Paramecium:
                    case GGConstants.ByteCodes.Entities.Ball:
                    case GGConstants.ByteCodes.Entities.Blob:
                    case GGConstants.ByteCodes.Entities.Teeth:
                    case GGConstants.ByteCodes.Entities.Fireball:
                    case GGConstants.ByteCodes.Entities.Melinda:
                    case GGConstants.ByteCodes.Entities.TimidTeeth:
                    case GGConstants.ByteCodes.Entities.Explosion:
                    case GGConstants.ByteCodes.Entities.YellowTank:
                    case GGConstants.ByteCodes.Entities.MirrorPlayer:
                    case GGConstants.ByteCodes.Entities.MirrorMelinda:
                    case GGConstants.ByteCodes.Entities.Rover:
                    case GGConstants.ByteCodes.Entities.ThinWallOrCanopy:
                        i += 2;
                        break;

                    case GGConstants.ByteCodes.Entities.ThinWall_E:
                    case GGConstants.ByteCodes.Entities.ThinWall_S:
                    case GGConstants.ByteCodes.Entities.ThinWall_SE:
                    case GGConstants.ByteCodes.Entities.RedBomb:
                    case GGConstants.ByteCodes.Entities.TimeBonus:
                    case GGConstants.ByteCodes.Entities.Stopwatch:
                    case GGConstants.ByteCodes.Entities.TimeBomb:
                    case GGConstants.ByteCodes.Entities.LightningBolt:
                    case GGConstants.ByteCodes.Entities.BowlingBall:
                    case GGConstants.ByteCodes.Entities.TimePenalty:
                        i++;
                        break;

                    case GGConstants.ByteCodes.Entities.RedKey:
                    case GGConstants.ByteCodes.Entities.BlueKey:
                    case GGConstants.ByteCodes.Entities.YellowKey:
                    case GGConstants.ByteCodes.Entities.GreenKey:
                    case GGConstants.ByteCodes.Entities.IceSkates:
                    case GGConstants.ByteCodes.Entities.SuctionBoots:
                    case GGConstants.ByteCodes.Entities.FireBoots:
                    case GGConstants.ByteCodes.Entities.Flippers:
                    case GGConstants.ByteCodes.Entities.HikingBoots:
                    case GGConstants.ByteCodes.Entities.Helmet:
                    case GGConstants.ByteCodes.Entities.SecretEye:
                        levelData.AddItem(mapByte);
                        i++;
                        if (mapData[i] == GGConstants.ByteCodes.Entities.DirtBlock
                            || mapData[i] == GGConstants.ByteCodes.Entities.Teeth)
                        {
                            i += 2; // Entity orientation and lower layer
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
