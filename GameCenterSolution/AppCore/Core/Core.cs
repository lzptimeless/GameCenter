using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AppCore
{
    public class Core : ICore
    {
        static Core()
        {
            Instance = new Core();
        }

        private Core()
        { }

        public static ICore Instance { get; private set; }

        public CoreConfig Config
        {
            get; private set;
        }

        public ILogger Logger
        {
            get; internal set;
        }

        public IModuleManager ModuleManager
        {
            get; internal set;
        }

        internal UIManager UIManager
        {
            get; set;
        }

        public void Run()
        {
            // 加载日志
            var logger = new FileLogger("Log.txt", 1);
            Logger = logger;
            // 加载核心配置
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string coreConfigPath = Path.Combine(baseDir, "CoreConfig.xml");
            CoreConfig coreConfig = new CoreConfig();

            if (File.Exists(coreConfigPath)) coreConfig.Load(coreConfigPath);

            Config = coreConfig;
            // 设置程序集解析路径
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += CurrentDomain_ReflectionOnlyAssemblyResolve;

            // 加载UI
            var uiManager = new UIManager();
            UIManager = uiManager;

            uiManager.Load();
            // 加载模块
            var moduleManager = new ModuleManager();
            ModuleManager = moduleManager;

            moduleManager.Load();
            // 开始
            uiManager.StartWork();
        }

        public void Shutdown()
        {
            var moduleManager = ModuleManager as ModuleManager;
            moduleManager.Release();

            UIManager.Release();

            var fileLogger = Logger as FileLogger;
            fileLogger.Dispose();
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            AssemblyName asmName = new AssemblyName(args.Name);
            Assembly asmExist = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName == args.Name);
            if (asmExist != null) return asmExist;

            string asmFileName = asmName.Name + ".dll";
            string interfaceFolderPath = Config.GetInterfaceFolderFullPath();
            if (!string.IsNullOrEmpty(interfaceFolderPath))
            {
                string interfaceFilePath = Path.Combine(interfaceFolderPath, asmFileName);

                if (File.Exists(interfaceFilePath)) return Assembly.LoadFrom(interfaceFilePath);
            }

            string moduleFolderPath = Config.GetModuleFolderFullPath();
            if (!string.IsNullOrEmpty(moduleFolderPath))
            {
                string moduleFilePath = Path.Combine(moduleFolderPath, asmFileName);

                if (File.Exists(moduleFilePath)) return Assembly.LoadFrom(moduleFilePath);
            }

            string uiFolderPath = Config.GetUIFolderFullPath();
            if (!string.IsNullOrEmpty(uiFolderPath))
            {
                string uiFilePath = Path.Combine(uiFolderPath, asmFileName);

                if (File.Exists(uiFilePath)) return Assembly.LoadFrom(uiFilePath);
            }

            return null;
        }

        private Assembly CurrentDomain_ReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
        {
            AssemblyName asmName = new AssemblyName(args.Name);
            Assembly asmExist = AppDomain.CurrentDomain.ReflectionOnlyGetAssemblies().FirstOrDefault(a => a.FullName == args.Name);
            if (asmExist != null) return asmExist;

            string asmFileName = asmName.Name + ".dll";
            string interfaceFolderPath = Config.GetInterfaceFolderFullPath();
            if (!string.IsNullOrEmpty(interfaceFolderPath))
            {
                string interfaceFilePath = Path.Combine(interfaceFolderPath, asmFileName);

                if (File.Exists(interfaceFilePath)) return Assembly.ReflectionOnlyLoadFrom(interfaceFilePath);
            }

            string moduleFolderPath = Config.GetModuleFolderFullPath();
            if (!string.IsNullOrEmpty(moduleFolderPath))
            {
                string moduleFilePath = Path.Combine(moduleFolderPath, asmFileName);

                if (File.Exists(moduleFilePath)) return Assembly.ReflectionOnlyLoadFrom(moduleFilePath);
            }

            string uiFolderPath = Config.GetUIFolderFullPath();
            if (!string.IsNullOrEmpty(uiFolderPath))
            {
                string uiFilePath = Path.Combine(uiFolderPath, asmFileName);

                if (File.Exists(uiFilePath)) return Assembly.ReflectionOnlyLoadFrom(uiFilePath);
            }

            return Assembly.ReflectionOnlyLoad(args.Name);
        }
    }
}
