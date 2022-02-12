using Newtonsoft.Json.Linq;

namespace ArkProjects.Wireguard.Deploy
{
    public class WgDeployTargetConfig
    {
        public string Name { get; set; }
        public string DeployMethod { get; set; }
        public JObject DeployMethodOptions { get; set; }
        public string ConfigFile { get; set; }

        public bool AllowCheckFail { get; set; } = false;
        public bool AllowDownFail { get; set; } = true;
        public bool AllowRemoveFail { get; set; } = true;
        public bool AllowUploadFail { get; set; } = false;
        public bool AllowUpFail { get; set; } = false;
    }
}