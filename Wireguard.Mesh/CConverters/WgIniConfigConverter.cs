using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using ArkProjects.Wireguard.Mesh.Configs;
using IniParser;
using IniParser.Model;

namespace ArkProjects.Wireguard.Mesh.CConverters
{
    public class WgIniConfigConverter : IWgConfigConverter
    {
        private static readonly Version Version = Assembly.GetAssembly(typeof(WgMeshBuilder))?.GetName().Version;

        private readonly WgConfigGeneratorInfo _info;

        public WgConfigGenType GenType => WgConfigGenType.Ini;
        public string Extension => ".conf";

        public WgIniConfigConverter(WgConfigGeneratorInfo info)
        {
            _info = info ?? new WgConfigGeneratorInfo();
        }

        public void SaveFile(string filePath, WgHostConfig config)
        {
            File.WriteAllText(filePath, Save(config));
        }

        public string Save(WgHostConfig config)
        {
            var ini = ConfigToIni(config);
            var str = IniToString(ini);
            return str;
        }

        private string IniToString(IEnumerable<IniData> blocks)
        {
            var parser = new StreamIniDataParser();
            var memStream = new MemoryStream();
            var writer = new StreamWriter(memStream);
            foreach (var block in blocks)
            {
                parser.WriteData(writer, block);
                writer.WriteLine();
            }

            writer.Flush();
            memStream.Position = 0;
            var str = Encoding.UTF8.GetString(memStream.ToArray());

            return str;
        }

        private IReadOnlyList<IniData> ConfigToIni(WgHostConfig config)
        {
            var blocks = new List<IniData>();
            {
                var ini = new IniData { Configuration = { CommentString = "#" } };

                var section = InterfaceToIni(config.Interface);
                section.Comments.Add($"INFO: MeshGenDate: {config.MeshGenDate}");
                section.Comments.Add($"INFO: MeshGenVersion: {config.MeshGenVersion}");
                section.Comments.Add($"INFO: ConvDate: {DateTime.Now}");
                section.Comments.Add($"INFO: ConvVersion: {Version}");
                section.Comments.Add($"INFO: Github: {_info.Github}");
                if (config.Name != null)
                    section.Comments.Add("NAME: " + config.Name);
                if (config.Comment != null)
                    section.Comments.Add("COMMENT: " + config.Comment);
                if (config.InterfaceName != null)
                    section.Comments.Add("IF: " + config.InterfaceName);

                ini.Sections.Add(section);
                blocks.Add(ini);
            }

            foreach (var configPeer in config.Peers)
            {
                var ini = new IniData { Configuration = { CommentString = "#" } };
                var section = PeerToIni(configPeer);
                ini.Sections.Add(section);
                blocks.Add(ini);
            }

            return blocks;
        }

        private SectionData InterfaceToIni(WgHostInterfaceConfig config)
        {
            var section = new SectionData("Interface");

            var keys = section.Keys;
            keys["PrivateKey"] = config.PrivateKey;
            keys["Address"] = string.Join(", ", config.Address.Select(x => x.ToString()));
            if (config.ListenPort is not null)
                keys["ListenPort"] = config.ListenPort.ToString();
            if (config.Dns?.Count > 0)
                keys["DNS"] = string.Join(", ", config.Dns);
            if (config.Mtu != null)
                keys["MTU"] = config.Mtu.ToString();
            if (config.Table != WgHostInterfaceConfig.TableMode.Auto)
                keys["Table"] = config.Table;

            if (config.PreUp?.Length > 0)
                keys["PreUp"] = string.Join("", config.PreUp);
            if (config.PostUp?.Length > 0)
                keys["PostUp"] = string.Join("", config.PostUp);
            if (config.PreDown?.Length > 0)
                keys["PreDown"] = string.Join("", config.PreDown);
            if (config.PostDown?.Length > 0)
                keys["PostDown"] = string.Join("", config.PostDown);

            if (config.SaveConfig)
                keys["SaveConfig"] = config.SaveConfig.ToString();

            return section;
        }

        private SectionData PeerToIni(WgHostPeerConfig config)
        {
            var section = new SectionData("Peer");
            if (config.Name != null)
                section.Comments.Add("NAME: " + config.Name);
            if (config.Comment != null)
                section.Comments.Add("COMMENT: " + config.Comment);

            var keys = section.Keys;

            keys["PublicKey"] = config.PublicKey;
            if (config.PreSharedKey != null)
                keys["PresharedKey"] = config.PreSharedKey;

            keys["AllowedIPs"] = string.Join(", ", config.AllowedIps.Select(x => x.ToString()));
            if (config.Endpoint != null)
                keys["Endpoint"] = config.Endpoint + (config.EndpointPort is null ? null : ":" + config.EndpointPort);

            if (config.PersistentKeepalive != null)
                keys["PersistentKeepalive"] = config.PersistentKeepalive?.ToString();

            return section;
        }
    }
}