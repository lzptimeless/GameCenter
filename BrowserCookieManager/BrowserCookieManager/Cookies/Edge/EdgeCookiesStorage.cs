using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowserCookieManager
{
    public class EdgeCookiesStorage : ICookiesStorage
    {
        #region fields

        #endregion

        #region ICookiesStorage
        public void Delete(string host, string path, string name)
        {
            throw new NotImplementedException();
        }

        public void DeleteByDomain(string domain)
        {
            throw new NotImplementedException();
        }

        public void DeleteByHost(string host)
        {
            throw new NotImplementedException();
        }

        public Cookie Get(string host, string path, string name)
        {
            throw new NotImplementedException();
        }

        public List<Cookie> GetByDomain(string domain)
        {
            throw new NotImplementedException();
        }

        public List<Cookie> GetByHost(string host)
        {
            throw new NotImplementedException();
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
        private List<Cookie> GetAllCookies()
        {
            var files = GetCookieFiles();
            List<Cookie> cookies = new List<Cookie>();
            foreach (var file in files)
            {
                List<Cookie> parsedCookies;
                if (TryParseFile(file, out parsedCookies))
                {
                    cookies.AddRange(parsedCookies);
                }
            }

            return cookies;
        }

        private bool TryParseFile(string filePath, out List<Cookie> cookies)
        {
            cookies = new List<Cookie>();

            int sectionLine = 0;
            Cookie cookie = new Cookie();
            using (StreamReader reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    switch (sectionLine)
                    {
                        case 0:
                            cookie.Name = line;
                            break;
                        case 1:
                            cookie.EncryptedValue = line;
                            break;
                        case 2:
                            cookie.Host = line;
                            break;
                        case 3:
                            // 未知
                            // Unkown
                            break;
                        case 4:
                            cookie.CreationUTC = DateTime.Parse(line);
                            break;
                        case 5:
                            // 未知
                            // Unkown
                            break;
                        case 6:
                            cookie.ExpiresUTC = DateTime.Parse(line);
                            break;
                        case 7:
                        // 未知
                        // Unkown
                        default:
                            break;
                    }

                    if (line == "*")
                    {
                        cookies.Add(cookie);
                        cookie = new Cookie();
                        sectionLine = 0;
                    }
                    else
                    {
                        sectionLine++;
                    }
                }// while
            }// using

            return false;
        }

        private List<string> GetCookieFiles()
        {
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string[] edges = Directory.GetDirectories(Path.Combine(localAppData, "Packages"), "*Edge*", SearchOption.TopDirectoryOnly);
            List<string> acs = new List<string>();
            foreach (var edgeDir in edges)
            {
                string[] acsTmp = Directory.GetDirectories(edgeDir, "AC", SearchOption.TopDirectoryOnly);
                acs.AddRange(acsTmp);
            }

            List<string> numDirs = new List<string>();
            foreach (var ac in acs)
            {
                string[] numDirsTmp = Directory.GetDirectories(ac, "#!*", SearchOption.TopDirectoryOnly);
                numDirs.AddRange(numDirs);
            }

            List<string> msEdges = new List<string>();
            foreach (var numDir in numDirs)
            {
                string[] msEdigesTmp = Directory.GetDirectories(numDir, "*Edge*", SearchOption.TopDirectoryOnly);
                msEdges.AddRange(msEdigesTmp);
            }

            List<string> cookiesDirs = new List<string>();
            foreach (var msEdge in msEdges)
            {
                string[] cookiesDirsTmp = Directory.GetDirectories(msEdge, "*Cookie*", SearchOption.TopDirectoryOnly);
                cookiesDirs.AddRange(cookiesDirsTmp);
            }

            List<string> files = new List<string>();
            foreach (var cookiesDir in cookiesDirs)
            {
                string[] filesTmp = Directory.GetDirectories(cookiesDir, "*.txt");
                files.AddRange(filesTmp);
                filesTmp = Directory.GetDirectories(cookiesDir, "*.cookie");
                files.AddRange(filesTmp);
            }

            return files;
        }
        #endregion
    }
}
