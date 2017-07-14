using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AppCore
{
    public class AppConfig : ConfigBase
    {
        static AppConfig()
        {
            _configPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "App.config");
        }

        private AppConfig()
        {
        }

        #region fields
        private static readonly string _configPath;
        #endregion

        #region properties

        #region Default
        private static AppConfig _default;
        public static AppConfig Default
        {
            get
            {
                if (_default == null)
                {
                    AppConfig config = new AppConfig();
                    config.Load(_configPath);
                    Interlocked.CompareExchange(ref _default, config, null);
                }

                return _default;
            }
        }
        #endregion

        #region PreviousCCleanerLogPath
        public const string PreviousCCleanerLogPathName = "PreviousCCleanerLogPath";
        public string PreviousCCleanerLogPath { get; set; }
        #endregion

        #region PreviousCortexCleanerLogPath
        public const string PreviousCortexCleanerLogPathName = "PreviousCortexCleanerLogPath";
        public string PreviousCortexCleanerLogPath { get; set; }
        #endregion

        #endregion

        #region private
        protected override bool TrySerializeProperty(string propertyName, object value, out string text)
        {
            text = string.Empty;
            switch (propertyName)
            {
                case PreviousCCleanerLogPathName:
                    text = (string)value;
                    return true;
                case PreviousCortexCleanerLogPathName:
                    text = (string)value;
                    return true;
                default:
                    return false;
            }
        }

        protected override bool TryDeserializeProperty(string propertyName, string text, out object value)
        {
            value = null;
            switch (propertyName)
            {
                case PreviousCCleanerLogPathName:
                    value = text;
                    return true;
                case PreviousCortexCleanerLogPathName:
                    value = text;
                    return true;
                default:
                    return false;
            }
        }
        #endregion
    }
}
