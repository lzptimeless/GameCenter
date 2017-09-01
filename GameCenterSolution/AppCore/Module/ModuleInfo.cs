using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore
{
    internal class ModuleInfo
    {
        public ModuleInfo()
        {
            State = ModuleStates.NotStarted;
            Dependencies = new List<string>();
        }

        /// <summary>
        /// 模块实现的接口类型，继承与<see cref="IModule"/>
        /// The interface type that implement by module, it inherit from <see cref="IModule"/>
        /// </summary>
        public string Interface { get; set; }
        /// <summary>
        /// 模块类型
        /// The module type
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 模块接口所在程序集路径
        /// </summary>
        public string InterfaceFile { get; set; }
        /// <summary>
        /// 模块所在程序集路径
        /// The assembly file path that contains this module
        /// </summary>
        public string File { get; set; }
        /// <summary>
        /// 在调用<see cref="IModule.Initialize(IModule[])"/>时需要传入的模块接口类型
        /// The interface types need to be set when call <see cref="IModule.Initialize(IModule[])"/>
        /// </summary>
        public List<string> Dependencies { get; private set; }
        /// <summary>
        /// 模块初始化状态
        /// The module state about initialization
        /// </summary>
        public ModuleStates State { get; set; }
    }
}
