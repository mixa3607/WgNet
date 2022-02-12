namespace ArkProjects.Wireguard.Deploy.DeployMethods
{
    public static partial class SshQuickWgDeploy
    {
        public class Options
        {
            /// <summary>
            /// SSH host
            /// </summary>
            public string Host { get; set; }

            /// <summary>
            /// SSH port
            /// </summary>
            public int Port { get; set; } = 22;

            /// <summary>
            /// Shell that used for execute scripts
            /// </summary>
            public string Shell { get; set; } = "sh";

            /// <summary>
            /// SSH user
            /// </summary>
            public string User { get; set; }

            /// <summary>
            /// SSH/sudo user password
            /// </summary>
            /// <remarks>
            /// Required if <see cref="PrivateKeyFile"/> not set or <see cref="OverSudo"/> enabled
            /// </remarks>
            public string Password { get; set; }

            /// <summary>
            /// Path to private key. Or provide <see cref="Password"/>
            /// </summary>
            public string PrivateKeyFile { get; set; }

            /// <summary>
            /// Phrase for private key. Used if <see cref="PrivateKeyFile"/> require it
            /// </summary>
            public string PrivateKeyFilePhrase { get; set; }

            /// <summary>
            /// Execute all commands over sudo
            /// </summary>
            public bool OverSudo { get; set; } = true;

            /// <summary>
            /// sudo -u &lt;user&gt;
            /// </summary>
            public string SudoUser { get; set; } = "root";

            /// <summary>
            /// Directory for configs
            /// </summary>
            public string RemoteConfigsDirectory { get; set; } = "/etc/wireguard";

            /// <summary>
            /// Enable wg interface in systemctl
            /// </summary>
            public bool SysCtlEnable { get; set; }
        }
    }
}