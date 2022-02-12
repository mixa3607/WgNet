using System;
using System.IO;
using ArkProjects.Wireguard.Mesh.CConverters;
using ArkProjects.Wireguard.Mesh.Configs;
using Microsoft.Extensions.Logging;

namespace ArkProjects.Wireguard.Deploy.DeployMethods
{
    public static partial class LocalFsWgDeploy
    {
        public class Method : IWireguardDeployMethod
        {
            private readonly WgIniConfigConverter _iniConverter;
            private readonly WgUciConfigConverter _uciConverter;
            private readonly ILogger<Method> _logger;
            private readonly WgDeployTargetConfig _target;
            private readonly Options _options;

            private bool _initialized = false;
            private WgHostConfig _wgConfig;

            public Method(WgDeployTargetConfig target, ILogger<Method> logger,
                WgIniConfigConverter iniConverter,
                WgUciConfigConverter uciConverter)
            {
                _logger = logger;
                _target = target;
                _iniConverter = iniConverter;
                _uciConverter = uciConverter;
                _options = _target.DeployMethodOptions.ToObject<Options>();
            }

            public void Init()
            {
                _wgConfig = WgHostConfigManager.LoadFile(_target.ConfigFile);
                _initialized = true;
            }

            // TODO: rewrite
            public int Check(bool allowNonZeroCode)
            {
                if (string.IsNullOrWhiteSpace(_options.Dir))
                {
                    _logger.LogError("{prop} property is empty", nameof(_options.Dir));
                    throw new Exception($"{nameof(_options.Dir)} property is empty");
                }

                if (!_options.CreateDirs && !Directory.Exists(_options.Dir))
                {
                    _logger.LogError("{prop} not exist. Enable {flag}", nameof(_options.Dir), nameof(_options.CreateDirs));
                    throw new Exception($"{nameof(_options.Dir)} not exist. Enable {nameof(_options.CreateDirs)}");
                }

                return 0;
            }

            public int RemoveConfig(bool allowNonZeroCode)
            {
                var converter = GetConverter();
                var file = _options.File;
                if (string.IsNullOrWhiteSpace(file))
                {
                    file = _wgConfig.Name + converter.Extension;
                    _logger.LogWarning("{prop} is empty, use {file} instead", nameof(_options.File), file);
                }

                var dest = Path.Combine(_options.Dir, file);
                if (File.Exists(dest))
                {
                    _logger.LogDebug("Delete {dest}", dest);
                    File.Delete(dest);
                    return 0;
                }

                if (allowNonZeroCode)
                {
                    _logger.LogWarning("File {dest} not exist", dest);
                    return 1;
                }

                _logger.LogError("File {dest} not exist", dest);
                throw new FileNotFoundException($"File {dest} not exist", dest);
            }

            public int UploadConfig(bool allowNonZeroCode)
            {
                try
                {
                    if (_options.CreateDirs && !Directory.Exists(_options.Dir))
                    {
                        _logger.LogDebug("Create dir {dir}", _options.Dir);
                        Directory.CreateDirectory(_options.Dir);
                    }

                    var converter = GetConverter();
                    var file = _options.File;
                    if (string.IsNullOrWhiteSpace(file))
                    {
                        file = _wgConfig.Name + converter.Extension;
                        _logger.LogWarning("{prop} is empty, use {file} instead", nameof(_options.File), file);
                    }

                    var dest = Path.Combine(_options.Dir, file);
                    _logger.LogDebug("Save to {dest}", dest);
                    converter.SaveFile(dest, _wgConfig);
                }
                catch (Exception e)
                {
                    if (allowNonZeroCode)
                    {
                        _logger.LogWarning(e, "Unhandled ex. Ignore");
                        return 1;
                    }

                    _logger.LogError(e, "Unhandled ex");
                    throw;
                }

                return 0;
            }

            public int DownInterface(bool allowNonZeroCode) => 0;

            public int UpInterface(bool allowNonZeroCode) => 0;

            public void Dispose()
            {
            }

            private IWgConfigConverter GetConverter()
            {
                return _options.ConvertTo switch
                {
                    WgConfigGenType.Ini => _iniConverter,
                    WgConfigGenType.UciBatch => _uciConverter,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }
    }
}