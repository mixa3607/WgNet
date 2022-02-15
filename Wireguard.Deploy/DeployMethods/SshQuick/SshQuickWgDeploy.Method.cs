using System;
using System.Collections.Generic;
using System.IO;
using ArkProjects.Wireguard.Deploy.Exceptions;
using ArkProjects.Wireguard.Mesh.CConverters;
using ArkProjects.Wireguard.Mesh.Configs;
using Microsoft.Extensions.Logging;
using Renci.SshNet;

namespace ArkProjects.Wireguard.Deploy.DeployMethods
{
    public static partial class SshQuickWgDeploy
    {
        public class Method : IWireguardDeployMethod
        {
            private readonly ILogger<Method> _logger;
            private readonly WgIniConfigConverter _iniConverter;
            private readonly WgDeployTargetConfig _target;
            private readonly Options _options;

            private bool _initialized;
            private SshClient _client;
            private WgHostConfig _wgConfig;

            public Method(WgDeployTargetConfig target, ILogger<Method> logger, WgIniConfigConverter iniConverter)
            {
                _logger = logger;
                _target = target;
                _iniConverter = iniConverter;
                _options = _target.DeployMethodOptions.ToObject<Options>();
            }

            public void Init()
            {
                _wgConfig = WgHostConfigManager.LoadFile(_target.ConfigFile);
                _logger.LogDebug("Create connection to {user}@{host}:{port}", _options.User, _options.Host, _options.Port);

                var authMethods = new List<AuthenticationMethod>();
                authMethods.Add(new PrivateKeyAuthenticationMethod(_options.User, new PrivateKeyFile(_options.PrivateKeyFile, _options.PrivateKeyFilePhrase)));
                if (!string.IsNullOrEmpty(_options.Password))
                    authMethods.Add(new PasswordAuthenticationMethod(_options.User, _options.Password));

                var connInfo = new ConnectionInfo(_options.Host, _options.Port, _options.User, authMethods.ToArray());

                _client = new SshClient(connInfo);
                _logger.LogInformation("Connecting to {user}@{host}:{port}", _options.User, _options.Host, _options.Port);
                _client.Connect();
                _initialized = true;
            }

            public int Check(bool allowNonZeroCode)
            {
                ThrowIfNotInitialized();
                _logger.LogDebug("Check wireguard bins");
                var script = FilesHelper.GetScript(Info.Name, "check.sh");

                var result = Exec(script);
                ThrowIfBadCode(result, allowNonZeroCode, $"Check success", "Check failed");

                return result.ExitStatus;
            }

            public int RemoveConfig(bool allowNonZeroCode)
            {
                ThrowIfNotInitialized();
                var dir = $"{_options.RemoteConfigsDirectory}";
                var file = $"{_wgConfig.InterfaceName}{_iniConverter.Extension}";
                var script = FilesHelper.GetScript(Info.Name, "remove.sh");
                var ifName = _wgConfig.InterfaceName;

                var result = Exec(script, dir, file, ifName);
                ThrowIfBadCode(result, allowNonZeroCode, $"Config removed from {Path.Combine(dir, file)}", "Removing failed");

                return result.ExitStatus;
            }

            public int UploadConfig(bool allowNonZeroCode)
            {
                ThrowIfNotInitialized();
                var cfg = _iniConverter.Save(_wgConfig);
                var dir = $"{_options.RemoteConfigsDirectory}";
                var file = $"{_wgConfig.InterfaceName}{_iniConverter.Extension}";
                var ifName = _wgConfig.InterfaceName;
                var enable = _options.SysCtlEnable ? "enable" : "";
                var script = FilesHelper.GetScript(Info.Name, "upload.sh");

                var result = Exec(script, cfg, dir, file, ifName, enable);
                ThrowIfBadCode(result, allowNonZeroCode, $"Config uploaded to {Path.Combine(dir, file)}", "Upload failed");

                return result.ExitStatus;
            }

            public int DownInterface(bool allowNonZeroCode)
            {
                ThrowIfNotInitialized();
                var script = FilesHelper.GetScript(Info.Name, "down.sh");
                var ifName = _wgConfig.InterfaceName;

                var result = Exec(script, ifName);
                ThrowIfBadCode(result, allowNonZeroCode, "Interface down", "Down failed");

                return result.ExitStatus;
            }

            public int UpInterface(bool allowNonZeroCode)
            {
                ThrowIfNotInitialized();
                var script = FilesHelper.GetScript(Info.Name, "up.sh");
                var ifName = _wgConfig.InterfaceName;

                var result = Exec(script, ifName);
                ThrowIfBadCode(result, allowNonZeroCode, "Interface up", "Up failed");

                return result.ExitStatus;
            }

            public void Dispose()
            {
                _client?.Dispose();
            }

            private SshCommand Exec(string script) => Exec(script, Array.Empty<string>());

            private SshCommand Exec(string script, params string[] args) =>
                SshClientHelper.Exec(new ExecArgs()
                {
                    Password = _options.Password,
                    Args = args,
                    Client = _client,
                    OverSudo = _options.OverSudo,
                    Script = script,
                    SudoUser = _options.SudoUser,
                    Shell = _options.Shell
                }, _logger);

            private void ThrowIfNotInitialized()
            {
                if (!_initialized)
                    throw new DeployMethodNotInitialized();
            }

            private void ThrowIfBadCode(SshCommand command, bool ignoreBadCode, string okLog, string badLog)
            {
                if (command.ExitStatus != 0)
                {
                    if (ignoreBadCode)
                    {
                        _logger.LogWarning($"{badLog}. {{out}}", command.Result);
                    }
                    else
                    {
                        _logger.LogError($"{badLog}. {{out}}", command.Result);
                        throw new BadCommandExitCodeException($"{badLog}. See trace log", command.ExitStatus);
                    }
                }
                else
                {
                    _logger.LogInformation(okLog);
                }
            }
        }
    }
}