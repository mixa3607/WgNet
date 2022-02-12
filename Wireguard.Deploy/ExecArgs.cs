using System.Collections.Generic;
using Renci.SshNet;

namespace ArkProjects.Wireguard.Deploy
{
    public class ExecArgs
    {
        public SshClient Client { get; set; }
        public string Shell { get; set; } = "sh";

        public bool OverSudo { get; set; }
        public string SudoUser { get; set; }
        public string Password { get; set; }

        public string Script { get; set; }
        public IReadOnlyList<string> Args { get; set; }
    }
}