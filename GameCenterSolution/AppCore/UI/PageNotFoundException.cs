using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore
{
    public class PageNotFoundException : Exception
    {
        public PageNotFoundException(string pageName)
        {
            PageName = pageName;
        }

        protected 

        public string PageName { get; set; }
    }
}
