using ArkProjects.Wireguard.Mesh.CConverters;
using Microsoft.Extensions.Logging;

namespace ArkProjects.Wireguard.Deploy.DeployMethods
{
    public static partial class SshQuickWgDeploy
    {
        public class Factory : IWireguardDeployMethodFactory<Method>
        {
            IWireguardDeployMethodInfo IWireguardDeployMethodFactory<Method>.Info => Info;

            private readonly ILogger<Method> _logger;
            private readonly WgIniConfigConverter _iniConverter;

            public Factory(ILogger<Method> logger, WgIniConfigConverter iniConverter)
            {
                _logger = logger;
                _iniConverter = iniConverter;
            }

            public Method CreateInstance(WgDeployTargetConfig target)
            {
                return new Method(target, _logger, _iniConverter);
            }
        }
    }
}