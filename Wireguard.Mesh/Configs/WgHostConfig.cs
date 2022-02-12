using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ArkProjects.Wireguard.Mesh.Configs
{
    [DebuggerDisplay("{Name}")]
    public class WgHostConfig
    {
        public string Name { get; set; }
        public string Comment { get; set; }
        public string InterfaceName { get; set; }

        public DateTime? MeshGenDate { get; set; }
        public Version MeshGenVersion { get; set; }

        public WgHostInterfaceConfig Interface { get; set; }
        public IReadOnlyList<WgHostPeerConfig> Peers { get; set; } = Array.Empty<WgHostPeerConfig>();
    }
}