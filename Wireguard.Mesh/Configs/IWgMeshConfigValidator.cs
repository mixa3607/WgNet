using System.Collections.Generic;
using ArkProjects.Wireguard.Mesh.Misc;

namespace ArkProjects.Wireguard.Mesh.Configs
{
    public interface IWgMeshConfigValidator
    {
        IReadOnlyList<ConfigValidationMessage> Validate(WgMeshConfig mesh);
    }
}