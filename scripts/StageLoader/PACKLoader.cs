using System.Linq;

namespace GalaxyGauntlet.scripts
{
    public static partial class LevelLoader
    {
        public static void LoadPACKStage()
        {
            GD.Print("Unpacking map data");
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
                return;
            }

            // Get map size
            int mapSizeX = packBytes[currentPackIndex++];
            int mapSizeY = packBytes[currentPackIndex++];
            GameData.SetMapDimensions(mapSizeX, mapSizeY);

            byte[] uncompressedMap = PACKDecoder.Decode(packBytes);
            GameData.MapData = uncompressedMap;

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
        }


        private static void PrintMap()
        {
            bool doContinue = true;
            int skipAmount = 0;
            int mapDimension = 32;
            while(doContinue)
            {
                var entitiesToPrint = _mapBytes.SkipAndTake(skipAmount * mapDimension, mapDimension);
                doContinue = (entitiesToPrint.Count == mapDimension);
                GD.Print(entitiesToPrint.Select(e => e.DataCode.ToString("x2")).ToArray().Join(", "));
                skipAmount++;
            }
        }
    }
}
