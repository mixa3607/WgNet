using System;

namespace ArkProjects.Wireguard.Deploy.DeployMethods
{
    public interface IWireguardDeployMethodInfo
    {
        string Name { get; }
        string Author { get; }
        string Description { get; }
        Version Version { get; }
    }
}