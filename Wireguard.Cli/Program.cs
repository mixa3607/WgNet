using ArkProjects.Wireguard.Deploy;
using ArkProjects.Wireguard.Deploy.DeployMethods;
using ArkProjects.Wireguard.Mesh;
using ArkProjects.Wireguard.Mesh.CConverters;
using ArkProjects.Wireguard.Mesh.Configs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PowerArgs;
using Serilog;
using Serilog.Events;

namespace ArkProjects.Wireguard.Cli
{
    static class Program
    {
        static void Main(string[] args)
        {
            var logOptions = Args.Parse<WgCliLogOptions>();
            var host = CreateHost(logOptions).Build();

            //reg factories
            Args.RegisterFactory(typeof(WgCli), () => host.Services.GetRequiredService<WgCli>());

            //invoke
            Args.InvokeAction<WgCli>(args);
        }

        public static IHostBuilder CreateHost(WgCliLogOptions options)
        {
            var builder = new HostBuilder()
                .UseContentRoot("./")
                .UseSerilog((x, logger) =>
                {
                    logger.MinimumLevel.Is(LogEventLevel.Verbose)
                        .WriteTo.Console(options.ConsoleLogLevel)
                        .WriteTo.File(options.LogFile, options.FileLogLevel);
                })
                .ConfigureServices(services =>
                {
                    services.AddSingleton<WgConfigGeneratorInfo>();
                    services.AddSingleton<WgIniConfigConverter>();
                    services.AddSingleton<WgUciConfigConverter>();
                    services.AddSingleton<IWgConfigConverter>(x => x.GetRequiredService<WgIniConfigConverter>());
                    services.AddSingleton<IWgConfigConverter>(x => x.GetRequiredService<WgUciConfigConverter>());

                    services.AddSingleton<IWireguardDeployMethodFactory<IWireguardDeployMethod>, LocalFsWgDeploy.Factory>();
                    services.AddSingleton<IWireguardDeployMethodFactory<IWireguardDeployMethod>, SshQuickWgDeploy.Factory>();
                    services.AddSingleton<IWireguardDeployMethodFactory<IWireguardDeployMethod>, SshUciBatchWgDeploy.Factory>();

                    services.AddSingleton<IWgMeshConfigValidator, WgConfigValidator>();
                    services.AddSingleton<IWgHostConfigValidator, WgConfigValidator>();

                    services.AddTransient<WgCli>();

                    services.AddSingleton<WgDeployMaster>();
                    services.AddSingleton<WgMeshBuilder>();
                    services.AddSingleton<WgKeyBuilder>();
                });
            return builder;
        }
    }
}