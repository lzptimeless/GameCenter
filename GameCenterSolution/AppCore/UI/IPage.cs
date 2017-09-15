using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore
{
    /// <summary>
    /// 页面接口
    /// The page interface
    /// </summary>
    public interface IPage
    {
        /// <summary>
        /// 从其他页面导航到这个页面时，初始化页面
        /// When navigate from other page, initialize this page
        /// </summary>
        /// <param name="parameters">导航参数, Navigation parameters</param>
        /// <remarks>这个函数肯定会调用</remarks>
        void Initialize(NavigationParameters parameters);

        /// <summary>
        /// 离开这个页面
        /// Leave from this page
        /// </summary>
        /// <remarks>这个函数可能不会调用，如果页面在首次显示后就不需要了会直接调用<see cref="Release"/></remarks>
        void OnLeave();

        /// <summary>
        /// 再度回到这个页面
        /// Comeback to this page
        /// </summary>
        /// <remarks>这个函数可能不会调用，如果页面在首次显示后就不需要了会直接调用<see cref="Release"/></remarks>
        void OnComeback();

        /// <summary>
        /// 不再需要这个页面了，释放页面
        /// No need this page anymore, release this page
        /// </summary>
        /// <remarks>这个函数肯定会调用</remarks>
        void Release();
    }
}
