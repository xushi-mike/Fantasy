#if FANTASY_SERVER
using CommandLine;
using Fantasy.Core;
using Fantasy.Helper;
using NLog;

namespace Fantasy
{
    public static class Application
    {
        public static void Initialize(int assemblyNameCoreKey)
        {
            // 设置默认的线程的同步上下文
            SynchronizationContext.SetSynchronizationContext(ThreadSynchronizationContext.Main);
            // 解析命令行参数
            Parser.Default.ParseArguments<CommandLineOptions>(Environment.GetCommandLineArgs())
                .WithNotParsed(error => throw new Exception("Command line format error!"))
                .WithParsed(option => Define.Options = option);
            // 检查启动参数
            switch (Define.Options.AppType)
            {
                case "Game":
                {
                    break;
                }
#if FANTASY_DEVELOP
                case "Export":
                {
                    new Exporter().Start();
                    return;
                }    
#endif
                default:
                {
                    throw new NotSupportedException($"AppType is {Define.Options.AppType} Unrecognized!");
                }
            }
            // 根据不同的运行模式来选择日志的方式
            switch (Define.Options.Mode)
            {
                case "Develop":
                {
                    LogManager.Configuration.RemoveRuleByName("ServerDebug");
                    LogManager.Configuration.RemoveRuleByName("ServerTrace");
                    LogManager.Configuration.RemoveRuleByName("ServerInfo");
                    LogManager.Configuration.RemoveRuleByName("ServerWarn");
                    LogManager.Configuration.RemoveRuleByName("ServerError");
                    break;
                }
                case "Release":
                {
                    LogManager.Configuration.RemoveRuleByName("ConsoleTrace");
                    LogManager.Configuration.RemoveRuleByName("ConsoleDebug");
                    LogManager.Configuration.RemoveRuleByName("ConsoleInfo");
                    LogManager.Configuration.RemoveRuleByName("ConsoleWarn");
                    LogManager.Configuration.RemoveRuleByName("ConsoleError");
                    break;
                }
            }
            // 初始化SingletonSystemCenter这个一定要放到最前面
            // 因为SingletonSystem会注册AssemblyManager的OnLoadAssemblyEvent和OnUnLoadAssemblyEvent的事件
            // 如果不这样、会无法把程序集的单例注册到SingletonManager中
            SingletonSystem.Initialize();
            // 加载核心程序集
            AssemblyManager.Load(assemblyNameCoreKey, typeof(AssemblyManager).Assembly);
        }
        
        public static async FTask Start()
        {
            switch (Define.Options.Mode)
            {
                case "Develop":
                {
                    // 开发模式默认所有Server都在一个进程中、方便调试、但网络还都是独立的
                    var serverConfigInfos = ConfigTableManage.AllServerConfig();
                    
                    foreach (var serverConfig in serverConfigInfos)
                    {
                        await Server.Create(serverConfig.Id);
                    }
                
                    return;
                }
                case "Release":
                {
                    // 发布模式只会启动启动参数传递的Server、也就是只会启动一个Server
                    // 您可以做一个Server专门用于管理启动所有Server的工作
                    await Server.Create(Define.Options.AppId);
                    return;
                }
            }
        }
        
        public static void Close()
        {
            SingletonSystem.Dispose();
            AssemblyManager.Dispose();
        }
    }
}
#endif