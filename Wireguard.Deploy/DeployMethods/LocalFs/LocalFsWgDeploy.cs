using System;

namespace ArkProjects.Wireguard.Deploy.DeployMethods
{
    public static partial class LocalFsWgDeploy
    {
        private static readonly IWireguardDeployMethodInfo Info = new WireguardDeployMethodInfo()
        {
            Author = "mixa3607",
            Description = "Deploy to local fs",
            Name = "local_fs",
            Version = new Version(1, 0, 0)
        };
    }
}