using System;
using System.IO;

namespace ArkProjects.Wireguard.Deploy
{
    public static class FilesHelper
    {
        public static string GetScript(string method, string scriptName) => File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "files/scripts", method, scriptName));
    }
}