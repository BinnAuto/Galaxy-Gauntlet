using System.Text;
using System.Text.Json;

namespace GalaxyGauntlet.Common
{
    public static class SCGlobal
    {
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
    }
}
