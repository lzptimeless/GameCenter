using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace BrowserCookieManager
{
    public class IECookiesStorage : ICookiesStorage
    {
        #region inner classes
        private struct IterateCookieFileOption
        {
            public IterateCookieFileOption(bool breakOp, bool delete)
            {
                Break = breakOp;
                Delete = delete;
            }

            public bool Break { get; set; }
            public bool Delete { get; set; }
        }
        #endregion

        #region fields

        #endregion

        #region ICookiesStorage
        public void Delete(string host, string path, string name)
        {
            IterateCookieFiles((file) =>
            {
                List<Cookie> cookies;
                using (var cookieFile = new CookieFileReader(file.FullName))
                {
                    var originalHost = cookieFile.GetOriginalHost();
                    if (!string.Equals(host, originalHost.Host, StringComparison.OrdinalIgnoreCase)) return new IterateCookieFileOption(false, false);

                    cookies = cookieFile.GetCookies();
                }

                foreach (var cookie in cookies.ToArray())
                {
                    if (name == cookie.Name &&
                        string.Equals(path, cookie.Path, StringComparison.OrdinalIgnoreCase))
                    {
                        cookies.Remove(cookie);
                    }
                }

                if (cookies.Count > 0)
                {
                    throw new NotImplementedException();
                }
                else return new IterateCookieFileOption(false, true);

                //return false;
            });
        }

        public void DeleteByDomain(string domain)
        {
            IterateCookieFiles((file) =>
            {
                using (var cookieFile = new CookieFileReader(file.FullName))
                {
                    var originalHost = cookieFile.GetOriginalHost();
                    if (string.Equals(domain, originalHost.Domain, StringComparison.OrdinalIgnoreCase)) return new IterateCookieFileOption(false, true);
                }

                return new IterateCookieFileOption(false, false);
            });
        }

        public void DeleteByHost(string host)
        {
            IterateCookieFiles((file) =>
            {
                using (var cookieFile = new CookieFileReader(file.FullName))
                {
                    var originalHost = cookieFile.GetOriginalHost();
                    if (string.Equals(host, originalHost.Host, StringComparison.OrdinalIgnoreCase)) return new IterateCookieFileOption(false, true);
                }

                return new IterateCookieFileOption(false, false);
            });
        }

        public Cookie Get(string host, string path, string name)
        {
            Cookie cookie = null;
            IterateCookieFiles(file =>
            {
                using (var cookieFile = new CookieFileReader(file.FullName))
                {
                    var originalHost = cookieFile.GetOriginalHost();
                    if (!string.Equals(host, originalHost.Host, StringComparison.OrdinalIgnoreCase)) return new IterateCookieFileOption(false, false);

                    foreach (var ck in cookieFile.GetCookies())
                    {
                        if (string.Equals(path, ck.Path, StringComparison.OrdinalIgnoreCase) &&
                            string.Equals(name, ck.Name))
                        {
                            cookie = ck;
                            return new IterateCookieFileOption(true, false);
                        }
                    }
                }

                return new IterateCookieFileOption(false, false);
            });

            return cookie;
        }

        public List<Cookie> GetByDomain(string domain)
        {
            List<Cookie> cookies = new List<Cookie>();
            IterateCookieFiles(file =>
            {
                using (var cookieFile = new CookieFileReader(file.FullName))
                {
                    var originalHost = cookieFile.GetOriginalHost();
                    if (!string.Equals(domain, originalHost.Domain, StringComparison.OrdinalIgnoreCase)) return new IterateCookieFileOption(false, false);

                    cookies.AddRange(cookieFile.GetCookies());
                }

                return new IterateCookieFileOption(false, false);
            });

            return cookies;
        }

        public List<Cookie> GetByHost(string host)
        {
            List<Cookie> cookies = new List<Cookie>();
            IterateCookieFiles(file =>
            {
                using (var cookieFile = new CookieFileReader(file.FullName))
                {
                    var originalHost = cookieFile.GetOriginalHost();
                    if (!string.Equals(host, originalHost.Host, StringComparison.OrdinalIgnoreCase)) return new IterateCookieFileOption(false, false);

                    cookies.AddRange(cookieFile.GetCookies());
                }

                return new IterateCookieFileOption(false, false);
            });

            return cookies;
        }

        public void Post(string host, string path, string name, string value, string encryptedValue, DateTime expiresUTC)
        {
            throw new NotImplementedException();
        }

        public void Put(string host, string path, string name, string value, string encryptedValue, DateTime expiresUTC)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region private methods
        private void IterateCookieFiles(Func<FileInfo, IterateCookieFileOption> taskFunc)
        {
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string dir1Path = Path.Combine(localAppData, "Microsoft\\Windows\\INetCookies");
            string dir2Path = Path.Combine(localAppData, "Microsoft\\Windows\\INetCache");
            foreach (var dirPath in new[] { dir1Path, dir2Path })
            {
                var dir = new DirectoryInfo(dirPath);
                List<FileInfo> deleteFiles = new List<FileInfo>();
                bool breakOp = false;
                try
                {
                    foreach (var cookieFile in dir.EnumerateFiles("*.*", SearchOption.TopDirectoryOnly))
                    {
                        if (string.Equals(cookieFile.Extension, ".txt", StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(cookieFile.Extension, ".cookie", StringComparison.OrdinalIgnoreCase))
                        {
                            var option = taskFunc(cookieFile);
                            if (option.Delete) deleteFiles.Add(cookieFile);
                            if (option.Break)
                            {
                                breakOp = true;
                                break;
                            }
                        }
                    }// foreach cookiesDir
                }
                catch (SecurityException)
                { }
                catch (UnauthorizedAccessException)
                { }

                try
                {
                    foreach (var cookieFile in deleteFiles)
                    {
                        cookieFile.Delete();
                    }
                }
                catch (SecurityException)
                { }
                catch (UnauthorizedAccessException)
                { }

                if (breakOp) break;
            }// foreach dirXPath
        }
        #endregion
    }
}
