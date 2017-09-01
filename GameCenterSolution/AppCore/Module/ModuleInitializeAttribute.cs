using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ModuleInitializeAttribute : Attribute
    {
        public ModuleInitializeAttribute(Type[] dependencies)
        {
            Dependencies = dependencies;
        }

        /// <summary>
        /// 需要在调用<see cref="IModule.Initialize(IModule[])"/>时传入的模块接口类型，传入顺序
        /// 与这里定义的顺序相同
        /// Dependent module interfaces that need to be set when call <see cref="IModule.Initialize(IModule[])"/>,
        /// The sequence of parameter as same as defined at here
        /// </summary>
        public Type[] Dependencies { get; set; }
    }
}
