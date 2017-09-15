using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore
{
    /// <summary>
    /// GUI接口，代表一个用户操作界面集合
    /// </summary>
    public interface IGUI : IPageJournalCollection
    {
        /// <summary>
        /// 初始化UI
        /// </summary>
        /// <remarks>
        /// 此时<see cref="Core.ModuleManager"/>还没有初始化
        /// </remarks>
        void Initialize(object splashScreen);

        /// <summary>
        /// 关闭启动画面（如果有的话），进入工作界面
        /// </summary>
        void GoWorkplace();

        /// <summary>
        /// 设置工具栏（上下左右），可以设置为null
        /// </summary>
        /// <param name="captionBar">标题栏，设置为null会启用默认标题栏</param>
        /// <param name="topBar">上边栏</param>
        /// <param name="bottomBar">下边栏</param>
        /// <param name="leftBar">左边栏</param>
        /// <param name="rightBar">右边栏</param>
        void SetBars(IBar captionBar, IBar topBar, IBar bottomBar, IBar leftBar, IBar rightBar);

        /// <summary>
        /// 释放模块
        /// </summary>
        /// <remarks>
        /// 可以在这里处理停止当前任务，保存配置等操作
        /// </remarks>
        void Release();
    }
}
