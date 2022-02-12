using ArkProjects.Wireguard.Mesh.CConverters;

namespace ArkProjects.Wireguard.Deploy.DeployMethods
{
    public static partial class LocalFsWgDeploy
    {
        public class Options
        {
            public string Dir { get; set; }
            public string File { get; set; }
            public bool CreateDirs { get; set; } = true;
            public WgConfigGenType ConvertTo { get; set; }
        }
    }
}