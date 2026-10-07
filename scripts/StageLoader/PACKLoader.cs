using System.Linq;

namespace GalaxyGauntlet.scripts
{
    public static partial class LevelLoader
    {
        public static void LoadPACKStage()
        {
            FileLogger.QuietLogMessage("Processing level PACK segment...");
            int sectionLength = GetSectionLength();
            byte[] packBytes = _fileBytes.SkipAndTake(_currentIndex, sectionLength);
            int currentPackIndex = 0;

            // Get list size
            byte[] uncompressedListSizeBytes = packBytes.SkipAndTake(0, 2);
            currentPackIndex += 2;
            int uncompressedListSize = ConvertToInt(uncompressedListSizeBytes);
            int firstDataBlockLength = packBytes[currentPackIndex++];
            if(firstDataBlockLength < 2)
            {
                FileLogger.QuietLogMessage($"Unexpected first data block length {firstDataBlockLength}");
                return;
            }

            // Get map size
            int mapSizeX = packBytes[currentPackIndex++];
            int mapSizeY = packBytes[currentPackIndex++];
            GameData.SetMapDimensions(mapSizeX, mapSizeY);

            byte[] uncompressedMap = PACKDecoder.Decode(packBytes);
            GameData.MapData = uncompressedMap;
            // DebugPrintMap(uncompressedMap);

            // Required chip count is derived from map data
            int requiredChips = 0;
            foreach(var data in uncompressedMap)
            {
                if(data == Constants.ByteCodes.Entities.Chip)
                {
                    requiredChips++;
                }
            }
            GameData.ChipsRequired = requiredChips;
            FileLogger.QuietLogMessage("PACK segment processed");
        }


        private static void DebugPrintMap(byte[] uncompressedMap)
        {
            bool doContinue = true;
            int skipAmount = 0;
            int mapDimension = 32;
            while(doContinue)
            {
                var entitiesToPrint = uncompressedMap.SkipAndTake(skipAmount * mapDimension, mapDimension);
                doContinue = (entitiesToPrint.Length == mapDimension);
                GD.Print(entitiesToPrint.Select(e => e.ToString("x2")).ToArray().Join(", "));
                skipAmount++;
            }
        }
    }
}
