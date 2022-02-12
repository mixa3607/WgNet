using ArkProjects.Wireguard.Mesh.CConverters;
using Microsoft.Extensions.Logging;

namespace ArkProjects.Wireguard.Deploy.DeployMethods
{
    public static partial class SshUciBatchWgDeploy
    {
        public class Factory : IWireguardDeployMethodFactory<Method>
        {
            IWireguardDeployMethodInfo IWireguardDeployMethodFactory<Method>.Info => Info;

            private readonly ILogger<Method> _logger;

            public Factory(ILogger<Method> logger)
            {
                _logger = logger;
            }

            public Method CreateInstance(WgDeployTargetConfig target)
            {
                return new Method(target, _logger, new WgUciConfigConverter(null));
            }
        }
    }
}