using Godot;
using System.Text;
using System.Text.Json;

namespace GalaxyGauntlet.Common
{
    public static class Global
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


        public static string[] GetLevelList(GameMode gameMode)
        {
            switch (gameMode)
            {
                case GameMode.Archipelago:
                    return GetLevelList("archipelago");

                default:
                    return GetLevelList();
            }
        }


        public static string[] GetLevelList()
        {
            return GetLevelList(string.Empty);
        }


        public static string[] GetLevelList(string subDirectoryOverride)
        {
            string localLevelPath = "./levels";
            Directory.CreateDirectory(localLevelPath);
            string fileSourcePath = localLevelPath;
            if(false == string.IsNullOrEmpty(subDirectoryOverride))
            {
                string subDirectory = $"{localLevelPath}/{subDirectoryOverride}";
                if(Directory.Exists(subDirectory))
                {
                    fileSourcePath = subDirectory;
                }
            }
            string[] files = Directory.GetFiles(fileSourcePath, "*.c2m", SearchOption.AllDirectories);
            return files;
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
    }
}
