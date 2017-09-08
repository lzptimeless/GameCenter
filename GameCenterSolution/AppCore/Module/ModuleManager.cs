using System;
using System.Collections.Generic;
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
        }

        private ModuleCatalog _catalog;
        private ModuleContainer _container;

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

        public void UnsubscribeEvents(object target)
        {
            var modules = _container.GetModules();
            foreach (var module in modules)
            {
                module.UnsubscribeEvents(target);
            }
        }

        internal void LoadModules()
        {
            _catalog.Initialize();
            CreateModules(_catalog.Items);
        }

        internal void ReleaseModules()
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
                // 加载程序集
                // Load assembly
                moduleInfo.State = ModuleStates.LoadingTypes;
                Assembly moduleInterfaceAsm = GetOrLoadAssembly(moduleInfo.InterfaceFile);
                Type moduleInterfaceType = moduleInterfaceAsm.GetType(moduleInfo.Interface);
                Assembly moduleAsm = GetOrLoadAssembly(moduleInfo.File);
                Type moduleType = moduleAsm.GetType(moduleInfo.Type);

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
