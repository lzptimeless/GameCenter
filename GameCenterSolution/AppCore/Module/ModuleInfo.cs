using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore
{
    public class ModuleInfo
    {
        public ModuleInfo()
            : this(null, null, null, null)
        { }

        public ModuleInfo(string name, string type, string file)
            : this(name, type, file, null)
        { }

        public ModuleInfo(string name, string type, string file, IEnumerable<string> dependencies)
        {
            Name = name;
            Type = type;
            File = file;
            Dependencies = new List<string>();
            if (dependencies != null) Dependencies.AddRange(dependencies);
        }

        public string Name { get; set; }
        public string Type { get; set; }
        /// <summary>
        /// 模块所在程序集路径，例如：file://c:/MyProject/Modules/Module1.dll
        /// </summary>
        public string File { get; set; }
        public List<string> Dependencies { get; private set; }
        public ModuleStates State { get; set; }
    }
}
