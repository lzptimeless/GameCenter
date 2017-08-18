using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore
{
    public class ModulesManager
    {
        public ModulesManager()
        {
            _catalog = new ModuleCatalog();
        }

        private ModuleCatalog _catalog;

        public void LoadModules()
        {
            _catalog.Initialize();
            LoadModules(_catalog.Items);
        }

        private void LoadModules(IEnumerable<ModuleInfo> modules)
        {
            foreach (var moduleInfo in modules)
            {
                string assemblyPath = GetAssemblyPath(moduleInfo.File);
                IoC.Instance.AddAssemblyCatalog(assemblyPath);
            }
        }

        private string GetAssemblyPath(string filePath)
        {
            string filePerfix = "file://";
            string assemblyPath = string.Empty;

            if (filePath.StartsWith(filePerfix + "/", StringComparison.OrdinalIgnoreCase))
                assemblyPath = filePath.Substring(filePerfix.Length + 1);
            else if (filePath.StartsWith(filePerfix, StringComparison.OrdinalIgnoreCase))
                assemblyPath = filePath.Substring(filePerfix.Length);
            else assemblyPath = filePath;

            return assemblyPath;
        }
    }
}
