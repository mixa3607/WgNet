using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using ArkProjects.Wireguard.Mesh.Misc;

namespace ArkProjects.Wireguard.Mesh.Configs
{
    public class WgHostInterfaceConfig
    {
        public WgHostInterfaceConfig()
        {
        }

        public WgHostInterfaceConfig(WgMeshNodeConfig node)
        {
            SaveConfig = node.SaveConfig;
            PreDown = node.PreDown;
            PostUp = node.PostUp;
            PostDown = node.PostDown;
            Mtu = node.Mtu;
            Table = node.Table;
            PreUp = node.PreUp;
            ListenPort = node.ListenPort;
            Address = node.BindAddresses ?? Array.Empty<IPNetwork>();
            Dns = node.Dns;
            PrivateKey = node.PrivateKey;
        }

        public string PrivateKey { get; set; }
        public IReadOnlyCollection<IPNetwork> Address { get; set; }
        public ushort? ListenPort { get; set; }
        public IReadOnlyCollection<IPAddress> Dns { get; set; } = Array.Empty<IPAddress>();

        /// <summary>
        /// MTU size. null for auto
        /// </summary>
        public ushort? Mtu { get; set; }

        /// <summary>
        /// Table for routes. See <see cref="TableMode"/> for more details or provide custom name
        /// </summary>
        [DefaultValue(TableMode.Auto)]
        public string Table { get; set; } = TableMode.Auto;

        /// <summary>Pre up script</summary>
        /// <remarks>On export will be just concat without any delimiters</remarks>
        public string[] PreUp { get; set; }

        /// <summary>Pre down script</summary>
        /// <remarks>On export will be just concat without any delimiters</remarks>
        public string[] PreDown { get; set; }

        /// <summary>Post up script</summary>
        /// <remarks>On export will be just concat without any delimiters</remarks>
        public string[] PostUp { get; set; }

        /// <summary>Post down script</summary>
        /// <remarks>On export will be just concat without any delimiters</remarks>
        public string[] PostDown { get; set; }

        public bool SaveConfig { get; set; }

        public static class TableMode
        {
            /// <summary>
            /// Auto select route table
            /// </summary>
            public const string Auto = "auto";

            /// <summary>
            /// Don't add routes
            /// </summary>
            public const string Off = "off";
        }
    }
}