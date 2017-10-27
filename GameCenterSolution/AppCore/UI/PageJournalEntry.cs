using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore
{
    public class PageJournalEntry
    {
        public PageJournalEntry(IPage page, NavigationParameters parameters)
        {
            Page = page;
            Parameters = parameters;
        }

        public IPage Page { get; private set; }
        public NavigationParameters Parameters { get; private set; }
    }
}
