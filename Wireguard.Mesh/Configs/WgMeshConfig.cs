using System;
using System.Collections.Generic;

namespace ArkProjects.Wireguard.Mesh.Configs
{
    public class WgMeshConfig
    {
        public IReadOnlyList<WgMeshNodeConfig> Nodes { get; set; } = Array.Empty<WgMeshNodeConfig>();
        public IReadOnlyList<WgMeshNodesPreSharedKey> NodesPreSharedKeys { get; set; }
    }
}