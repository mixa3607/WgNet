using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ArkProjects.Wireguard.Cli.Options;
using ArkProjects.Wireguard.Deploy;
using ArkProjects.Wireguard.Mesh;
using ArkProjects.Wireguard.Mesh.CConverters;
using ArkProjects.Wireguard.Mesh.Configs;
using ArkProjects.Wireguard.Mesh.Misc;
using ConsoleTables;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PowerArgs;

namespace ArkProjects.Wireguard.Cli
{
    [ArgExceptionBehavior(ArgExceptionPolicy.StandardExceptionHandling)]
    public class WgCli : WgCliLogOptions
    {
        private readonly ILogger<WgCli> _logger;
        private readonly IServiceProvider _serviceProvider;

        private IReadOnlyList<IWgHostConfigValidator> _hostValidators;
        private IReadOnlyList<IWgMeshConfigValidator> _meshValidators;

        [HelpHook, ArgShortcut("-?"), ArgShortcut("-h"), ArgShortcut("--help"), ArgDescription("Shows this help")]
        public bool Help { get; set; }

        [VersionHook(TypeInTargetAssembly = typeof(WgCli)), ArgShortcut("-v"), ArgShortcut("--version"), ArgDescription("Show version info")]
        public bool Version { get; set; }

        public WgCli(IServiceProvider serviceProvider, ILogger<WgCli> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        [ArgActionMethod]
        public void ValidateMesh(WgCliValidateMeshOptions opts)
        {
            var mesh = WgMeshConfigManager.LoadFile(opts.Config);
            _logger.LogInformation("Validating mesh");

            var messages = ValidateMesh(mesh);
            if (messages.Count != 0)
            {
                var table = ConsoleTable.From(messages).Configure(x => { x.EnableCount = false; }).ToMinimalString();
                _logger.LogInformation("Found {count} problems in mesh\n{table}", messages.Count, table);
            }
            else
            {
                _logger.LogInformation("No problems found");
            }
        }

        [ArgActionMethod]
        public void ValidateConfigs(WgCliValidateConfigsOptions opts)
        {
            var configs = LoadConfigs(opts.InDir, true);
            var targetConfigs = configs.Where(x => opts.Names?.Contains(x.Name) != false).ToArray();

            foreach (var host in targetConfigs)
            {
                var messages = ValidateConfig(host);
                if (messages.Count != 0)
                {
                    var table = ConsoleTable.From(messages).Configure(x => { x.EnableCount = false; }).ToMinimalString();
                    _logger.LogInformation("Found {count} problems in host {name}\n{table}", messages.Count, host.Name, table);
                }
                else
                {
                    _logger.LogInformation("No problems found in {name}", host.Name);
                }
            }
        }

        [ArgActionMethod]
        public void GenerateKeys(WgCliGenerateKeysOptions opts)
        {
            var mesh = WgMeshConfigManager.LoadFile(opts.Config);
            var builder = _serviceProvider.GetRequiredService<WgKeyBuilder>();

            if (opts.FixInvalid)
            {
                builder.FixKeyPairs(mesh);
                builder.FixPreSharedKeys(mesh);
            }

            builder.GenerateKeyPair(mesh, opts.ReGenerate);

            if (opts.WithPreShared)
                builder.GeneratePreSharedKeys(mesh, opts.ReGenerate);

            var outFile = opts.OutConfig ?? opts.Config;
            _logger.LogInformation("Save to {out}", outFile);
            WgMeshConfigManager.SaveAsFile(mesh, outFile, true);
        }

        [ArgActionMethod]
        public void BuildMesh(WgCliBuildMeshOptions opts)
        {
            var mesh = WgMeshConfigManager.LoadFile(opts.Config);
            var errs = ValidateMesh(mesh).Count(x => x.Level >= ConfigValidationMessageLevel.Warning);
            if (errs != 0)
            {
                _logger.LogWarning("Found {count} errors in mesh config", errs);
                if (!opts.IgnoreConfigErrors)
                    throw new Exception("Validate/fix/suppress errors in mesh config");
            }

            var builder = _serviceProvider.GetRequiredService<WgMeshBuilder>();
            var configs = builder.BuildConfigs(mesh);
            if (!Directory.Exists(opts.OutDir))
            {
                _logger.LogWarning("Directory {dir} not exist. Create", opts.OutDir);
                Directory.CreateDirectory(opts.OutDir);
            }

            foreach (var wgConfig in configs)
            {
                var saveTo = Path.Combine(opts.OutDir, wgConfig.Name + ".json");
                try
                {
                    WgHostConfigManager.SaveAsFile(wgConfig, saveTo);
                    _logger.LogInformation("Save {name} to {file}", wgConfig.Name, saveTo);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Skip config: {file}", saveTo);
                }
            }
        }

        [ArgActionMethod]
        public void ExportConfig(WgCliExportConfigsOptions opts)
        {
            var configs = LoadConfigs(opts.InDir, false);
            var targetConfigs = configs.Where(x => opts.Names?.Contains(x.Name) != false).ToArray();

            foreach (var hostConfig in targetConfigs)
            {
                var errs = ValidateConfig(hostConfig).Count(x => x.Level >= ConfigValidationMessageLevel.Warning);
                if (errs != 0)
                {
                    _logger.LogWarning("Found {count} errors in mesh config", errs);
                    if (!opts.IgnoreConfigErrors)
                        throw new Exception("Validate/fix/suppress errors in mesh config");
                }
            }

            if (!Directory.Exists(opts.OutDir))
            {
                _logger.LogWarning("Directory {dir} not exist. Create", opts.OutDir);
                Directory.CreateDirectory(opts.OutDir);
            }

            var converters = _serviceProvider.GetRequiredService<IEnumerable<IWgConfigConverter>>().ToArray();
            var converter = converters.FirstOrDefault(x => x.GenType == opts.Type);
            if (converter == null)
                throw new NotSupportedException($"Exporter {opts.Type} not supported");

            foreach (var targetConfig in targetConfigs)
            {
                var saveTo = Path.Combine(opts.OutDir, targetConfig.Name + converter.Extension);
                try
                {
                    _logger.LogInformation("Export {name} to {file}", targetConfig.Name, saveTo);
                    converter.SaveFile(saveTo, targetConfig);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Skip config: {file}", saveTo);
                }
            }
        }

        [ArgActionMethod]
        public void DeployConfig(WgCliDeployConfigsOptions opts)
        {
            var deployMaster = _serviceProvider.GetRequiredService<WgDeployMaster>();

            if (opts.ListMethods)
            {
                var infos = deployMaster.GetAllMethodsInfo();
                var table = ConsoleTable.From(infos).Configure(x => { x.EnableCount = false; }).ToMinimalString();
                _logger.LogInformation("Found {count} deploy methods\n{table}", infos.Count, table);
                return;
            }

            var deploy = WgDeployConfig.LoadFile(opts.Config);
            var targets = deploy.Targets.Where(x => opts.Names?.Contains(x.Name) != false).ToArray();
            _logger.LogInformation("No validations on deploy");

            foreach (var target in targets)
            {
                deployMaster.Deploy(target, opts.TargetStages);
            }
        }

        private IReadOnlyList<WgHostConfig> LoadConfigs(string dir, bool throwIfFailed)
        {
            if (!Directory.Exists(dir))
            {
                _logger.LogCritical("Directory {inDir} not exist", dir);
                throw new DirectoryNotFoundException();
            }

            var files = Directory.GetFiles(dir, "*.json");
            var configs = new List<WgHostConfig>();
            var failedCount = 0;
            foreach (var file in files)
            {
                try
                {
                    configs.Add(WgHostConfigManager.LoadFile(file));
                }
                catch (Exception e)
                {
                    failedCount++;
                    _logger.LogError(e, "Skip file: {file}", file);
                }
            }

            _logger.LogInformation("Loaded: {count}", configs.Count);
            if (failedCount == 0)
            {
                _logger.LogInformation("Failed {count}", failedCount);
            }
            else
            {
                if (throwIfFailed)
                {
                    _logger.LogCritical("Failed {count}", failedCount);
                    throw new Exception("Some configs not loaded");
                }

                _logger.LogError("Failed {count}", failedCount);
            }

            return configs;
        }

        private IReadOnlyList<ConfigValidationMessage> ValidateConfig(WgHostConfig host)
        {
            _hostValidators ??= _serviceProvider.GetRequiredService<IEnumerable<IWgHostConfigValidator>>().ToArray();
            return _hostValidators.SelectMany(x => x.Validate(host)).ToArray();
        }

        private IReadOnlyList<ConfigValidationMessage> ValidateMesh(WgMeshConfig mesh)
        {
            _meshValidators ??= _serviceProvider.GetRequiredService<IEnumerable<IWgMeshConfigValidator>>().ToArray();
            return _meshValidators.SelectMany(x => x.Validate(mesh)).ToArray();
        }
    }
}