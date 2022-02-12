using PowerArgs;

namespace ArkProjects.Wireguard.Cli.Cli.Options
{
    public class WgCliGenerateKeysOptions
    {
        [ArgShortcut("--cfg"), ArgShortcut("-c"), ArgDefaultValue("./mesh.json"), ArgDescription("Mesh config file")]
        public string Config { get; set; }

        [ArgShortcut("--out"), ArgShortcut("-o"), ArgDescription("Out file. Use input file by default")]
        public string OutConfig { get; set; }

        [ArgShortcut("-p"), ArgShortcut("--preshared"), ArgDefaultValue(true), ArgDescription("Generate pre-shared keys")]
        public bool WithPreShared { get; set; } = true;

        [ArgShortcut("--regenerate"), ArgDefaultValue(false), ArgDescription("Drop all keys and generate new")]
        public bool ReGenerate { get; set; }

        [ArgShortcut("--fix"), ArgDefaultValue(true), ArgDescription("Fix invalid keys")]
        public bool FixInvalid { get; set; }
    }
}