using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore
{
    public class ModuleCatalog
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
            Load();
            ValidateUnique();
            ValidateDependencyGraph();
            Sort();
        }

        private void Sort()
        {
            List<ModuleInfo> sortedItems = new List<ModuleInfo>();
            foreach (var itemName in SolveDependencies(_items))
            {
                ModuleInfo moduleInfo = _items.First(m => m.Name == itemName);
                sortedItems.Add(moduleInfo);
            }

            _items = sortedItems;
        }

        private void AddModule(ModuleInfo moduleInfo)
        {
            _items.Add(moduleInfo);
        }

        /// <summary>
        /// 加载模块配置，改这个函数来改模块的导入方式（配置文件，文件夹...等等）
        /// </summary>
        private void Load()
        {
            ModulesConfigSection section = ModulesConfigSection.GetConfig();

            if (section != null)
            {
                foreach (ModuleConfigElement element in section.Modules)
                {
                    IList<string> dependencies = new List<string>();

                    if (element.Dependencies.Count > 0)
                    {
                        foreach (ModuleDependencyConfigElement dependency in element.Dependencies)
                        {
                            dependencies.Add(dependency.Name);
                        }
                    }

                    string filePath = GetFileAbsoluteUri(element.File);
                    ModuleInfo moduleInfo = new ModuleInfo(element.Name, element.Type, filePath, dependencies);
                    AddModule(moduleInfo);
                }
            }
        }

        private void ValidateUnique()
        {
            List<string> moduleNames = _items.Select(m => m.Name).ToList();
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
                solver.AddModule(data.Name);

                if (data.Dependencies != null)
                {
                    foreach (string dependency in data.Dependencies)
                    {
                        solver.AddDependency(data.Name, dependency);
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
