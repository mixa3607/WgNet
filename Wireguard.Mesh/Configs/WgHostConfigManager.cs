using System.Collections.Generic;
using System.IO;
using ArkProjects.Wireguard.Mesh.JConverters;
using Newtonsoft.Json;

namespace ArkProjects.Wireguard.Mesh.Configs
{
    public class WgHostConfigManager
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings()
        {
            Converters = new List<JsonConverter> { new IPAddressConverter(), new IPNetworkConverter() },
            DefaultValueHandling = DefaultValueHandling.Ignore,
            Formatting = Formatting.Indented
        };

        public static WgHostConfig LoadFile(string filePath)
        {
            var jsonStr = File.ReadAllText(filePath);
            return Load(jsonStr);
        }

        public static WgHostConfig Load(string jsonStr)
        {
            return JsonConvert.DeserializeObject<WgHostConfig>(jsonStr, JsonSerializerSettings);
        }

        public static void SaveAsFile(WgHostConfig config, string filePath, bool createPath = false)
        {
            if (createPath)
            {
                var dir = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir)) 
                    Directory.CreateDirectory(dir);
            }
            File.WriteAllText(filePath, Save(config));
        }

        public static string Save(WgHostConfig config)
        {
            return JsonConvert.SerializeObject(config, JsonSerializerSettings);
        }
    }
}