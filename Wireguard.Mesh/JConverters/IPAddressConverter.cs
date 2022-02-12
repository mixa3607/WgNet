#nullable enable
using System;
using System.Net;
using Newtonsoft.Json;

namespace ArkProjects.Wireguard.Mesh.JConverters
{
    // ReSharper disable once InconsistentNaming
    public class IPAddressConverter : JsonConverter<IPAddress>
    {
        public override IPAddress? ReadJson(JsonReader reader, Type objectType, IPAddress? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return reader.Value == null ? existingValue : IPAddress.Parse((string)reader.Value);
        }

        public override void WriteJson(JsonWriter writer, IPAddress? value, JsonSerializer serializer)
        {
            writer.WriteValue(value?.ToString());
        }
    }
}