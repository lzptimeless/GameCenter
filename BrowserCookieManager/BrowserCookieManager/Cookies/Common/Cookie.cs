using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowserCookieManager
{
    public class Cookie
    {
        public string Host { get; set; }
        public string Domain { get; }
        public string Path { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string EncryptedValue { get; set; }
        public DateTime CreationUTC { get; set; }
        public DateTime ExpiresUTC { get; set; }
        public DateTime LastAccessUTC { get; set; }
    }
}
