using ArkProjects.Wireguard.Deploy;
using PowerArgs;

namespace ArkProjects.Wireguard.Cli.Options
{
    public class WgCliDeployConfigsOptions
    {
        [ArgShortcut("--cfg"), ArgShortcut("-c"), ArgDefaultValue("./deploy.json"), ArgDescription("Deploy config")]
        public string Config { get; set; }

        [ArgShortcut("--names"), ArgShortcut("-n"), ArgDescription("Deploy selected targets instead all")]
        public string[] Names { get; set; }

        [ArgShortcut("-t"), ArgDefaultValue(WgDeployStageType.All), ArgDescription("Execute only selected stages")]
        public WgDeployStageType TargetStages { get; set; }

        [ArgShortcut("--list-methods"), ArgDescription("List all methods and exit")]
        public bool ListMethods { get; set; }
    }
}