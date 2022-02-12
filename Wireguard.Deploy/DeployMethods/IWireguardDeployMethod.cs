using System;

namespace ArkProjects.Wireguard.Deploy.DeployMethods
{
    /// <summary>
    /// Deploy method
    /// </summary>
    public interface IWireguardDeployMethod : IDisposable
    {
        /// <summary>
        /// Initialize all internal processes
        /// </summary>
        void Init();

        /// <summary>
        /// Check all dirs/connections/bins/etc
        /// </summary>
        /// <param name="allowNonZeroCode">Ignore errors</param>
        /// <returns></returns>
        int Check(bool allowNonZeroCode);

        /// <summary>
        /// Down interface
        /// </summary>
        /// <param name="allowNonZeroCode">Ignore errors</param>
        /// <returns></returns>
        int DownInterface(bool allowNonZeroCode);

        /// <summary>
        /// Remove config/Remove from auto start
        /// </summary>
        /// <param name="allowNonZeroCode">Ignore errors</param>
        /// <returns></returns>
        int RemoveConfig(bool allowNonZeroCode);

        /// <summary>
        /// Upload/Add to auto start/Save config
        /// </summary>
        /// <param name="allowNonZeroCode">Ignore errors</param>
        /// <returns></returns>
        int UploadConfig(bool allowNonZeroCode);

        /// <summary>
        /// Up interface
        /// </summary>
        /// <param name="allowNonZeroCode">Ignore errors</param>
        /// <returns></returns>
        int UpInterface(bool allowNonZeroCode);
    }
}