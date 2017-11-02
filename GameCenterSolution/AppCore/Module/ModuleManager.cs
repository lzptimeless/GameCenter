using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AppCore
{
    public class ModuleManager : IModuleManager
    {
        public ModuleManager()
        {
            _catalog = new ModuleCatalog();
            _container = new ModuleContainer();
            _enviroments = new Dictionary<string, ModuleEnviroment>();
        }

        private ModuleCatalog _catalog;
        private ModuleContainer _container;
        /// <summary>
        /// Key: Module full type name, Value: <see cref="ModuleEnviroment"/>
        /// </summary>
        private Dictionary<string, ModuleEnviroment> _enviroments;

        public IModule GetModule(Type moduleInterfaceType)
        {
            return _container.GetModule(moduleInterfaceType);
        }

        public TModule GetModule<TModule>() where TModule : IModule
        {
            return _container.GetModule<TModule>();
        }

        public IModule[] GetModules()
        {
            return _container.GetModules();
        }

        public ModuleEnviroment GetModuleEnviroment<TModule>() where TModule : IModule
        {
            Type interfaceType = typeof(TModule);

            if (!_enviroments.ContainsKey(interfaceType.FullName))
                throw new ArgumentException($"Can not found enviroment for {interfaceType.FullName}");

            return _enviroments[interfaceType.FullName];
        }

        public void UnsubscribeEvents(object target)
        {
            var modules = _container.GetModules();
            foreach (var module in modules)
            {
                module.UnsubscribeEvents(target);
            }
        }

        internal void Load()
        {
            _catalog.Initialize();
            CreateModules(_catalog.Items);
        }

        internal void Release()
        {
            var modules = _container.GetModules();
            modules = modules.Reverse().ToArray();

            foreach (var module in modules)
            {
                module.Release();
            }
        }

        private void CreateModules(IEnumerable<ModuleInfo> modules)
        {
            foreach (var moduleInfo in modules)
            {
                // 创建模块环境
                if (!_enviroments.ContainsKey(moduleInfo.InterfaceTypeFullName))
                {
                    string userDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    string appName = Core.Instance.Config.AppName;
                    string moduleUserDataFolder = Path.Combine(userDataFolder, appName, moduleInfo.TypeName);
                    ModuleEnviroment moduleEnviroment = new ModuleEnviroment(moduleInfo.AssemblyFullName, moduleUserDataFolder);
                    _enviroments.Add(moduleInfo.InterfaceTypeFullName, moduleEnviroment);
                }

                // 加载程序集
                // Load assembly
                moduleInfo.State = ModuleStates.LoadingTypes;
                Assembly moduleInterfaceAsm = GetOrLoadAssembly(moduleInfo.InterfaceAssemblyCodeBase);
                Type moduleInterfaceType = moduleInterfaceAsm.GetType(moduleInfo.InterfaceTypeFullName);
                Assembly moduleAsm = GetOrLoadAssembly(moduleInfo.AssemblyCodeBase);
                Type moduleType = moduleAsm.GetType(moduleInfo.TypeFullName);

                // 创建模块
                // Create module
                moduleInfo.State = ModuleStates.ReadyForInitialization;
                IModule moduleInstance = Activator.CreateInstance(moduleType) as IModule;
                if (moduleInstance == null) throw new ModularityException($"Create {moduleType} failed.", null);

                // 初始化模块
                // Initialize module
                moduleInfo.State = ModuleStates.Initializing;
                List<IModule> dependencies = new List<IModule>();
                foreach (var dependencyName in moduleInfo.Dependencies)
                {
                    IModule dependentModule = _container.GetModule(dependencyName);
                    if (dependentModule == null) throw new ModularityException($"Get dependent:{dependencyName} failed.", null);
                    dependencies.Add(dependentModule);
                }
                moduleInstance.Initialize(dependencies.ToArray());

                // 添加到模块容器
                _container.Register(moduleType, moduleInterfaceType, moduleInstance);

                moduleInfo.State = ModuleStates.Initialized;
            }
        }

        private Assembly GetOrLoadAssembly(string codeBase)
        {
            Assembly asm = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.CodeBase == codeBase);
            if (asm == null)
            {
                asm = Assembly.LoadFrom(codeBase);
            }

            return asm;
        }
    }
}
