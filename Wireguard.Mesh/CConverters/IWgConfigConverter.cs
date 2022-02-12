using ArkProjects.Wireguard.Mesh.Configs;

namespace ArkProjects.Wireguard.Mesh.CConverters
{
    public interface IWgConfigConverter
    {
        string Extension { get; }
        WgConfigGenType GenType { get; }
        void SaveFile(string filePath, WgHostConfig config);
        string Save(WgHostConfig config);
    }
}