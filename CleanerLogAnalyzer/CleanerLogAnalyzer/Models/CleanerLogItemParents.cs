using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanerLogAnalyzer.Models
{
    [Flags]
    public enum CleanerLogItemParents
    {
        CCleaner = 0x01,
        Cortex = 0x02
    }
}
