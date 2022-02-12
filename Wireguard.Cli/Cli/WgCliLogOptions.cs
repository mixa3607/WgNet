using PowerArgs;
using Serilog.Events;

namespace ArkProjects.Wireguard.Cli.Cli;

public class WgCliLogOptions
{
    [ArgShortcut("--file-level"), ArgDescription("File log level"), ArgDefaultValue(LogEventLevel.Verbose)]
    public LogEventLevel FileLogLevel { get; set; } = LogEventLevel.Verbose;

    [ArgShortcut("--console-level"), ArgDescription("Console log level"), ArgDefaultValue(LogEventLevel.Information)]
    public LogEventLevel ConsoleLogLevel { get; set; } = LogEventLevel.Information;

    [ArgShortcut("--log-file"), ArgDescription("Log file"), ArgDefaultValue("out.log")]
    public string LogFile { get; set; } = "out.log";
}