using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowserCookieManager
{
    public class CookieOriginalHost
    {
        public CookieOriginalHost(string value)
        {
            OriginalHost = value;
        }

        public string OriginalHost { get; set; }
        public string Domain
        {
            get
            {
                string host = Host;
                if (string.IsNullOrEmpty(host)) return host;

                int domainIndex = 0;
                int lastDotIndex = -1;
                for (int i = 0; i < host.Length; i++)
                {
                    if (host[i] == '.')
                    {
                        if (lastDotIndex != -1) domainIndex = lastDotIndex + 1;

                        lastDotIndex = i;
                    }
                }

                string domain = domainIndex > 0 ? host.Substring(domainIndex) : host;
                return domain;
            }
        }
        public string Host
        {
            get
            {
                string originalHost = OriginalHost;
                if (string.IsNullOrEmpty(originalHost)) return originalHost;

                int pathIndex = originalHost.IndexOf('/');
                string host = pathIndex > 0 ? originalHost.Substring(0, pathIndex) : originalHost;

                return host;
            }
        }
        public string Path
        {
            get
            {
                string originalHost = OriginalHost;
                if (string.IsNullOrEmpty(originalHost)) return originalHost;

                int pathIndex = originalHost.IndexOf('/');
                string path = pathIndex > 0 ? originalHost.Substring(pathIndex) : "/";

                return path;
            }
        }
    }
}
