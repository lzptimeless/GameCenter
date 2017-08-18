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
            CustomHideFileExtensions = new List<string>();
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

        #region CustomHideFileExtensions
        public const string CustomHideFileExtensionsName = "CustomHideFileExtensions";
        public List<string> CustomHideFileExtensions { get; private set; }
        #endregion

        #endregion

        #region public
        public void Save()
        {
            Save(_configPath);
        }
        #endregion

        #region private
        protected override SerializePropertyResults TrySerializeProperty(string propertyName)
        {
            switch (propertyName)
            {
                case PreviousCCleanerLogPathName:
                    return new SerializePropertyResults(PreviousCCleanerLogPath);
                case PreviousCortexCleanerLogPathName:
                    return new SerializePropertyResults(PreviousCortexCleanerLogPath);
                case CustomHideFileExtensionsName:
                    var text = string.Join(",", CustomHideFileExtensions);
                    return new SerializePropertyResults(text);
                default:
                    return new SerializePropertyResults(false, "Not supported.");
            }
        }

        protected override DeserializePropertyResults TryDeserializeProperty(string propertyName, string text)
        {
            switch (propertyName)
            {
                case PreviousCCleanerLogPathName:
                    PreviousCCleanerLogPath = text;
                    return new DeserializePropertyResults(true);
                case PreviousCortexCleanerLogPathName:
                    PreviousCortexCleanerLogPath = text;
                    return new DeserializePropertyResults(true);
                case CustomHideFileExtensionsName:
                    IEnumerable<string> extensions = (text ?? string.Empty).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    extensions = extensions.Select(p => p.Trim()).Where(p => !string.IsNullOrEmpty(p));
                    CustomHideFileExtensions.AddRange(extensions);
                    return new DeserializePropertyResults(true);
                default:
                    return new DeserializePropertyResults(false, "Not supported.");
            }
        }
        #endregion
    }
}
