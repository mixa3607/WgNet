using System;

namespace ArkProjects.Wireguard.Deploy.DeployMethods
{
    public class WireguardDeployMethodInfo : IWireguardDeployMethodInfo
    {
        public string Name { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public Version Version { get; set; }
    }
}