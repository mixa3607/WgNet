using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using ArkProjects.Wireguard.Mesh.Misc;

namespace ArkProjects.Wireguard.Mesh.Configs
{
    public class WgMeshNodeConfig
    {
        public string Name { get; set; }
        public string Comment { get; set; }

        public string PrivateKey { get; set; }
        public string PublicKey { get; set; }

        public string InterfaceName { get; set; }
        public ushort? ListenPort { get; set; }
        public string Endpoint { get; set; }
        public IReadOnlyList<IPNetwork> BindAddresses { get; set; }
        public IReadOnlyList<IPNetwork> AdditionalAllowedIps { get; set; }
        public IReadOnlyList<IPAddress> Dns { get; set; }

        public string[] PreUp { get; set; }
        public string[] PreDown { get; set; }
        public string[] PostUp { get; set; }
        public string[] PostDown { get; set; }

        public ushort? Mtu { get; set; }

        [DefaultValue(WgHostInterfaceConfig.TableMode.Auto)]
        public string Table { get; set; } = WgHostInterfaceConfig.TableMode.Auto;

        public ushort? PersistentKeepalive { get; set; }
        public bool SaveConfig { get; set; }

        public WgMeshNodeTargetsMode TargetsMode { get; set; }
        public IReadOnlyList<string> Targets { get; set; }

        [DefaultValue(true)]
        public bool AddSelfAddresses { get; set; } = true;

        public IReadOnlyList<MutatedConfig> MutatedConfigs { get; set; }

        public class MutatedConfig
        {
            public string Name { get; set; }
            public IReadOnlyList<Override> Overrides { get; set; } = Array.Empty<Override>();

            public class Override
            {
                public string Node { get; set; }
                public bool InheritKeepalive { get; set; } = false;
                public IReadOnlyList<IPNetwork> AllowedIps { get; set; } = Array.Empty<IPNetwork>();
            }
        }
    }
}