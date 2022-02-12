namespace ArkProjects.Wireguard.Deploy.DeployMethods
{
    public static partial class SshUciBatchWgDeploy
    {
        public class Options
        {
            public string Host { get; set; }
            public int Port { get; set; } = 22;

            public string User { get; set; }
            public string Password { get; set; }
            public string PrivateKeyFile { get; set; }
            public string PrivateKeyFilePhrase { get; set; }

            public string FirewallZone { get; set; }
            public bool AutoStart { get; set; }
        }
    }
}