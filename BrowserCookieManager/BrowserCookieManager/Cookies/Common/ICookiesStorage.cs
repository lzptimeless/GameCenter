using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowserCookieManager
{
    public interface ICookiesStorage
    {
        void Put(string host, string path, string name, string value, string encryptedValue, DateTime expiresUTC);
        void Delete(string host, string path, string name);
        void DeleteByHost(string host);
        void DeleteByDomain(string domain);
        Cookie Get(string host, string path, string name);
        List<Cookie> GetByHost(string host);
        List<Cookie> GetByDomain(string domain);
        void Post(string host, string path, string name, string value, string encryptedValue, DateTime expiresUTC);
    }
}
