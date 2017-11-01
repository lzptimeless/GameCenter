using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    }
}
