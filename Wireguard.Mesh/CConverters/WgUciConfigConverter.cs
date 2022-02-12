using System;
using System.IO;
using System.Reflection;
using System.Text;
using ArkProjects.Wireguard.Mesh.Configs;

namespace ArkProjects.Wireguard.Mesh.CConverters
{
    public class WgUciConfigConverter : IWgConfigConverter
    {
        private static readonly Version Version = Assembly.GetAssembly(typeof(WgUciConfigConverter))?.GetName().Version;
        private readonly WgConfigGeneratorInfo _info;

        public WgConfigGenType GenType => WgConfigGenType.UciBatch;
        public string Extension => ".uci";

        public WgUciConfigConverter(WgConfigGeneratorInfo info)
        {
            _info = info ?? new WgConfigGeneratorInfo();
        }

        public void SaveFile(string filePath, WgHostConfig config)
        {
            File.WriteAllText(filePath, Save(config));
        }

        public string Save(WgHostConfig config)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"#INFO: MeshGenDate: {config.MeshGenDate}");
            sb.AppendLine($"#INFO: MeshGenVersion: {config.MeshGenVersion}");
            sb.AppendLine($"#INFO: ConvDate: {DateTime.Now}");
            sb.AppendLine($"#INFO: ConvVersion: {Version}");
            sb.AppendLine($"#INFO: Github: {_info.Github}");
            if (config.Name != null)
                sb.AppendLine("#NAME: " + config.Name);
            if (config.Comment != null)
                sb.AppendLine("#COMMENT: " + config.Comment);
            if (config.InterfaceName != null)
                sb.AppendLine("#IF: " + config.InterfaceName);

            sb.AppendLine($"set network.{config.InterfaceName}=interface");
            sb.AppendLine($"set network.{config.InterfaceName}.proto='wireguard'");
            sb.AppendLine($"set network.{config.InterfaceName}.private_key={config.Interface.PrivateKey}");
            if (config.Interface.ListenPort is not null) 
                sb.AppendLine($"set network.{config.InterfaceName}.listen_port={config.Interface.ListenPort}");
            foreach (var ipNetwork in config.Interface.Address)
                sb.AppendLine($"add_list network.{config.InterfaceName}.addresses='{ipNetwork}'");
            sb.AppendLine();

            foreach (var peer in config.Peers)
            {
                if (config.Name != null)
                    sb.AppendLine("#NAME: " + config.Name);
                if (config.Comment != null)
                    sb.AppendLine("#COMMENT: " + config.Comment);
                sb.AppendLine($"add network  wireguard_{config.InterfaceName}");
                sb.AppendLine($"set network.@wireguard_{config.InterfaceName}[-1].description='{peer.Name}'");
                sb.AppendLine($"set network.@wireguard_{config.InterfaceName}[-1].public_key='{peer.PublicKey}'");
                if (peer.PreSharedKey != null)
                    sb.AppendLine($"set network.@wireguard_{config.InterfaceName}[-1].preshared_key='{peer.PreSharedKey}'");
                sb.AppendLine($"set network.@wireguard_{config.InterfaceName}[-1].endpoint_host='{peer.Endpoint}'");
                if (peer.EndpointPort is not null) 
                    sb.AppendLine($"set network.@wireguard_{config.InterfaceName}[-1].endpoint_port='{peer.EndpointPort}'");
                if (config.Interface.Table != WgHostInterfaceConfig.TableMode.Off) 
                    sb.AppendLine($"set network.@wireguard_{config.InterfaceName}[-1].route_allowed_ips='1'");
                if (peer.PersistentKeepalive != null)
                    sb.AppendLine($"set network.@wireguard_{config.InterfaceName}[-1].persistent_keepalive='{peer.PersistentKeepalive}'");
                foreach (var allowedIp in peer.AllowedIps)
                    sb.AppendLine($"add_list network.@wireguard_{config.InterfaceName}[-1].allowed_ips='{allowedIp}'");
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}