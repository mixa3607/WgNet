namespace ArkProjects.Wireguard.Mesh.Misc
{
    public class ConfigValidationMessage
    {
        public ConfigValidationMessage(string path, string message, ConfigValidationMessageLevel level = ConfigValidationMessageLevel.Note)
        {
            Level = level;
            Path = path;
            Message = message;
        }

        public string Path { get; set; }
        public string Message { get; set; }
        public ConfigValidationMessageLevel Level { get; set; }

        public override string ToString() => $"[{Level}] {Path}: {Message}";
    }
}