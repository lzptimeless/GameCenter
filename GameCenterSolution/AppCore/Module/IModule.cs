using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore
{
    /// <summary>
    /// 模块接口，模块事件可通过<see cref="ModuleEventAggregator"/>来实现
    /// </summary>
    public interface IModule
    {
        /// <summary>
        /// 初始化模块，并传入通过<see cref="ModuleInitializeAttribute"/>定义的已经初始化的模块
        /// </summary>
        /// <param name="dependencies">通过<see cref="ModuleInitializeAttribute"/>定义的这
        /// 个模块初始化需要的依赖模块，如果没有定义则传入空数组</param>
        /// <remarks>
        /// 这个操作应该尽量减少时间消耗
        /// </remarks>
        void Initialize(IModule[] dependencies);

        /// <summary>
        /// 释放模块
        /// </summary>
        /// <remarks>
        /// 可以在这里处理停止当前任务，保存配置等操作
        /// </remarks>
        void Release();

        /// <summary>
        /// 取消指定对象实例的所有事件订阅
        /// Unsubscribe all event that request by target.
        /// </summary>
        /// <param name="target">订阅了事件的对象实例，The target have requested events.</param>
        void UnsubscribeEvents(object target);
    }
}
