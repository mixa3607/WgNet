using System;
using ArkProjects.Wireguard.Deploy.DeployMethods;

namespace ArkProjects.Wireguard.Deploy.Exceptions
{
    public class DeployMethodNotInitialized : Exception
    {
        public DeployMethodNotInitialized(string message):base(message){}
        public DeployMethodNotInitialized() : this($"Call {nameof(IWireguardDeployMethod.Init)} before use this method"){}
    }
}