using Godot;
using System.Text;
using System.Text.Json;

namespace GalaxyGauntlet.Common
{
    public static class GGG
    {
        private static readonly double VolumeMultiplier = 0.015;

        public static string ToJson(object value)
        {
            var jsonDocument = JsonSerializer.SerializeToDocument(value);
            using MemoryStream mStream = new();
            Utf8JsonWriter writer = new(mStream, new() { Indented = true });
            jsonDocument.WriteTo(writer);
            writer.Flush();
            string json = Encoding.UTF8.GetString(mStream.ToArray());
            mStream.Dispose();
            return json;
        }


        public static string[] GetLevelList()
        {
            string localLevelPath = "./levels";
            Directory.CreateDirectory(localLevelPath);
            string[] files = Directory.GetFiles(localLevelPath, "*.c2m", SearchOption.AllDirectories);
            return files;
        }

        #region Audio Methods

        public static string GetRandomMusicFile(string directory)
        {
            return GetRandomMusicFile(directory, true);
        }


        public static AudioStream BuildAudioStream(string filePath)
        {
            // TODO: Add support for WAV and OGG file types
            AudioStreamMP3 result = new()
            {
                Data = File.ReadAllBytes(filePath)
            };
            return result;
        }


        public static string GetRandomMusicFile(string directory, bool fallbackToRoot)
        {
            // TODO: Add support for WAV and OGG file types
            string[] files = Directory.GetFiles(directory, "*.mp3");
            if(files.Length == 0)
            {
                if(false == fallbackToRoot)
                {
                    return string.Empty;
                }

                files = Directory.GetFiles("./custom/music", "*.mp3");
            }
            if(files.Length == 0)
            {
                return string.Empty;
            }

            RandomNumberGenerator rng = new();
            int songIndex = rng.RandiRange(0, files.Length - 1);
            string result = files[songIndex];
            return result;
        }


        public static void SetBusVolume(string busName, double value)
        {
            int busIndex = AudioServer.GetBusIndex(busName);
            if(busIndex < 0)
            {
                return;
            }

            value *= VolumeMultiplier;
            AudioServer.SetBusVolumeLinear(busIndex, (float)value);
        }

        #endregion
    }
}
