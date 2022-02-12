using PowerArgs;

namespace ArkProjects.Wireguard.Cli.Cli.Options
{
    public class WgCliBuildMeshOptions
    {
        [ArgShortcut("--cfg"), ArgShortcut("-c"), ArgDefaultValue("./mesh.json"), ArgDescription("Mesh config file")]
        public string Config { get; set; }

        [ArgShortcut("--out-dir"), ArgShortcut("-d"), ArgDefaultValue("./configs"), ArgDescription("Out directory for configs")]
        public string OutDir { get; set; }

        [ArgShortcut("--ignore-errs"), ArgDescription("Ignore all validation errors. If not set then only Note level allowed")]
        public bool IgnoreConfigErrors { get; set; }
    }
}