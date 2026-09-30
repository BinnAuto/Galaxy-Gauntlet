using GalaxyGauntlet.APWorldGen.CodeGen;
using GalaxyGauntlet.Common;
using Godot;
using System.Security.Cryptography;

namespace GalaxyGauntlet.APWorldGen
{
    internal class Program
    {
        private const string LevelDirectory = "./levels";

        static void Main(string[] _)
        {
            try
            {
                FileLogger.QuietLogMessage("Program start");

                #region Initialize Program

                APWorldCodeGen.LoadLevelOverrides();

                if (false == Directory.Exists(LevelDirectory))
                {
                    Directory.CreateDirectory(LevelDirectory);
                    FileLogger.LogMessage("Level directory created");
                }

                var files = Directory.GetFiles(LevelDirectory, "*.c2m");
                if(files.Length == 0)
                {
                    throw new($"No c2m files found in levels folder");
                }

                #endregion

                #region Get name of game from user

                Console.Write("Enter a game name: ");
                APWorldCodeGen.GameName = Console.ReadLine();
                while(string.IsNullOrEmpty(APWorldCodeGen.GameName) 
                    || string.IsNullOrEmpty(APWorldCodeGen.ClassPrefix))
                {
                    FileLogger.QuietLogMessage($"Game name entered: {APWorldCodeGen.GameName}");
                    if(APWorldCodeGen.GameName == "Chip's Challenge"
                        || APWorldCodeGen.GameName == "Chip's Challenge 2")
                    {
                        FileLogger.LogMessage($"The game name \"{APWorldCodeGen.GameName}\" may cause conflicts if other players are playing the base {APWorldCodeGen.GameName} Archipelago. Please enter a different game name.");
                        continue;
                    }
                    Console.Write($"Invalid game name. Enter a game name: ");
                    APWorldCodeGen.GameName = Console.ReadLine();
                }
                FileLogger.QuietLogMessage($"Finalized game name: {APWorldCodeGen.GameName}");

                #endregion

                #region Get author name

                Console.Write("Enter your username, as author of this APWorld: ");
                APWorldCodeGen.AuthorName = Console.ReadLine();
                FileLogger.QuietLogMessage($"Finalized author name: {APWorldCodeGen.GameName}");

                #endregion

                List<byte> combinedFileData = [];
                int fileIndex = 1;
                string filePath = $"./levels/map{fileIndex:000}.c2m";
                while(File.Exists(filePath))
                {
                    try
                    {
                        FileLogger.LogMessage($"Processing file {filePath}...");
                        combinedFileData.AddRange(File.ReadAllBytes(filePath));
                        byte[] mapData = FileLoader.LoadMapData(filePath);
                        APWorldCodeGen.BuildLevelData(mapData);
                        FileLogger.QuietLogMessage($"Level {FileLoader.LevelName} added to list");
                        fileIndex++;
                        filePath = $"./levels/map{fileIndex:000}.c2m";
                    }
                    catch(Exception e)
                    {
                        FileLogger.LogMessage($"Error processing file {filePath}");
                        throw;
                    }
                }
                FileLogger.LogMessage($"File {filePath} not found, assuming end of level sequence");
                ValidateLevelData();
                var stageHash = GetFileHash(combinedFileData);
                APWorldCodeGen.GenerateClassFiles(stageHash);
            }
            catch(Exception e)
            {
                FileLogger.QuietLogMessage("!!!========================!!!");
                FileLogger.LogMessage("Error occurred");
                FileLogger.LogMessage(e.Message);
                FileLogger.QuietLogMessage(e.StackTrace);
                FileLogger.QuietLogMessage("!!!========================!!!");
            }
            FileLogger.LogMessage("Program complete");
            Console.WriteLine("Press enter to exit.");
            Console.Read();
        }


        private static void ValidateLevelData()
        {
            if (APWorldCodeGen.LevelNames.Count < 8)
            {
                FileLogger.LogMessage("WARNING: A proper level sequence must have at least eight levels. Some game options may cause generation to fail.");
            }

            if(false == APWorldCodeGen.LevelDataSet.Levels.Any(e => e.ItemsRequired.Length == 0))
            {
                throw new("A valid level sequence must have at least one level that does not use keys or boots.");
            }
        }


        private static string GetFileHash(List<byte> combinedFileData)
        {
            SHA256 sha256 = SHA256.Create();
            var hash = SHA256.HashData([..combinedFileData]);
            return hash.Select(e => $"{e:x2}").ToArray().Join("");
        }
    }
}
