using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ArkProjects.Wireguard.Mesh.Configs;
using ArkProjects.Wireguard.Mesh.Misc;
using Microsoft.Extensions.Logging;

namespace ArkProjects.Wireguard.Mesh
{
    public class WgMeshBuilder
    {
        private static readonly Version BuilderVersion = Assembly.GetAssembly(typeof(WgMeshBuilder))?.GetName().Version;

        private readonly ILogger<WgMeshBuilder> _logger;

        public WgMeshBuilder(ILogger<WgMeshBuilder> logger)
        {
            _logger = logger;
        }

        public IReadOnlyCollection<WgHostConfig> BuildConfigs(WgMeshConfig config)
        {
            return config.Nodes.SelectMany(x => BuildConfigs(x, config.Nodes, config.NodesPreSharedKeys)).ToArray();
        }

        public IReadOnlyList<WgHostConfig> BuildConfigs(WgMeshNodeConfig node, IReadOnlyCollection<WgMeshNodeConfig> all, IReadOnlyList<WgMeshNodesPreSharedKey> preSharedKeyPairs)
        {
            _logger.LogInformation("Build {name} config", node.Name);
            var configs = new List<WgHostConfig>();

            //main
            _logger.LogInformation("Build main config: {name}", node.Name);
            configs.Add(BuildMainConfig(node, all, preSharedKeyPairs));

            //over gate
            if (node.MutatedConfigs != null)
            {
                foreach (var mutatedConfig in node.MutatedConfigs)
                {
                    _logger.LogInformation("Build mutated config: {name}", mutatedConfig.Name);
                    if (mutatedConfig.Overrides == null)
                        continue;
                    configs.Add(BuildMutatedConfig(mutatedConfig, node, all, preSharedKeyPairs));
                }
            }

            _logger.LogInformation("Build {count} configs from {name}", configs.Count, node.Name);
            return configs;
        }

        private static WgHostConfig BuildMutatedConfig(WgMeshNodeConfig.MutatedConfig mutatedConfig, WgMeshNodeConfig thisNode, IReadOnlyCollection<WgMeshNodeConfig> allNodes, IReadOnlyList<WgMeshNodesPreSharedKey> preSharedKeyPairs)
        {
            var hostConfig = new WgHostConfig()
            {
                MeshGenDate = DateTime.Now,
                MeshGenVersion = BuilderVersion,
                Name = mutatedConfig.Name,
                Comment = thisNode.Comment,
                Interface = new WgHostInterfaceConfig(thisNode),
                InterfaceName = thisNode.InterfaceName,
            };

            var peers = new List<WgHostPeerConfig>();
            if (mutatedConfig.Overrides != null)
            {
                foreach (var configOverride in mutatedConfig.Overrides)
                {
                    var peerNodeConfig = allNodes.First(x => x.Name == configOverride.Node);
                    var preSharedKey = preSharedKeyPairs?.FirstOrDefault(x => x.Nodes.Contains(thisNode.Name) && x.Nodes.Contains(peerNodeConfig.Name))?.PreSharedKey;
                    var peer = new WgHostPeerConfig(peerNodeConfig, configOverride.AllowedIps, preSharedKey);
                    if (!configOverride.InheritKeepalive)
                        peer.PersistentKeepalive = null;
                    peers.Add(peer);
                }
            }

            foreach (var peerNodeConfig in allNodes.Where(x => peers.All(y => y.Name != x.Name) && IsPeerOf(thisNode, x)))
            {
                var allowedIps = peerNodeConfig.AdditionalAllowedIps?.ToList() ?? new List<IPNetwork>();
                if (peerNodeConfig.AddSelfAddresses)
                    allowedIps.AddRange(peerNodeConfig.BindAddresses);

                var preSharedKey = preSharedKeyPairs?.FirstOrDefault(x => x.Nodes.Contains(thisNode.Name) && x.Nodes.Contains(peerNodeConfig.Name))?.PreSharedKey;
                peers.Add(new WgHostPeerConfig(peerNodeConfig, allowedIps, preSharedKey));
            }

            hostConfig.Peers = peers;
            return hostConfig;
        }

        private static WgHostConfig BuildMainConfig(WgMeshNodeConfig thisNode, IReadOnlyCollection<WgMeshNodeConfig> allNodes, IReadOnlyList<WgMeshNodesPreSharedKey> preSharedKeyPairs)
        {
            var hostConfig = new WgHostConfig()
            {
                MeshGenDate = DateTime.Now,
                MeshGenVersion = BuilderVersion,
                Name = thisNode.Name,
                Comment = thisNode.Comment,
                Interface = new WgHostInterfaceConfig(thisNode),
                InterfaceName = thisNode.InterfaceName,
            };

            var peers = new List<WgHostPeerConfig>();
            foreach (var peerNodeConfig in allNodes.Where(x => IsPeerOf(thisNode, x)))
            {
                var allowedIps = peerNodeConfig.AdditionalAllowedIps?.ToList() ?? new List<IPNetwork>();
                if (peerNodeConfig.AddSelfAddresses)
                    allowedIps.AddRange(peerNodeConfig.BindAddresses);

                var preSharedKey = preSharedKeyPairs?.FirstOrDefault(x => x.Nodes.Contains(thisNode.Name) && x.Nodes.Contains(peerNodeConfig.Name))?.PreSharedKey;
                peers.Add(new WgHostPeerConfig(peerNodeConfig, allowedIps, preSharedKey));
            }

            hostConfig.Peers = peers;
            return hostConfig;
        }

        private static bool IsUsedInOverrides(WgMeshNodeConfig node, WgMeshNodeConfig possiblePeer)
        {
            var peerUsedInOverrides = node.MutatedConfigs?
                .Where(x => x.Overrides != null)
                .SelectMany(x => x.Overrides.Select(y => y.Node))
                .Any(x => x == possiblePeer.Name) == true;

            return peerUsedInOverrides;
        }

        private static bool IsPeerOf(WgMeshNodeConfig thisNode, WgMeshNodeConfig possiblePeer)
        {
            if (thisNode == null)
                throw new ArgumentNullException(nameof(thisNode));
            if (possiblePeer == null)
                throw new ArgumentNullException(nameof(possiblePeer));

            if (possiblePeer == thisNode)
                return false;

            if (IsUsedInOverrides(possiblePeer, thisNode))
                return true;

            return thisNode.TargetsMode switch
            {
                WgMeshNodeTargetsMode.Exclude => thisNode.Targets == null || thisNode.Targets.All(x => x != possiblePeer.Name),
                WgMeshNodeTargetsMode.Include => thisNode.Targets != null && thisNode.Targets.Any(x => x == possiblePeer.Name),
                _ => throw new NotSupportedException($"Mode {thisNode.TargetsMode} not supported")
            };
        }
    }
}