using System;
using System.Collections.Generic;
using System.Linq;
using ArkProjects.Wireguard.Deploy.DeployMethods;
using Microsoft.Extensions.Logging;

namespace ArkProjects.Wireguard.Deploy
{
    public class WgDeployMaster
    {
        private readonly ILogger<WgDeployMaster> _logger;
        private readonly IReadOnlyList<IWireguardDeployMethodFactory<IWireguardDeployMethod>> _deployMethodFactories;

        public WgDeployMaster(IEnumerable<IWireguardDeployMethodFactory<IWireguardDeployMethod>> deployMethodFactories, ILogger<WgDeployMaster> logger)
        {
            _deployMethodFactories = deployMethodFactories.ToList();
            _logger = logger;
        }

        public IReadOnlyList<IWireguardDeployMethodInfo> GetAllMethodsInfo()
        {
            return _deployMethodFactories.Select(x => x.Info).ToArray();
        }

        public void Deploy(WgDeployTargetConfig target, WgDeployStageType stages)
        {
            if (target.Disable)
            {
                _logger.LogInformation("Config {name} disabled. Skip!", target.Name);
                return;
            }
            try
            {
                _logger.LogInformation("Deploying {name} over {method}", target.ConfigFile, target.DeployMethod);

                using var deployMethod = _deployMethodFactories.First(x => x.Info.Name == target.DeployMethod).CreateInstance(target);
                _logger.LogDebug("Stage: Init");
                deployMethod.Init();

                ExecStage(deployMethod.Check, target.AllowCheckFail, stages, WgDeployStageType.Check);
                ExecStage(deployMethod.DownInterface, target.AllowDownFail, stages, WgDeployStageType.DownIf);
                ExecStage(deployMethod.RemoveConfig, target.AllowRemoveFail, stages, WgDeployStageType.Remove);
                ExecStage(deployMethod.UploadConfig, target.AllowUploadFail, stages, WgDeployStageType.Upload);
                ExecStage(deployMethod.UpInterface, target.AllowUpFail, stages, WgDeployStageType.UpIf);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while deploy");
            }
        }

        private int? ExecStage(Func<bool, int> stage, bool allowFail, WgDeployStageType targetStages, WgDeployStageType thisStage)
        {
            if (targetStages.HasFlag(thisStage))
            {
                _logger.LogDebug("Stage: {stage}", thisStage);
                return stage(allowFail);
            }

            _logger.LogWarning("Skip stage: {stage}", thisStage);
            return null;
        }
    }
}