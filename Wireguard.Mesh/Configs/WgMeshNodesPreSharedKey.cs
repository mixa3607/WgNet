using System.Collections.Generic;

namespace ArkProjects.Wireguard.Mesh.Configs;

public class WgMeshNodesPreSharedKey
{
    public IReadOnlyList<string> Nodes { get; set; }
    public string PreSharedKey { get; set; }
}