using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore
{
    public class Navigator : INavigator
    {
        internal Navigator(IPageJournalCollection pageJournalCollection, IGUIFactory guiFactory)
        {
            _pageJournalCollection = pageJournalCollection;
            _guiFactory = guiFactory;
        }

        private IPageJournalCollection _pageJournalCollection;
        private IGUIFactory _guiFactory;

        public int CanBackCount
        {
            get
            {
                return _pageJournalCollection.ActiveIndex;
            }
        }

        public int CanForwardCount
        {
            get
            {
                int count = _pageJournalCollection.Count - 1 - _pageJournalCollection.ActiveIndex;
                return count;
            }
        }

        public int CurrentIndex
        {
            get
            {
                return _pageJournalCollection.ActiveIndex;
            }
        }

        public void Back()
        {
            int activeIndex = _pageJournalCollection.ActiveIndex;
            if (activeIndex > 0)
            {
                _pageJournalCollection[activeIndex].Page.OnLeave();

                _pageJournalCollection.ActiveIndex--;
                activeIndex--;

                _pageJournalCollection[activeIndex].Page.OnComeback();
            }
        }

        public void Forward()
        {
            int activeIndex = _pageJournalCollection.ActiveIndex;
            if (activeIndex < _pageJournalCollection.Count - 1)
            {
                _pageJournalCollection[activeIndex].Page.OnLeave();

                _pageJournalCollection.ActiveIndex++;
                activeIndex++;

                _pageJournalCollection[activeIndex].Page.OnComeback();
            }
        }

        public void Home()
        {
            var homePage = _guiFactory.CreateHomePage();
            homePage.Initialize(null);
            InnerNew(homePage, null);
        }

        public void New(string pageTypeName, NavigationParameters parameters)
        {
            var page = _guiFactory.CreatePage(pageTypeName);
            page.Initialize(parameters);
            InnerNew(page, parameters);
        }

        public void Next(string pageTypeName, NavigationParameters parameters)
        {
            int removeCount = _pageJournalCollection.Count - 1 - _pageJournalCollection.ActiveIndex;
            if (removeCount > 0)
            {
                // 如果当前记录不是最后一个记录则移除当前记录之后的记录
                List<PageJournalEntry> removedEntries = _pageJournalCollection.Remove(_pageJournalCollection.ActiveIndex + 1, removeCount);
                foreach (var removedEntry in removedEntries)
                {
                    removedEntry.Page.Release();
                }
            }
            // 添加新页面
            var page = _guiFactory.CreatePage(pageTypeName);
            page.Initialize(parameters);
            var journalEntry = new PageJournalEntry(page, parameters);
            _pageJournalCollection.Append(journalEntry);
            _pageJournalCollection.ActiveIndex++;
        }

        public void Refresh()
        {
            int activeIndex = _pageJournalCollection.ActiveIndex;
            // 没有需要刷新的页面，直接退出
            if (activeIndex < 0) return;
            // 获取旧的记录和页面名
            PageJournalEntry oldJournalEntry = _pageJournalCollection[activeIndex];
            string pageTypeName = oldJournalEntry.Page.GetType().FullName;
            // 初始化新页面
            IPage newPage = _guiFactory.CreatePage(pageTypeName);
            newPage.Initialize(oldJournalEntry.Parameters);
            // 替换
            PageJournalEntry newJournalEntry = new PageJournalEntry(newPage, oldJournalEntry.Parameters);
            _pageJournalCollection[activeIndex] = newJournalEntry;
            // 释放旧页面
            oldJournalEntry.Page.Release();
        }

        private void InnerNew(IPage page, NavigationParameters parameters)
        {
            // 移除所有记录
            if (_pageJournalCollection.Count > 0)
            {
                List<PageJournalEntry> removedJournalEntries = _pageJournalCollection.Remove(0, _pageJournalCollection.Count);
                foreach (var removedJournalEntry in removedJournalEntries)
                {
                    removedJournalEntry.Page.Release();
                }
            }
            // 添加新页面
            var newJournalEntry = new PageJournalEntry(page, parameters);
            _pageJournalCollection.Append(newJournalEntry);
            // 不用设置ActiveIndex了，默认PageCollection会自动设置ActiveIndex到唯一的一个页面
        }
    }
}
