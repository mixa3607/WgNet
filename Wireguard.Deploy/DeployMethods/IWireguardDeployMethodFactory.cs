namespace ArkProjects.Wireguard.Deploy.DeployMethods
{
    public interface IWireguardDeployMethodFactory<out T> where T : IWireguardDeployMethod
    {
        /// <summary>
        /// Method info
        /// </summary>
        IWireguardDeployMethodInfo Info { get; }

        T CreateInstance(WgDeployTargetConfig target);
    }
}