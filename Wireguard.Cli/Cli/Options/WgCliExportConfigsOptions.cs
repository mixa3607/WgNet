using ArkProjects.Wireguard.Mesh.CConverters;
using PowerArgs;

namespace ArkProjects.Wireguard.Cli.Cli.Options
{
    public class WgCliExportConfigsOptions
    {
        [ArgShortcut("--names"), ArgShortcut("-n"), ArgDescription("Deploy selected targets instead all")]
        public string[] Names { get; set; }

        [ArgShortcut("--type"), ArgShortcut("-t")]
        public WgConfigGenType Type { get; set; }

        [ArgShortcut("--out-dir"), ArgShortcut("-d"), ArgDefaultValue("./export")]
        public string OutDir { get; set; }

        [ArgShortcut("--in-dir"), ArgShortcut("-s"), ArgDefaultValue("./configs"), ArgDescription("Directory with host configs")]
        public string InDir { get; set; }

        [ArgShortcut("--ignore-errs"), ArgDescription("Ignore all validation errors. If not set then only Note level allowed")]
        public bool IgnoreConfigErrors { get; set; }
    }
}