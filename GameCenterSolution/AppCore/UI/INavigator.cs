using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore
{
    public interface INavigator
    {
        /// <summary>
        /// 当前页面在历史记录中的索引，以0起始
        /// </summary>
        int CurrentIndex { get; }

        /// <summary>
        /// 可以返回的页面数量
        /// </summary>
        int CanBackCount { get; }

        /// <summary>
        /// 可以前进的页面数量
        /// </summary>
        int CanForwardCount { get; }

        /// <summary>
        /// 清空当前历史记录和移除当前页面，进入主页
        /// </summary>
        void Home();

        /// <summary>
        /// 清空当前历史记录和移除当前页面，再导航到新的页面
        /// </summary>
        /// <param name="parameters"></param>
        void New(string pageTypeName, NavigationParameters parameters);

        /// <summary>
        /// 将当前页面加入到历史记录，并导航到下一个页面
        /// </summary>
        /// <param name="parameters"></param>
        void Next(string pageTypeName, NavigationParameters parameters);

        /// <summary>
        /// 刷新当前页面，本质上是移除当前页面，并用一个新的页面替代
        /// </summary>
        void Refresh();

        /// <summary>
        /// 回到上一个页面，如果没有上一个页面也不会出错
        /// </summary>
        void Back();

        /// <summary>
        /// 前进到下一个页面，如果没有下一个页面也不会出错
        /// </summary>
        void Forward();
    }
}
