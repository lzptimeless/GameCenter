using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AppCore
{
    internal class ModuleCatalog
    {
        public ModuleCatalog()
        {
            _items = new List<ModuleInfo>();
        }

        private List<ModuleInfo> _items;

        public IReadOnlyList<ModuleInfo> Items
        {
            get { return _items; }
        }

        public void Initialize()
        {
            SetReflectionOnlyAssemblyResolve();
            Load();
            ClearReflectionOnlyAssemblyResolve();
            ValidateUnique();
            ValidateDependencyGraph();
            Sort();
        }

        private void Sort()
        {
            List<ModuleInfo> sortedItems = new List<ModuleInfo>();
            foreach (var itemName in SolveDependencies(_items))
            {
                ModuleInfo moduleInfo = _items.First(m => m.Interface == itemName);
                sortedItems.Add(moduleInfo);
            }

            _items = sortedItems;
        }

        private void AddModule(ModuleInfo moduleInfo)
        {
            _items.Add(moduleInfo);
        }

        private void SetReflectionOnlyAssemblyResolve()
        {
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += CurrentDomain_ReflectionOnlyAssemblyResolve;
        }

        private void ClearReflectionOnlyAssemblyResolve()
        {
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve -= CurrentDomain_ReflectionOnlyAssemblyResolve;
        }

        private Assembly CurrentDomain_ReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
        {
            Assembly asm = Assembly.ReflectionOnlyLoad(args.Name);
            return asm;
        }

        /// <summary>
        /// 加载模块配置，改这个函数来改模块的导入方式（配置文件，文件夹...等等）
        /// </summary>
        private void Load()
        {
            List<ModuleInfo> moduleInfos = new List<ModuleInfo>();
            // 从/Interface目录获取所有模块接口
            string imodule = typeof(IModule).FullName;
            var imodulePaths = GetInterfaceAssemblies();
            foreach (var imodulePath in imodulePaths)
            {
                Assembly imoduleAsm = Assembly.ReflectionOnlyLoadFrom(imodulePath);

                foreach (var type in imoduleAsm.GetTypes())
                {
                    // 过滤不是接口的类型
                    if (!type.IsInterface) continue;

                    if (type.FindInterfaces((t, c) => t.FullName == imodule, null).Length > 0)
                        moduleInfos.Add(new ModuleInfo { Interface = type.FullName, InterfaceFile = imoduleAsm.CodeBase });
                }
            }// foreach imodulePaths

            // 从/Modules目录获取所有模块
            string initializeAttrName = typeof(ModuleInitializeAttribute).FullName;
            var modulePaths = GetModuleAssemblies();
            foreach (var modulePath in modulePaths)
            {
                Assembly moduleAsm = Assembly.ReflectionOnlyLoadFrom(modulePath);
                foreach (var type in moduleAsm.GetTypes())
                {
                    // 过滤不是类的类型
                    if (type.IsInterface || type.IsValueType) continue;
                    // 过滤没有继承IModule的类型
                    if (type.FindInterfaces((t, c) => t.FullName == imodule, null).Length == 0) continue;
                    // 查找所实现的模块接口并完善模块信息
                    var selectedModuleInfos = moduleInfos.Where(m => type.FindInterfaces((t, c) => t.FullName == m.Interface, null).Length > 0).ToArray();

                    if (selectedModuleInfos.Length == 0) continue;
                    if (selectedModuleInfos.Length > 1)
                        throw new MultiModuleInterfaceException(type.FullName, selectedModuleInfos.Select(m => m.Interface));

                    ModuleInfo moduleInfo = selectedModuleInfos[0];
                    if (moduleInfo.Type != null)
                        throw new ConflictModuleException(moduleInfo.Type, type.FullName, moduleInfo.Interface);

                    MethodInfo initializeMethodInfo = type.GetMethod("Initialize", BindingFlags.Public | BindingFlags.Instance);
                    CustomAttributeData initializeAttr = initializeMethodInfo.GetCustomAttributesData().FirstOrDefault(a => a.AttributeType.FullName == initializeAttrName);

                    moduleInfo.Type = type.FullName;
                    moduleInfo.File = moduleAsm.CodeBase;
                    if (initializeAttr != null && initializeAttr.ConstructorArguments.Count > 0)
                    {
                        var dependencies = initializeAttr.ConstructorArguments[0].Value as IEnumerable<CustomAttributeTypedArgument>;
                        if (dependencies != null)
                        {
                            var dependencyNames = dependencies.Select(d => (d.Value as Type).FullName);
                            moduleInfo.Dependencies.AddRange(dependencyNames);
                        }
                    }
                }
            }// foreach modulePaths

            // 筛选可用的模块
            var availableModuleInfos = moduleInfos.Where(m => m.Type != null);
            _items.AddRange(availableModuleInfos);
        }

        private List<string> GetInterfaceAssemblies()
        {
            List<string> asms = new List<string>();

            string baseDirPath = AppDomain.CurrentDomain.BaseDirectory;
            string imodulesDirPath = Path.Combine(baseDirPath, "Interfaces");

            if (Directory.Exists(imodulesDirPath))
                asms.AddRange(Directory.GetFiles(imodulesDirPath, "*.dll", SearchOption.TopDirectoryOnly));

            asms.AddRange(Directory.GetFiles(baseDirPath, "*.interface.dll", SearchOption.TopDirectoryOnly));

            return asms;
        }

        private List<string> GetModuleAssemblies()
        {
            List<string> asms = new List<string>();

            string baseDirPath = AppDomain.CurrentDomain.BaseDirectory;
            string modulesDirPath = Path.Combine(baseDirPath, "Modules");

            if (Directory.Exists(modulesDirPath))
                asms.AddRange(Directory.GetFiles(modulesDirPath, "*.dll", SearchOption.TopDirectoryOnly));

            asms.AddRange(Directory.GetFiles(baseDirPath, "*.module.dll", SearchOption.TopDirectoryOnly));

            return asms;
        }

        private void ValidateUnique()
        {
            List<string> moduleNames = _items.Select(m => m.Type).ToList();
            List<string> duplicateNames = moduleNames.Where(m => moduleNames.Count(m2 => m2 == m) > 1).ToList();

            if (duplicateNames.Count > 0) throw new DuplicateModulesException(duplicateNames);
        }

        private void ValidateDependencyGraph()
        {
            SolveDependencies(_items);
        }

        /// <summary>
        /// Checks for cyclic dependencies, by calling the dependencysolver. 
        /// </summary>
        /// <param name="modules">the.</param>
        /// <returns></returns>
        private static string[] SolveDependencies(IEnumerable<ModuleInfo> modules)
        {
            if (modules == null) throw new System.ArgumentNullException("modules");

            ModuleDependencySolver solver = new ModuleDependencySolver();

            foreach (ModuleInfo data in modules)
            {
                solver.AddModule(data.Interface);

                if (data.Dependencies != null)
                {
                    foreach (var dependency in data.Dependencies)
                    {
                        solver.AddDependency(data.Interface, dependency);
                    }
                }
            }

            if (solver.ModuleCount > 0)
            {
                return solver.Solve();
            }

            return new string[0];
        }

        private static string GetFileAbsoluteUri(string filePath)
        {
            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Host = String.Empty;
            uriBuilder.Scheme = Uri.UriSchemeFile;
            uriBuilder.Path = Path.GetFullPath(filePath);
            Uri fileUri = uriBuilder.Uri;

            return fileUri.ToString();
        }
    }
}
