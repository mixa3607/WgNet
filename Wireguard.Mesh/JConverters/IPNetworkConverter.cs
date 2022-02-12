#nullable enable
using System;
using ArkProjects.Wireguard.Mesh.Misc;
using Newtonsoft.Json;

namespace ArkProjects.Wireguard.Mesh.JConverters
{
    // ReSharper disable once InconsistentNaming
    public class IPNetworkConverter : JsonConverter<IPNetwork>
    {
        public override IPNetwork? ReadJson(JsonReader reader, Type objectType, IPNetwork? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return reader.Value == null ? existingValue : new IPNetwork((string)reader.Value);
        }

        public override void WriteJson(JsonWriter writer, IPNetwork? value, JsonSerializer serializer)
        {
            writer.WriteValue(value?.ToString());
        }
    }
}