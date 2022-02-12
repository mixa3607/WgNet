using System.Collections.Generic;
using System.IO;
using ArkProjects.Wireguard.Mesh.JConverters;
using Newtonsoft.Json;

namespace ArkProjects.Wireguard.Mesh.Configs
{
    public class WgMeshConfigManager
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings()
        {
            Converters = new List<JsonConverter> { new IPAddressConverter(), new IPNetworkConverter() },
            DefaultValueHandling = DefaultValueHandling.Ignore,
            Formatting = Formatting.Indented
        };

        public static WgMeshConfig LoadFile(string filePath)
        {
            var jsonStr = File.ReadAllText(filePath);
            return Load(jsonStr);
        }

        public static WgMeshConfig Load(string jsonStr)
        {
            return JsonConvert.DeserializeObject<WgMeshConfig>(jsonStr, JsonSerializerSettings);
        }

        public static void SaveAsFile(WgMeshConfig config, string filePath, bool createPath = false)
        {
            if (createPath)
            {
                var dir = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
            }
            File.WriteAllText(filePath, Save(config));
        }

        public static string Save(WgMeshConfig config)
        {
            return JsonConvert.SerializeObject(config, JsonSerializerSettings);
        }
    }
}