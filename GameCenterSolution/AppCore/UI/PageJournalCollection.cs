using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AppCore
{
    public class PageJournalCollection : IPageJournalCollection
    {
        /// <summary>
        /// <see cref="ActiveIndex"/>不指向任何一个页面时的空值
        /// The empty value of <see cref="ActiveIndex"/>
        /// </summary>
        public const int EmptyIndex = -1;

        private List<PageJournalEntry> _history = new List<PageJournalEntry>();
        private int _activeIndex = EmptyIndex;

        public PageJournalEntry this[int index]
        {
            get
            {
                if (index < 0 || index > _history.Count - 1)
                    throw new ArgumentOutOfRangeException("index", $"index out of range: PageCount={_history.Count}");

                return _history[index];
            }

            set
            {
                if (value == null) throw new ArgumentNullException("value");

                if (index < 0 || index > _history.Count - 1)
                    throw new ArgumentOutOfRangeException("index", $"index out of range: PageCount={_history.Count}");

                if (_history[index] != value)
                {
                    _history[index] = value;
                    if (index == _activeIndex) OnActiveEntryChanged(value);
                }
            }
        }

        public int ActiveIndex
        {
            get { return _activeIndex; }

            set
            {
                if (value != EmptyIndex && value > _history.Count - 1)
                    throw new ArgumentOutOfRangeException("ActiveIndex", $"ActiveIndex out of range: PageCount={_history.Count}");

                if (_history.Count == 0 && value != EmptyIndex)
                    throw new ArgumentOutOfRangeException("ActiveIndex", $"ActiveIndex out of range: PageCount={_history.Count}");

                if (_activeIndex != value)
                {
                    var oldActiveEntry = ActiveEntry;

                    _activeIndex = value;
                    OnActiveIndexChanged(value);

                    if (oldActiveEntry != ActiveEntry) OnActiveEntryChanged(ActiveEntry);
                }
            }
        }

        public PageJournalEntry ActiveEntry
        {
            get
            {
                if (_activeIndex == EmptyIndex) return null;

                return _history[_activeIndex];
            }

            set
            {
                int index = EmptyIndex;
                if (value != null)
                {
                    index = _history.IndexOf(value);
                    if (index < 0) throw new ArgumentException("value not exist in collection.");
                }

                ActiveIndex = index;
            }
        }

        public int Count
        {
            get { return _history.Count; }
        }

        public event EventHandler<PageJournalActiveIndexArgs> ActiveIndexChanged;
        private void OnActiveIndexChanged(int activeIndex)
        {
            var args = new PageJournalActiveIndexArgs(activeIndex);
            Volatile.Read(ref ActiveIndexChanged)?.Invoke(this, args);
        }

        public event EventHandler<PageJournalActiveEntryArgs> ActiveEntryChanged;
        private void OnActiveEntryChanged(PageJournalEntry entry)
        {
            var args = new PageJournalActiveEntryArgs(entry);
            Volatile.Read(ref ActiveEntryChanged)?.Invoke(this, args);
        }

        public void Append(PageJournalEntry page)
        {
            if (page == null) throw new ArgumentNullException("page");

            _history.Add(page);

            if (_history.Count == 1) ActiveIndex = 0;
        }

        public List<PageJournalEntry> Remove(int startIndex, int count)
        {
            if (startIndex < 0 || startIndex > _history.Count - 1)
                throw new ArgumentOutOfRangeException("index", $"index out of range: PageCount={_history.Count}");

            if (startIndex + count > _history.Count)
                throw new ArgumentOutOfRangeException("count", $"The items want to remove are not exist: PageCount={_history.Count}, startIndex={startIndex}, count={count}");

            // 如果移除了当前激活页面则重置当前页面为空白页
            // If ActiveIndex in the remove range, reset it to EmptyIndex
            if (_activeIndex >= startIndex && _activeIndex <= startIndex + count - 1)
            {
                ActiveIndex = EmptyIndex;
            }

            List<PageJournalEntry> removeItems = new List<PageJournalEntry>();
            for (int i = startIndex; i < count; i++)
            {
                removeItems.Add(_history[i]);
            }

            _history.RemoveRange(startIndex, count);

            // 若有需要，更新当前页面的索引
            // If need, update ActiveEntry index
            if (_activeIndex > startIndex + count - 1)
            {
                ActiveIndex = _activeIndex - count;
            }

            return removeItems;
        }

        public IEnumerator<PageJournalEntry> GetEnumerator()
        {
            return _history.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _history.GetEnumerator();
        }
    }
}
