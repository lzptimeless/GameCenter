using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AppCore
{
    internal class ModuleContainer
    {
        private struct ModuleRegistration
        {
            public ModuleRegistration(Type type, Type moduleInterface, IModule instance)
            {
                Type = type;
                Interface = moduleInterface;
                Instance = instance;
            }

            public Type Type { get; set; }
            public Type Interface { get; set; }
            public IModule Instance { get; set; }
        }

        public ModuleContainer()
        {
            _items = new ConcurrentBag<ModuleRegistration>();
        }

        private ConcurrentBag<ModuleRegistration> _items;

        public void Register(Type moduleType, Type moduleInterface, IModule moduleInstance)
        {
            if (moduleType == null) throw new ArgumentNullException("moduleType");
            if (moduleInterface == null) throw new ArgumentNullException("moduleInterface");
            if (moduleInstance == null) throw new ArgumentNullException("moduleInstance");

            var reg = new ModuleRegistration(moduleType, moduleInterface, moduleInstance);
            _items.Add(reg);
        }

        public IModule GetModule(string moduleInterfaceName)
        {
            return DoGetModule(null, null, moduleInterfaceName);
        }

        public IModule GetModule(Type moduleInterface)
        {
            return DoGetModule(null, moduleInterface, null);
        }

        public TModule GetModule<TModule>() where TModule : IModule
        {
            return (TModule)DoGetModule(null, typeof(TModule), null);
        }

        public IModule[] GetModules()
        {
            return _items.Select(m => m.Instance).ToArray();
        }

        private IModule DoGetModule(Type moduleType, Type moduleInterface, string moduleInterfaceName)
        {
            List<IModule> modules = new List<IModule>();
            foreach (var item in _items)
            {
                if ((moduleType == null || item.Type == moduleType) &&
                    (moduleInterface == null || item.Interface == moduleInterface) &&
                    (string.IsNullOrEmpty(moduleInterfaceName) || item.Interface.FullName == moduleInterfaceName))
                    modules.Add(item.Instance);
            }

            if (modules.Count == 0) return null;
            if (modules.Count > 1) throw new ActivationException($"{moduleType}:{moduleInterface} has multi result.");

            return modules[0];
        }
    }
}
