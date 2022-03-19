# WgNet
Wireguard mesh config builder written on .NET

Тулза для генерации и деплоя конфигов вг на хосты. Так же можно экспортить.

### wg-net
```
$ ./wg-net -h
Usage -  <action> -options
GlobalOption                        Description
Help (-?, -h, --help)               Shows this help
Version (-v, --version)             Show version info
FileLogLevel (--file-level)         File log level [Default='Verbose']
                                    Verbose
                                    Debug
                                    Information
                                    Warning
                                    Error
                                    Fatal
ConsoleLogLevel (--console-level)   Console log level [Default='Information']
                                    Verbose
                                    Debug
                                    Information
                                    Warning
                                    Error
                                    Fatal
LogFile (--log-file)                Log file [Default='out.log']
Actions
  ValidateMesh -options -
    Option               Description
    Config (-c, --cfg)   Mesh config file [Default='./mesh.json']
  ValidateConfigs -options -
    Option                 Description
    Names (-n, --names)    Deploy selected targets instead all
    InDir (-s, --in-dir)   Directory with host configs [Default='./configs']
  GenerateKeys -options -
    Option                            Description
    Config (-c, --cfg)                Mesh config file [Default='./mesh.json']
    OutConfig (-o, --out)             Out file. Use input file by default
    WithPreShared (-p, --preshared)   Generate pre-shared keys [Default='True']
    ReGenerate (--regenerate)         Drop all keys and generate new [Default='False']
    FixInvalid (--fix)                Fix invalid keys [Default='True']
  BuildMesh -options -
    Option                               Description
    Config (-c, --cfg)                   Mesh config file [Default='./mesh.json']
    OutDir (-d, --out-dir)               Out directory for configs
                                         [Default='./configs']
    IgnoreConfigErrors (--ignore-errs)   Ignore all validation errors. If not
                                         set then only Note level allowed
  ExportConfig -options -
    Option                               Description
    Names (-n, --names)                  Deploy selected targets instead all
    Type (-t, --type)
                                         Unknown
                                         Ini
                                         UciBatch
    OutDir (-d, --out-dir)               [Default='./export']
    InDir (-s, --in-dir)                 Directory with host configs
                                         [Default='./configs']
    IgnoreConfigErrors (--ignore-errs)   Ignore all validation errors. If not
                                         set then only Note level allowed
  DeployConfig -options -
    Option                         Description
    Config (-c, --cfg)             Deploy config [Default='./deploy.json']
    Names (-n, --names)            Deploy selected targets instead all
    TargetStages (-t)              Execute only selected stages [Default='All']
                                   Nothing
                                   Check
                                   DownIf
                                   Remove
                                   Upload
                                   UpIf
                                   All
    ListMethods (--list-methods)   List all methods and exit

$ ./wg-net -v
wg-net version 1.0.1, commit a156fdd5eb1921d7a260a735aa9c954dd5820ce7
Tag: v1.0.1
Build: 03/10/2022 21:16:57
Project: https://github.com/mixa3607/WgNet
Repo: git://github.com/mixa3607/WgNet.git
```
