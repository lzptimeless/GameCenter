using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore
{
    public class PageJournalActiveEntryArgs : EventArgs
    {
        public PageJournalActiveEntryArgs(PageJournalEntry entry)
        {
            ActiveEntry = entry;
        }

        /// <summary>
        /// 可能为null
        /// May be null
        /// </summary>
        public PageJournalEntry ActiveEntry { get; private set; }
    }
}
