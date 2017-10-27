using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore
{
    public class PageJournalActiveIndexArgs:EventArgs
    {
        public PageJournalActiveIndexArgs(int activeIndex)
        {
            ActiveIndex = activeIndex;
        }

        public int ActiveIndex { get; private set; }
    }
}
