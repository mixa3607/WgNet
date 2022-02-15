using System;
using System.IO;
using ArkProjects.Wireguard.Mesh.JConverters;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ArkProjects.Wireguard.Mesh.Configs
{
    public static class WgHostConfigManager
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings()
        {
            Converters = new JsonConverter[] { new IPAddressConverter(), new IPNetworkConverter(), new StringEnumConverter() },
            DefaultValueHandling = DefaultValueHandling.Ignore,
            Formatting = Formatting.Indented
        };

        /// <summary>
        /// Load config fom file
        /// </summary>
        public static WgHostConfig LoadFile(string filePath)
        {
            if (filePath == null) 
                throw new ArgumentNullException(nameof(filePath));

            var jsonStr = File.ReadAllText(filePath);
            return Load(jsonStr);
        }

        /// <summary>
        /// Load config from file
        /// </summary>
        public static WgHostConfig Load(string jsonStr)
        {
            return JsonConvert.DeserializeObject<WgHostConfig>(jsonStr, JsonSerializerSettings);
        }

        /// <summary>
        /// Save config to file
        /// </summary>
        public static void SaveAsFile(WgHostConfig config, string filePath, bool createPath = false)
        {
            if (filePath == null) 
                throw new ArgumentNullException(nameof(filePath));

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
        public static string Save(WgHostConfig config)
        {
            return JsonConvert.SerializeObject(config, JsonSerializerSettings);
        }
    }
}