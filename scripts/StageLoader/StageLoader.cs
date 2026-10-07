using GalaxyGauntlet.scripts.MapEntities.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalaxyGauntlet.scripts
{
    public static partial class LevelLoader
    {
        private static byte[] _fileBytes;

        private static int _currentIndex = 0;

        public static string GetLevelName(string filePath)
        {
            _currentIndex = 0;
            _fileBytes = File.ReadAllBytes(filePath);
            bool doContinue = true;
            while(doContinue)
            {
                string sectionHeader = GetSectionHeader();
                if(sectionHeader == "TITL")
                {
                    return GetMapTitle();
                }

                int length = GetSectionLength();
                _currentIndex += length;
            }

            return Path.GetFileNameWithoutExtension(filePath);
        }


        public static void LoadLevel(string filePath)
        {
            try
            {
                FileLogger.QuietLogMessage($"Loading level file {filePath}");
                InitializeValues();
                _fileBytes = File.ReadAllBytes(filePath);
                bool doContinue = true;
                while(doContinue)
                {
                    int length = 0;
                    string sectionHeader = GetSectionHeader();
                    switch (sectionHeader)
                    {
                        // Ignore following strings
                        case "CC2M": // File version
                        case "LOCK": 
                        case "AUTH": // Author
                        case "NOTE": // Notes
                        case "KEY": // ??
                        case "REPL": // Replay
                        case "PRPL": // Packed Replay
                        case "VERS": // Editor version
                            // Ignore
                            length = GetSectionLength();
                            _currentIndex += length;
                            break;

                        case "TITL":
                            GameData.LevelName = GetMapTitle();
                            FileLogger.QuietLogMessage($"Level name: {GameData.LevelName}");
                            break;

                        case "CLUE":
                            GetHint();
                            break;

                        case "OPTN":
                            LoadOptions();
                            break;

                        case "MAP":
                            break;

                        case "PACK":
                            LoadPACKStage();
                            GameData.PopulateFromMapBytes();
                            break;

                        case "END":
                            doContinue = false;
                            break;

                        default:
                            FileLogger.QuietLogMessage($"Unrecognized file header {sectionHeader}");
                            doContinue = false;
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                FileLogger.LogException($"Error processing file at byte {_currentIndex + 1}", e);
            }
        }


        private static void InitializeValues()
        {
            GameData.ResetMapData();
            _fileBytes = [];
            _currentIndex = 0;
        }

        #region Section Methods

        private static string GetSectionHeader()
        {
            byte[] headerBytes = GetNextBytes(4);
            string result = ConvertToString(headerBytes);
            result = result.Trim();
            return result;
        }


        private static int GetSectionLength()
        {
            byte[] lengthBytes = GetNextBytes(4);
            int sectionLength = ConvertToInt(lengthBytes);
            return sectionLength;
        }


        private static string GetMapTitle()
        {
            int titleLength = GetSectionLength();
            byte[] titleBytes = GetNextBytes(titleLength);
            string title = ConvertToString(titleBytes).Trim('\0');
            return title;
        }


        private static void GetHint()
        {
            int hintLength = GetSectionLength();
            byte[] hintBytes = GetNextBytes(hintLength);
            string hint = ConvertToString(hintBytes);
            if(false == string.IsNullOrEmpty(hint))
            {
                hint = hint.Replace("\r\n", " ");
            }
            GameData.LevelHint = hint;
        }


        private static void LoadOptions()
        {
            FileLogger.QuietLogMessage("Processing level OPTN segment...");
            int optionLength = GetSectionLength();
            byte[] optionBytes = GetNextBytes(optionLength);
            byte[] timeBytes = optionBytes.SkipAndTake(0, 2);
            GameData.TimeLimit = ConvertToInt(timeBytes);
            FileLogger.QuietLogMessage($"Level timer: {GameData.TimeLimit}");
            FileLogger.QuietLogMessage("OPTN segment processed");
        }

        #endregion


        #region File Read Methods


        private static T[] SkipAndTake<T>(this T[] input, int skip, int take)
        {
            return input.Skip(skip).Take(take).ToArray();
        }


        private static List<T> SkipAndTake<T>(this List<T> input, int skip, int take)
        {
            return input.Skip(skip).Take(take).ToList();
        }


        private static void DebugPrintArray(byte[] input)
        {
            string print = input.Select(e => $"{e:x2}").ToArray().Join(", ");
            GD.Print(print);
        }


        private static byte PeekByte()
        {
            return _fileBytes[_currentIndex];
        }


        private static byte ReadByte()
        {
            byte result = PeekByte();
            _currentIndex++;
            return result;
        }


        private static byte[] GetNextBytes(int count)
        {
            byte[] result = _fileBytes.SkipAndTake(_currentIndex, count);
            _currentIndex += count;
            return result;
        }


        private static string ConvertToString(byte[] input)
        {
            return Encoding.ASCII.GetString(input);
        }


        private static int ConvertToInt(byte[] input, bool reverseEndianness = false)
        {
            if(reverseEndianness)
            {
                input = ReverseEndianness(input);
            }
            if(input.Length == 2)
            {
                return BitConverter.ToInt16(input);
            }
            return BitConverter.ToInt32(input);
        }


        private static byte[] ReverseEndianness(byte[] bytes)
        {
            byte[] result = bytes.Reverse().ToArray();
            return result;            
        }

        #endregion
    }
}
