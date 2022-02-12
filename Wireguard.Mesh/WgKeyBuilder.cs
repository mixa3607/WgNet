using System;
using System.Collections.Generic;
using System.Linq;
using ArkProjects.Wireguard.Mesh.Configs;
using ArkProjects.Wireguard.Tools;
using Microsoft.Extensions.Logging;

namespace ArkProjects.Wireguard.Mesh;

public class WgKeyBuilder
{
    private readonly ILogger<WgKeyBuilder> _logger;

    public WgKeyBuilder(ILogger<WgKeyBuilder> logger)
    {
        _logger = logger;
    }

    public void FixPreSharedKeys(WgMeshConfig mesh)
    {
        _logger.LogInformation("Fix pre-shared keys");
        var preSharedPairs = new List<WgMeshNodesPreSharedKey>();

        var nodeNames = mesh.Nodes
            .Select(x => x.Name)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToArray();

        if (mesh.NodesPreSharedKeys != null)
        {
            foreach (var existedPair in mesh.NodesPreSharedKeys)
            {
                if (existedPair.PreSharedKey != null && !WgTools.PreSharedKeyValid(existedPair.PreSharedKey))
                {
                    _logger.LogWarning("Drop key {key}. Bad key", existedPair.PreSharedKey);
                    existedPair.PreSharedKey = null;
                }

                if (existedPair.Nodes == null || existedPair.Nodes.Count <= 1)
                {
                    _logger.LogWarning("Drop {p}. Empty or 1 nodes in list", existedPair);
                }
                else
                {
                    var existedNodes = existedPair.Nodes.Where(x => nodeNames.Contains(x)).ToArray();
                    if (existedNodes.Length != existedPair.Nodes.Count)
                    {
                        _logger.LogWarning("Change {e} to {f}", existedPair.Nodes, existedNodes);
                        existedPair.Nodes = existedNodes;
                    }

                    preSharedPairs.Add(existedPair);
                }
            }
        }

        mesh.NodesPreSharedKeys = preSharedPairs;
    }

    public void GeneratePreSharedKeys(WgMeshConfig mesh, bool reGenerate)
    {
        var nodeNames = mesh.Nodes
            .Select(x => x.Name)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToArray();

        _logger.LogInformation("Generate pre-shared keys");
        var nodePairs = new List<(string a, string b)>();
        for (var i = 1; i < nodeNames.Length; i++)
        for (var j = i; j < nodeNames.Length; j++)
            nodePairs.Add((nodeNames[i - 1], nodeNames[j]));

        var preSharedPairs = mesh.NodesPreSharedKeys?.ToList() ?? new List<WgMeshNodesPreSharedKey>();

        //add missed
        foreach (var (a, b) in nodePairs)
        {
            var pair = preSharedPairs.FirstOrDefault(x => x.Nodes.Contains(a) && x.Nodes.Contains(b));
            if (pair != null)
                continue;
            _logger.LogInformation("Create node pair {a}<=>{b}", a, b);
            preSharedPairs.Add(new WgMeshNodesPreSharedKey() { Nodes = new[] { a, b } });
        }

        //gen keys
        foreach (var preSharedPair in preSharedPairs.Where(x => x.PreSharedKey == null || reGenerate))
        {
            _logger.LogInformation("Generate key for {h}", preSharedPair.Nodes);
            preSharedPair.PreSharedKey = Convert.ToBase64String(WgTools.GenPreSharedKey());
        }

        mesh.NodesPreSharedKeys = preSharedPairs;
    }

    public void FixKeyPairs(WgMeshConfig mesh)
    {
        foreach (var node in mesh.Nodes)
        {
            _logger.LogInformation("Validating node keys: \"{node}\"", node.Name);

            if (node.PrivateKey == null)
            {
                node.PrivateKey = null;
                node.PublicKey = null;
            }
            else if (!WgTools.PrivateKeyValid(node.PrivateKey))
            {
                _logger.LogInformation("Reset public and private keys");
                node.PrivateKey = null;
                node.PublicKey = null;
            }
            else if (!WgTools.PublicKeyValid(node.PublicKey))
            {
                _logger.LogInformation("Reset public key");
                node.PublicKey = null;
            }
            else if (!WgTools.KeyPairValid(node.PrivateKey, node.PublicKey))
            {
                _logger.LogInformation("Reset public key");
                node.PublicKey = null;
            }
        }
    }

    public void GenerateKeyPair(WgMeshConfig mesh, bool reGenerate)
    {
        foreach (var node in mesh.Nodes)
        {
            _logger.LogInformation("Generating node keys: \"{node}\"", node.Name);

            if (reGenerate || node.PrivateKey == null)
            {
                _logger.LogInformation("Generate private and public keys");
                var privKey = WgTools.GenPrivateKey();
                var pubKey = WgTools.GenPublicKey(privKey);
                node.PrivateKey = Convert.ToBase64String(privKey);
                node.PublicKey = Convert.ToBase64String(pubKey);
            }
            else if (node.PublicKey == null)
            {
                _logger.LogInformation("Generate public key");
                try
                {
                    var privKey = Convert.FromBase64String(node.PrivateKey);
                    var pubKey = WgTools.GenPublicKey(privKey);
                    node.PublicKey = Convert.ToBase64String(pubKey);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Can't gen public key from private");
                }
            }
            else
            {
                _logger.LogInformation("No key changes: \"{node}\"", node.Name);
            }
        }
    }
}