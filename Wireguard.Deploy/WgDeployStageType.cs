using System;

namespace ArkProjects.Wireguard.Deploy
{
    [Flags]
    public enum WgDeployStageType
    {
        Nothing = 0,
        Check = 1,
        DownIf = 2,
        Remove = 4,
        Upload = 8,
        UpIf = 16,

        All = Check | DownIf | Remove | Upload | UpIf
    }
}