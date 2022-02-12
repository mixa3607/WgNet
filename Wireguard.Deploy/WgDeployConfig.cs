using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace ArkProjects.Wireguard.Deploy
{
    public class WgDeployConfig
    {
        public IReadOnlyList<WgDeployTargetConfig> Targets { get; set; } = Array.Empty<WgDeployTargetConfig>();

        #region Load/Save

        public static WgDeployConfig LoadFile(string filePath)
        {
            var jsonStr = File.ReadAllText(filePath);
            return Load(jsonStr);
        }

        public static WgDeployConfig Load(string jsonStr)
        {
            return JsonConvert.DeserializeObject<WgDeployConfig>(jsonStr);
        }

        public string Save()
        {
            return JsonConvert.SerializeObject(this);
        }

        public void SaveFile(string filePath)
        {
            File.WriteAllText(filePath, Save());
        }

        #endregion
    }
}