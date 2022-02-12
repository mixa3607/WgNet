namespace ArkProjects.Wireguard.Mesh.Configs;

public enum WgMeshNodeTargetsMode : byte
{
    /// <summary>
    /// Exclude targets that listed in <see cref="WgMeshNodeConfig.Targets"/>
    /// </summary>
    Exclude = 0,

    /// <summary>
    /// Include only targets that listed in <see cref="WgMeshNodeConfig.Targets"/>
    /// </summary>
    Include = 1,
}