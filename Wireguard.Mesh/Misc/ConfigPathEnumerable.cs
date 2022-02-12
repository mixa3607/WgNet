using System.Collections.Generic;

namespace ArkProjects.Wireguard.Mesh.Misc;

public class ConfigPathEnumerable<T, TA> where T: IEnumerable<TA>
{
    private readonly string _path;
    public ConfigPathEnumerable(string basePath = "")
    {
        _path = basePath;
    }
    public ConfigPath<TA> Index(int idx)
    {
        var bakingCopy = new ConfigPath<TA>(string.IsNullOrEmpty(_path) ? $"[{idx}]" : _path + $"[{idx}]");
        return bakingCopy;
    }

    public override string ToString()
    {
        return _path;
    }

    public static implicit operator string(ConfigPathEnumerable<T, TA> d) => d.ToString();
}