namespace GalaxyGauntlet.Common
{
    public static class PACKDecoder
    {
        public static byte[] Decode(byte[] compressedData)
        {
            int pointerIndex = 0;

            // Determine size of uncompressed map data
            byte[] listSizeBytes = compressedData.Take(2).ToArray();
            int uncompressedListSize = ConvertToInt(listSizeBytes);
            pointerIndex += 2;

            // Determine length of first data block
            int firstDataBlockLength = compressedData[pointerIndex++];
            if(firstDataBlockLength < 2)
            {
                throw new($"Invalid first data block length {firstDataBlockLength}");
            }

            // Skip map size
            pointerIndex += 2;

            List<byte> resultList = [];
            for(int i = 0; i < firstDataBlockLength - 2; i++)
            {
                resultList.Add(compressedData[pointerIndex++]);
            }
            while(true)
            {
                int mapByte = compressedData[pointerIndex];
                if(mapByte < 0x80)
                {
                    // Data block
                    int readNextBytes = compressedData[pointerIndex]; // Same as mapByte
                    for(int i = 0; i < readNextBytes; i++)
                    {
                        pointerIndex++;
                        resultList.Add(compressedData[pointerIndex]);
                    }
                }
                else
                {
                    // Back-reference
                    int dataLength = mapByte - 0x80;
                    if(dataLength == 0)
                    {
                        pointerIndex += 2;
                        continue;
                    }

                    pointerIndex++;
                    int offset = compressedData[pointerIndex];
                    int copyEndIndex = resultList.Count;
                    int copyStartIndex = copyEndIndex - offset;
                    List<byte> bytesToCopy = resultList[copyStartIndex..copyEndIndex];
                    for(int i = 0; i < dataLength; i++)
                    {
                        resultList.Add(bytesToCopy[i % bytesToCopy.Count]);
                    }
                }
                pointerIndex++;

                if(resultList.Count + 2 >= uncompressedListSize)
                {
                    return resultList.ToArray();
                }
            }
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
    }
}
