using System.IO;
using ArkProjects.Wireguard.Mesh.JConverters;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ArkProjects.Wireguard.Mesh.Configs
{
    public static class WgMeshConfigManager
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings = new()
        {
            Converters = new JsonConverter[] { new IPAddressConverter(), new IPNetworkConverter(), new StringEnumConverter() },
            DefaultValueHandling = DefaultValueHandling.Ignore,
            Formatting = Formatting.Indented
        };

        /// <summary>
        /// Load config fom file
        /// </summary>
        public static WgMeshConfig LoadFile(string filePath)
        {
            var jsonStr = File.ReadAllText(filePath);
            return Load(jsonStr);
        }

        /// <summary>
        /// Load config from file
        /// </summary>
        public static WgMeshConfig Load(string jsonStr)
        {
            return JsonConvert.DeserializeObject<WgMeshConfig>(jsonStr, JsonSerializerSettings);
        }

        /// <summary>
        /// Save config to file
        /// </summary>
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

        /// <summary>
        /// Convert config to json string
        /// </summary>
        public static string Save(WgMeshConfig config)
        {
            return JsonConvert.SerializeObject(config, JsonSerializerSettings);
        }
    }
}