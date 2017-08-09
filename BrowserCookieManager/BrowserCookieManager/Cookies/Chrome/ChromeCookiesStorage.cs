using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowserCookieManager
{
    public class ChromeCookiesStorage : ICookiesStorage, IDisposable
    {
        #region constructors
        public ChromeCookiesStorage()
        {
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string dataPath = Path.Combine(localAppData, "Google\\Chrome\\User Data\\Default\\Cookies");
            _conn = new SQLiteConnection($"Data Source={dataPath}");
            _conn.Open();
        }
        #endregion

        #region fields
        private SQLiteConnection _conn;
        #endregion

        #region ICookiesStorage
        public void Delete(string host, string path, string name)
        {
            using (var cmd = _conn.CreateCommand())
            {
                cmd.CommandText = $"DELETE FROM cookies WHERE host_key='%{host}%' AND path='{path}' AND name='{name}'";
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteByDomain(string domain)
        {
            using (var cmd = _conn.CreateCommand())
            {
                cmd.CommandText = $"DELETE FROM cookies WHERE host_key LIKE '%{domain}%'";
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteByHost(string host)
        {
            using (var cmd = _conn.CreateCommand())
            {
                cmd.CommandText = $"DELETE FROM cookies WHERE host_key='{host}'";
                cmd.ExecuteNonQuery();
            }
        }

        public Cookie Get(string host, string path, string name)
        {
            Cookie ck = null;
            using (var cmd = _conn.CreateCommand())
            {
                cmd.CommandText = "SELECT * " +
                                  "FROM cookies " +
                                   $"WHERE host_key='{host}' AND path='{path}' AND name='{name}'";
                using (var reader = cmd.ExecuteReader(System.Data.CommandBehavior.KeyInfo))
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        ck = ReadCookie(reader);
                    }
                }// using reader
            }// using cmd

            return ck;
        }

        public List<Cookie> GetByDomain(string domain)
        {
            List<Cookie> cookies = new List<Cookie>();
            using (var cmd = _conn.CreateCommand())
            {
                cmd.CommandText = "SELECT * " +
                                  "FROM cookies " +
                                   $"WHERE host_key LIKE '%{domain}%'";
                using (var reader = cmd.ExecuteReader(System.Data.CommandBehavior.KeyInfo))
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var ck = ReadCookie(reader);
                            cookies.Add(ck);
                        }
                    }
                }// using reader
            }// using cmd

            return cookies;
        }

        public List<Cookie> GetByHost(string host)
        {
            List<Cookie> cookies = new List<Cookie>();
            using (var cmd = _conn.CreateCommand())
            {
                cmd.CommandText = "SELECT * " +
                                  "FROM cookies " +
                                   $"WHERE host_key='{host}'";
                using (var reader = cmd.ExecuteReader(System.Data.CommandBehavior.KeyInfo))
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var ck = ReadCookie(reader);
                            cookies.Add(ck);
                        }
                    }
                }// using reader
            }// using cmd

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

        #region IDisposable
        public void Dispose()
        {
            _conn.Close();
        }
        #endregion

        #region private methods
        private Cookie ReadCookie(SQLiteDataReader reader)
        {
            var ck = new Cookie();

            var originalHost = new CookieOriginalHost(reader.GetString(reader.GetOrdinal("host_key")));
            ck.Domain = originalHost.Domain;
            ck.Host = originalHost.Host;
            ck.Path = reader.GetString(reader.GetOrdinal("path"));
            ck.Name = reader.GetString(reader.GetOrdinal("name"));

            //var eValueBlob = reader.GetBlob(reader.GetOrdinal("encrypted_value"), true);
            //byte[] eValueBytes = new byte[eValueBlob.GetCount()];
            //eValueBlob.Read(eValueBytes, eValueBytes.Length, 0);

            return ck;
        }
        #endregion
    }
}
