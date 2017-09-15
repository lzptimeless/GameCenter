using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore
{
    /// <summary>
    /// 导航记录集合，记录了导航页面和它的参数
    /// </summary>
    internal interface IPageJournalCollection : IReadOnlyCollection<PageJournalEntry>, IEnumerable<PageJournalEntry>
    {
        /// <summary>
        /// 获取或设置激活的页面索引（当前页面索引）
        /// </summary>
        int ActiveIndex { get; set; }

        /// <summary>
        /// 获取或替换导航记录
        /// </summary>
        /// <param name="index">导航记录索引</param>
        /// <returns></returns>
        PageJournalEntry this[int index] { get; set; }

        /// <summary>
        /// 添加一个导航记录到末尾
        /// </summary>
        /// <param name="page">导航记录</param>
        void Append(PageJournalEntry page);

        /// <summary>
        /// 移除页面并返回移除的所有导航记录
        /// </summary>
        /// <param name="startIndex">起始索引</param>
        /// <param name="count">移除的导航记录数</param>
        /// <returns>移除了的导航记录</returns>
        List<PageJournalEntry> Remove(int startIndex, int count);
    }
}
