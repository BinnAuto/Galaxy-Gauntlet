using GalaxyGauntlet.APWorldGen.CodeGen;
using GalaxyGauntlet.Common;
using Godot;
using System.Security.Cryptography;

namespace GalaxyGauntlet.APWorldGen
{
    internal class Program
    {
        private const string LevelDirectory = "./levels";

        static void Main(string[] args)
        {
            try
            {
                FileLogger.QuietLogMessage("Program start");
                ParseArguments(args);
                CheckForLevelDirectory();
                APWorldCodeGen.LoadLevelOverrides();
                var files = CheckLevelList();
                PromptAPGameName();
                PromptAPWorldAuthor();

                List<byte> combinedFileData = [];
                int levelIndex = 1;
                foreach(string file in files)
                {
                    try
                    {
                        FileLogger.LogMessage($"Processing file {file}...");
                        byte[] mapData = FileLoader.LoadMapData(file);
                        APWorldCodeGen.BuildLevelData(mapData, levelIndex);
                        FileLogger.QuietLogMessage($"Map {levelIndex} - {FileLoader.LevelName} added to list");
                        combinedFileData.AddRange(File.ReadAllBytes(file));
                        levelIndex++;
                    }
                    catch (Exception e)
                    {
                        FileLogger.LogException($"Error processing file {file}", e);
                    }
                }
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


        private static void ParseArguments(string[] args)
        {
            if(args.Length == 0)
            {
                return;
            }

            try
            {
                for(int i = 0; i < args.Length; i++)
                {
                    switch(args[i].ToLower())
                    {
                        case "-g":
                        case "-game":
                            i++;
                            APWorldCodeGen.GameName = args[i];
                            if (APWorldCodeGen.GameName == "Chip's Challenge"
                                || APWorldCodeGen.GameName == "Chip's Challenge 2")
                            {
                                FileLogger.LogMessage($"WARNING: The game name \"{APWorldCodeGen.GameName}\" may cause conflicts if other players are playing the base {APWorldCodeGen.GameName} Archipelago.");
                            }
                            break;

                        case "-a":
                        case "-author":
                            i++;
                            APWorldCodeGen.AuthorName = args[i];
                            break;

                        default:
                            throw new($"Argument {args[i]} not recognized.");
                    }
                }
            }
            catch(Exception e)
            {
                FileLogger.LogException("Error occurred while parsing arguments", e);
                APWorldCodeGen.GameName = string.Empty;
                APWorldCodeGen.AuthorName = string.Empty;
            }
        }


        private static void CheckForLevelDirectory()
        {
            if (false == Directory.Exists(LevelDirectory))
            {
                Directory.CreateDirectory(LevelDirectory);
                FileLogger.LogMessage("Level directory created");
            }
        }


        private static string[] CheckLevelList()
        {
            var result = GGG.GetLevelList();
            if (result.Length == 0)
            {
                throw new($"No c2m files found in levels folder");
            }
            // Remove files to ignore
            result = result
                .Where(e => false == 
                    APWorldCodeGen.LevelOverrides.IgnoreFiles.Any(f => 
                        e.EndsWith(f, StringComparison.InvariantCultureIgnoreCase)))
                .ToArray();

            return result;
        }


        /// <summary>
        /// Prompt the user for the name of the game for this AP World
        /// </summary>
        private static void PromptAPGameName()
        {
            if(false == string.IsNullOrEmpty(APWorldCodeGen.GameName))
            {
                return;
            }

            Console.Write("Enter a game name: ");
            APWorldCodeGen.GameName = Console.ReadLine();
            while (string.IsNullOrEmpty(APWorldCodeGen.GameName)
                || string.IsNullOrEmpty(APWorldCodeGen.ClassPrefix))
            {
                FileLogger.QuietLogMessage($"Game name entered: {APWorldCodeGen.GameName}");
                if (APWorldCodeGen.GameName == "Chip's Challenge"
                    || APWorldCodeGen.GameName == "Chip's Challenge 2")
                {
                    FileLogger.LogMessage($"The game name \"{APWorldCodeGen.GameName}\" may cause conflicts if other players are playing the base {APWorldCodeGen.GameName} Archipelago. Please enter a different game name.");
                    continue;
                }
                Console.Write($"Invalid game name. Enter a game name: ");
                APWorldCodeGen.GameName = Console.ReadLine();
            }
            FileLogger.QuietLogMessage($"Finalized game name: {APWorldCodeGen.GameName}");
        }


        /// <summary>
        /// Prompt the user for the author name of this AP World
        /// </summary>
        private static void PromptAPWorldAuthor()
        {
            if(false == string.IsNullOrEmpty(APWorldCodeGen.AuthorName))
            {
                return;
            }

            Console.Write("Enter your username, as author of this APWorld: ");
            APWorldCodeGen.AuthorName = Console.ReadLine();
            while(string.IsNullOrEmpty(APWorldCodeGen.AuthorName))
            {
                Console.Write("Please specify an author: ");
                APWorldCodeGen.AuthorName = Console.ReadLine();
            }
            FileLogger.QuietLogMessage($"Finalized author name: {APWorldCodeGen.GameName}");
        }


        /// <summary>
        /// Validate that the given level set can be used to generate a proper Archipelago randomizer.
        /// </summary>
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
