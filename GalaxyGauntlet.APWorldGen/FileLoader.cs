using Godot;
using GalaxyGauntlet.APWorldGen.CodeGen;
using GalaxyGauntlet.Common;
using System.Text;

namespace GalaxyGauntlet.APWorldGen
{
    public static class FileLoader
    {
        public static Vector2I MapDimensions = Vector2I.Zero;

        public static string? LevelName { get; private set; }

        private static int _currentIndex = 0;

        private static byte[]? _fileData;

        public static byte[] LoadMapData(string filePath)
        {
            LevelName = string.Empty;
            _currentIndex = 0;
            _fileData = File.ReadAllBytes(filePath);
            MapDimensions = Vector2I.Zero;

            byte[] result = [];
            bool doContinue = true;
            while(doContinue)
            {
                string sectionHeader = GetSectionHeader();
                switch(sectionHeader)
                {
                    case "TITL":
                        FileLogger.QuietLogMessage("TITL section found");
                        int titleLength = GetSectionLength();
                        byte[] titleBytes = GetNextBytes(titleLength);
                        LevelName = ConvertToString(titleBytes).Trim('\0');
                        APWorldCodeGen.LevelNames.Add(LevelName);
                        FileLogger.QuietLogMessage($"Level name: {LevelName}");
                        break;

                    case "PACK":
                        FileLogger.QuietLogMessage("PACK section found");
                        int packLength = GetSectionLength();
                        byte[] packBytes = _fileData.SkipAndTake(_currentIndex, packLength);
                        int mapSizeX = packBytes[3];
                        int mapSizeY = packBytes[4];
                        MapDimensions = new(mapSizeX, mapSizeY);
                        result = PACKDecoder.Decode(packBytes);
                        FileLogger.QuietLogMessage("Map data unpacked");
                        _currentIndex += packLength;
                        break;

                    case "END":
                        FileLogger.QuietLogMessage("END section found");
                        doContinue = false;
                        break;

                    default:
                        FileLogger.QuietLogMessage($"Skipping section {sectionHeader}");
                        int sectionLength = GetSectionLength();
                        _currentIndex += sectionLength;
                        break;
                }
            }

            if(result.Length == 0)
            {
                throw new("PACK section not found on file");
            }
            if(string.IsNullOrEmpty(LevelName))
            {
                throw new("TITL section not found on file");
            }
            return result;
        }


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

        #region Helper Methods    

        private static string ConvertToString(byte[] input)
        {
            return Encoding.ASCII.GetString(input);
        }


        private static int ConvertToInt(byte[] input, bool reverseEndianness = false)
        {
            if (reverseEndianness)
            {
                input = ReverseEndianness(input);
            }
            if (input.Length == 2)
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


        private static byte[] GetNextBytes(int count)
        {
            byte[] result = _fileData.SkipAndTake(_currentIndex, count);
            _currentIndex += count;
            return result;
        }


        private static T[] SkipAndTake<T>(this T[] input, int skip, int take)
        {
            return input.Skip(skip).Take(take).ToArray();
        }

        #endregion
    }
}
