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
