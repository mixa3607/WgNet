using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Renci.SshNet;

namespace ArkProjects.Wireguard.Deploy
{
    public static class SshClientHelper
    {
        public static SshCommand ExecWait(SshClient client, string command, ILogger logger)
        {
            var result = client.RunCommand(command);
            logger.LogTrace("Command: {command}. Exit code {code}, StdOut: {out}, StdErr: {err}",
                command, result.ExitStatus, result.Result, result.Error);
            return result;
        }

        public static string ThrowIfBinNotExist(SshClient client, string binName, ILogger logger)
        {
            var whichResult = ExecWait(client, $"which {binName}", logger);
            if (whichResult.ExitStatus != 0)
            {
                logger.LogError("{bin} not found", binName);
                throw new Exception($"{binName} not found");
            }

            return whichResult.Result.Split("\n")[0];
        }

        public static SshCommand Exec(ExecArgs args, ILogger logger)
        {
            var scriptEsc = ShellEscape.Transform(args.Script);
            var argsEsc = args.Args.Select(ShellEscape.Transform).ToArray();
            var argsStr = string.Join(" ", argsEsc);
            if (args.OverSudo)
            {
                var sudoPref = ShellEscape.Transform($"{args.Password}\n");
                var user = ShellEscape.Transform(args.SudoUser);
                return ExecWait(args.Client, $"echo {sudoPref}{scriptEsc} | sudo -u {user} -p '' -k -S {args.Shell} -s - {argsStr}", logger);
            }
            else
            {
                return ExecWait(args.Client, $"echo {scriptEsc} | {args.Shell} -s - {argsStr}", logger);
            }
        }
    }
}