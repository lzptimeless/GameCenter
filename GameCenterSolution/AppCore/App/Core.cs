using System;
using System.Collections.Generic;
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
            var logger = new FileLogger("Log.txt", 1);
            Logger = logger;

            var uiManager = new UIManager();
            UIManager = uiManager;

            uiManager.Load();

            var moduleManager = new ModuleManager();
            ModuleManager = moduleManager;
            
            moduleManager.Load();

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
