using ArkProjects.Wireguard.Mesh.CConverters;
using Microsoft.Extensions.Logging;

namespace ArkProjects.Wireguard.Deploy.DeployMethods
{
    public static partial class LocalFsWgDeploy
    {
        public class Factory : IWireguardDeployMethodFactory<Method>
        {
            IWireguardDeployMethodInfo IWireguardDeployMethodFactory<Method>.Info => Info;

            private readonly ILogger<Method> _logger;
            private readonly WgIniConfigConverter _iniConverter;
            private readonly WgUciConfigConverter _uciConverter;

            public Factory(ILogger<Method> logger, WgIniConfigConverter iniConverter, WgUciConfigConverter uciConverter)
            {
                _logger = logger;
                _iniConverter = iniConverter;
                _uciConverter = uciConverter;
            }

            public Method CreateInstance(WgDeployTargetConfig target)
            {
                return new Method(target, _logger, _iniConverter, _uciConverter);
            }
        }
    }
}