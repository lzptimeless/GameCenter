using Microsoft.Practices.Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanerLogAnalyzer.Models
{
    public class CleanerLogItem : BindableBase
    {
        #region properties
        public string Content { get; set; }

        public CleanerLogItemParents Parents { get; set;}

        public bool CCleanerContains
        {
            get { return (Parents & CleanerLogItemParents.CCleaner) == CleanerLogItemParents.CCleaner; }
        }

        public bool CortexContains
        {
            get { return (Parents & CleanerLogItemParents.Cortex) == CleanerLogItemParents.Cortex; }
        }
        
        public string CCleanerContent
        {
            get { return CCleanerContains ? Content : string.Empty; }
        }

        public string CortexContent
        {
            get { return CortexContains ? Content : string.Empty; }
        }

        public int CCleanerRepeatCount { get; set; }

        public int CortexRepeatCount { get; set; }

        public int CortexCatagoryID { get; set; }

        public int CortexFileID { get; set; }
        #endregion
    }
}
