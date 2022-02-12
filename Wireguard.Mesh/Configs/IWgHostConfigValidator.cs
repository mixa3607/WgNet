using System.Collections.Generic;
using ArkProjects.Wireguard.Mesh.Misc;

namespace ArkProjects.Wireguard.Mesh.Configs
{
    public interface IWgHostConfigValidator
    {
        IReadOnlyList<ConfigValidationMessage> Validate(WgHostConfig mesh);
    }
}