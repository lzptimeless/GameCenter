using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore
{
    /// <summary>
    /// UI接口，代表一个用户操作界面集合
    /// </summary>
    public interface IUI
    {
        /// <summary>
        /// 初始化UI
        /// </summary>
        /// <remarks>
        /// 此时<see cref="Core.ModuleManager"/>还没有初始化
        /// </remarks>
        void Initialize();

        /// <summary>
        /// 开始初始化模块之前
        /// </summary>
        /// <param name="moduleManager"><see cref="IModuleManager"/></param>
        /// <remarks>
        /// 在这之前<see cref="Core.ModuleManager"/>将会被赋值
        /// </remarks>
        void PreInitializeModule(IModuleManager moduleManager);

        /// <summary>
        /// 释放模块
        /// </summary>
        /// <remarks>
        /// 可以在这里处理停止当前任务，保存配置等操作
        /// </remarks>
        void Release();
    }
}
