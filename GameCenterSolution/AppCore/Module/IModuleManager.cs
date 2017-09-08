using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore
{
    public interface IModuleManager
    {
        /// <summary>
        /// 通过模块接口获取模块
        /// Get module by specified module interface
        /// </summary>
        /// <param name="moduleInterfaceType">Module interface, must inherit from <see cref="IModule"/></param>
        /// <returns></returns>
        IModule GetModule(Type moduleInterfaceType);

        /// <summary>
        /// 通过模块接口获取模块
        /// Get module by specified module interface
        /// </summary>
        /// <typeparam name="TModule">模块接口</typeparam>
        /// <returns></returns>
        TModule GetModule<TModule>() where TModule : IModule;

        /// <summary>
        /// 获取所有已经初始化的模块
        /// </summary>
        /// <returns></returns>
        IModule[] GetModules();

        /// <summary>
        /// 注销这个对象实例在所有模块中注册过的事件
        /// </summary>
        /// <param name="target">注册过模块事件的对象实例</param>
        void UnsubscribeEvents(object target);
    }
}
