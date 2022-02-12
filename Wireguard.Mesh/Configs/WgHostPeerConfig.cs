using System;
using System.Collections.Generic;
using ArkProjects.Wireguard.Mesh.Misc;

namespace ArkProjects.Wireguard.Mesh.Configs
{
    public class WgHostPeerConfig
    {
        public WgHostPeerConfig()
        {
        }

        public WgHostPeerConfig(WgMeshNodeConfig node, IReadOnlyCollection<IPNetwork> allowedIps, string preSharedKey = null)
        {
            Endpoint = node.Endpoint;
            EndpointPort = node.ListenPort;
            PersistentKeepalive = node.PersistentKeepalive;
            PublicKey = node.PublicKey;
            PreSharedKey = preSharedKey;
            AllowedIps = allowedIps;
            Name = node.Name;
            Comment = node.Comment;
        }

        public string Name { get; set; }
        public string Comment { get; set; }

        public IReadOnlyCollection<IPNetwork> AllowedIps { get; set; } = Array.Empty<IPNetwork>();
        public string PublicKey { get; set; }
        public string PreSharedKey { get; set; }
        public string Endpoint { get; set; }
        public ushort? EndpointPort { get; set; }
        public ushort? PersistentKeepalive { get; set; }

        public override string ToString()
        {
            return $"{Name}({Endpoint}:{EndpointPort ?? -1})";
        }
    }
}