using PowerArgs;

namespace ArkProjects.Wireguard.Cli.Cli.Options
{
    public class WgCliValidateConfigsOptions
    {
        [ArgShortcut("--names"), ArgShortcut("-n"), ArgDescription("Deploy selected targets instead all")]
        public string[] Names { get; set; }

        [ArgShortcut("--in-dir"), ArgShortcut("-s"), ArgDefaultValue("./configs"), ArgDescription("Directory with host configs")]
        public string InDir { get; set; }
    }
}