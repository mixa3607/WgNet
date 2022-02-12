using System;
using System.Collections.Generic;
using System.Linq;
using ArkProjects.Wireguard.Mesh.Misc;
using ArkProjects.Wireguard.Tools;
using static ArkProjects.Wireguard.Mesh.Misc.ConfigValidationMessageLevel;

namespace ArkProjects.Wireguard.Mesh.Configs
{
    public class WgConfigValidator : IWgMeshConfigValidator, IWgHostConfigValidator
    {
        public IReadOnlyList<ConfigValidationMessage> Validate(WgMeshConfig mesh)
        {
            var errs = new ValidationMessageCollection();
            var p = new ConfigPath<WgMeshConfig>();
            var p1 = p.Name(x => x.Nodes);
            var p2 = p.Name(x => x.NodesPreSharedKeys);

            var nodeNames = mesh.Nodes?
                .Select(x => x.Name)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray() ?? Array.Empty<string>();
            var allNames = mesh.Nodes?
                .Where(x => x?.MutatedConfigs != null)
                .SelectMany(x => x.MutatedConfigs.Select(y => y.Name))
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Concat(nodeNames) ?? nodeNames;

            //pre-shared
            if (mesh.NodesPreSharedKeys == null || mesh.NodesPreSharedKeys.Count == 0)
            {
                errs.Add(p2, "Recommend use pre-shared keys", Note);
            }
            else
            {
                //iterate all pairs
                var nodePairs = new List<(string a, string b)>();
                for (var i = 1; i < nodeNames.Length; i++)
                for (var j = i; j < nodeNames.Length; j++)
                    nodePairs.Add((nodeNames[i - 1], nodeNames[j]));

                foreach (var (a, b) in nodePairs)
                {
                    var matchedInPairs = mesh.NodesPreSharedKeys.Count(x => x.Nodes.Contains(a) && x.Nodes.Contains(b));
                    if (matchedInPairs == 0)
                        errs.Add(p2, $"Not found pre-shared key for pair {a}<=>{b}", Note);
                    else if (matchedInPairs != 1)
                        errs.Add(p2, $"Found more than 1 pre-shared keys for pair {a}<=>{b}", Error);
                }

                //check exist
                for (int i = 0; i < mesh.NodesPreSharedKeys.Count; i++)
                {
                    var p2I = p2.Index(i);
                    var p2I1 = p2I.Name(x => x.PreSharedKey);
                    var p2I2 = p2I.Name(x => x.Nodes);
                    var n = mesh.NodesPreSharedKeys[i];

                    if (n == null)
                    {
                        errs.Add(p2I, "Must be not null", Error);
                    }
                    else
                    {
                        //key
                        if (n.PreSharedKey == null)
                            errs.Add(p2I1, "Fix or remove section", Note);
                        else if (!WgTools.PreSharedKeyValid(n.PreSharedKey))
                            errs.Add(p2I1, "Bad key", Error);

                        //nodes
                        if (n.Nodes == null || n.Nodes.Count <= 1)
                        {
                            errs.Add(p2I2, "Must be not empty and have more than one node", Error);
                        }
                        else
                        {
                            //node
                            for (int j = 0; j < n.Nodes.Count; j++)
                            {
                                var p2I2I = p2I2.Index(j);
                                if (string.IsNullOrWhiteSpace(n.Nodes[j]))
                                    errs.Add(p2I2I, "Must be not empty", Error);
                                else if (nodeNames.All(x => x != n.Nodes[j]))
                                    errs.Add(p2I2I, "Node not found", Error);
                            }
                        }
                    }
                }
            }

            //nodes
            if (mesh.Nodes == null || mesh.Nodes.Count == 0)
            {
                errs.Add(p1, "Must be not empty", Error);
            }
            else
            {
                //node
                for (var nIdx = 0; nIdx < mesh.Nodes.Count; nIdx++)
                {
                    var node = mesh.Nodes[nIdx];
                    var p1I = p1.Index(nIdx);
                    var p1I1 = p1I.Name(x => x.Targets);
                    var p1I2 = p1I.Name(x => x.MutatedConfigs);

                    if (node == null)
                    {
                        errs.Add(p1I, "Must be not null", Error);
                    }
                    else
                    {
                        //name
                        if (string.IsNullOrWhiteSpace(node.Name))
                            errs.Add(p1I.Name(x => x.Name), "Must be not empty", Warning);
                        else if (allNames.Count(x => x == node.Name) != 1)
                            errs.Add(p1I.Name(x => x.Name), "Must be uniq", Error);

                        //keys
                        if (!WgTools.PrivateKeyValid(node.PrivateKey))
                            errs.Add(p1I.Name(x => x.PrivateKey), "Bad key", Error);
                        if (!WgTools.PublicKeyValid(node.PublicKey))
                            errs.Add(p1I.Name(x => x.PublicKey), "Bad key", Error);
                        if (!WgTools.KeyPairValid(node.PrivateKey, node.PublicKey))
                            errs.Add(p1I, "Bad key pair", Error);

                        //port
                        if (node.ListenPort == 0)
                            errs.Add(p1I.Name(x => x.ListenPort), "Can't be 0. Use null for disable", Error);

                        //mtu
                        if (node.Mtu == 0)
                            errs.Add(p1I.Name(x => x.Mtu), "Can't be 0. Use null for disable", Error);

                        //bind
                        if (node.BindAddresses == null || node.BindAddresses.Count == 0)
                            errs.Add(p1I.Name(x => x.BindAddresses), "1+ address required", Error);

                        //if
                        if (string.IsNullOrWhiteSpace(node.InterfaceName))
                            errs.Add(p1I.Name(x => x.InterfaceName), "Must be not empty", Warning);

                        //targets
                        if (node.Targets != null)
                        {
                            for (int i = 0; i < node.Targets.Count; i++)
                            {
                                if (!nodeNames.Contains(node.Targets[i]))
                                    errs.Add(p1I1.Index(i), "Node not found", Error);
                            }
                        }

                        //mutations
                        if (node.MutatedConfigs != null)
                        {
                            for (var i = 0; i < node.MutatedConfigs.Count; i++)
                            {
                                var p1I2I = p1I2.Index(i);
                                var p1I2I1 = p1I2I.Name(x => x.Overrides);
                                var additionalConfig = node.MutatedConfigs[i];

                                //name
                                if (string.IsNullOrWhiteSpace(additionalConfig.Name))
                                    errs.Add(p1I2I.Name(x => x.Name), "Must be not empty", Warning);
                                else if (allNames.Count(x => x == additionalConfig.Name) != 1)
                                    errs.Add(p1I2I.Name(x => x.Name), "Must be uniq", Error);

                                //overrides
                                if (additionalConfig.Overrides is null or { Count: 0 })
                                {
                                    errs.Add(p1I2I1, "can't be empty", Error);
                                }
                                else
                                {
                                    for (int j = 0; j < additionalConfig.Overrides.Count; j++)
                                    {
                                        var p1I2I1I = p1I2I1.Index(j);
                                        var overrideConf = additionalConfig.Overrides[j];

                                        //name
                                        if (string.IsNullOrWhiteSpace(overrideConf.Node))
                                            errs.Add(p1I2I1I.Name(x => x.Node), "Must be not empty", Warning);
                                        else if (nodeNames.Count(x => x == overrideConf.Node) != 1)
                                            errs.Add(p1I2I1I.Name(x => x.Node), "Not found", Error);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return errs;
        }

        //TODO: rewrite
        public IReadOnlyList<ConfigValidationMessage> Validate(WgHostConfig host)
        {
            var errs = new List<ConfigValidationMessage>();
            var p = new ConfigPath();

            if (string.IsNullOrWhiteSpace(host.Name))
                errs.Add(new(p.Name(nameof(host.Name)), "is null or empty", Error));
            if (string.IsNullOrWhiteSpace(host.InterfaceName))
                errs.Add(new(p.Name(nameof(host.InterfaceName)), "is null or empty", Error));

            var pIf = p.Name(nameof(host.Interface));
            if (host.Interface == null)
                errs.Add(new(pIf, "is null", Error));
            else
            {
                var ifCfg = host.Interface;
                if (string.IsNullOrWhiteSpace(ifCfg.PrivateKey))
                    errs.Add(new(p.Name(nameof(ifCfg.PrivateKey)), "is null or empty", Error));

                if (ifCfg.ListenPort == 0)
                    errs.Add(new(p.Name(nameof(ifCfg.ListenPort)), "can't be 0. Use null for disable", Error));

                if (!(ifCfg.Address?.Count > 0))
                    errs.Add(new(p.Name(nameof(ifCfg.Address)), "1 more ip required", Error));
            }

            var pP = p.Name(nameof(host.Peers));
            if (host.Peers is null || host.Peers.Count == 0)
            {
                errs.Add(new(pP, "usually 1 more", Note));
            }
            else
            {
                for (int i = 0; i < host.Peers.Count; i++)
                {
                    var pPIdx = pP.Index(i);
                    var peer = host.Peers[i];

                    if (string.IsNullOrWhiteSpace(peer.PublicKey))
                        errs.Add(new(pPIdx.Name(nameof(peer.PublicKey)), "is null or empty", Error));

                    if (string.IsNullOrWhiteSpace(peer.PreSharedKey))
                        errs.Add(new(pPIdx.Name(nameof(peer.PreSharedKey)), "is null or empty", Note));

                    if (string.IsNullOrWhiteSpace(peer.Endpoint))
                        errs.Add(new(pPIdx.Name(nameof(peer.Endpoint)), "is null or empty", Note));

                    if (peer.AllowedIps == null || peer.AllowedIps.Count == 0)
                        errs.Add(new(pPIdx.Name(nameof(peer.AllowedIps)), "1 more ip", Note));
                }
            }

            return errs;
        }
    }
}