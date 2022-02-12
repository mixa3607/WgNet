using PowerArgs;

namespace ArkProjects.Wireguard.Cli.Cli.Options
{
    public class WgCliValidateMeshOptions
    {
        [ArgShortcut("--cfg"), ArgShortcut("-c"), ArgDefaultValue("./mesh.json"), ArgDescription("Mesh config file")]
        public string Config { get; set; }
    }
}