using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore
{
    public class LanguageChangedEventArgs : EventArgs
    {
        public LanguageChangedEventArgs(string languageTagFrom, string languageTagTo)
        {
            LanguageTagFrom = languageTagFrom;
            LanguageTagTo = languageTagTo;
        }

        public string LanguageTagFrom { get; private set; }
        public string LanguageTagTo { get; private set; }
    }
}
