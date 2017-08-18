using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace AppCore
{
    public abstract class ConfigBase
    {
        #region classes
        protected struct SerializePropertyResults
        {
            public SerializePropertyResults(string text)
            {
                Success = true;
                Text = text;
                Message = null;
            }

            public SerializePropertyResults(bool success, string message)
            {
                Success = success;
                Text = null;
                Message = message;
            }

            public bool Success { get; set; }
            public string Text { get; set; }
            public string Message { get; set; }
        }

        protected struct DeserializePropertyResults
        {
            public DeserializePropertyResults(bool success)
            {
                Success = success;
                Message = null;
            }

            public DeserializePropertyResults(bool success, string message)
            {
                Success = success;
                Message = message;
            }

            public bool Success { get; set; }
            public string Message { get; set; }
        }
        #endregion

        #region fields

        #endregion

        #region properties

        #endregion

        #region public methods

        #endregion

        #region private methods
        protected abstract SerializePropertyResults TrySerializeProperty(string propertyName);

        protected abstract DeserializePropertyResults TryDeserializeProperty(string propertyName, string text);

        protected void Save(string configPath)
        {
            if (string.IsNullOrEmpty(configPath)) throw new InvalidOperationException("ConfigPath is empty.");

            ILogger logger = Application.Current.ToIApp().Logger;
            XDocument doc = new XDocument();
            var root = new XElement("Config");
            doc.Add(root);
            var properties = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var propInfo in properties)
            {
                var results = TrySerializeProperty(propInfo.Name);
                if (!results.Success)
                {
                    logger.Warn($"Serialize {propInfo.Name} failed: {results.Message}");
                    continue;
                }
                root.Add(new XElement(propInfo.Name, results.Text ?? string.Empty));
            }

            try
            {
                using (FileStream fs = new FileStream(configPath, FileMode.Create, FileAccess.Write, FileShare.Read))
                using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    doc.Save(sw);
                }
            }
            catch (Exception saveEx)
            {
                logger.Warn($"Save config failed.\r\n{saveEx}");
            }
        }

        protected void Load(string configPath)
        {
            if (string.IsNullOrEmpty(configPath)) throw new InvalidOperationException("ConfigPath is empty.");

            ILogger logger = Application.Current.ToIApp().Logger;
            if (!File.Exists(configPath))
            {
                logger.Warn($"Can not read config, file not exist: {configPath}");
                return;
            }

            string content;
            try
            {
                content = File.ReadAllText(configPath, Encoding.UTF8);
            }
            catch (Exception readEx)
            {
                logger.Warn($"Read config failed.\r\n{readEx}");
                return;
            }

            if (!string.IsNullOrEmpty(content))
            {
                var properties = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
                var propDic = new Dictionary<string, PropertyInfo>();
                foreach (var propInfo in properties)
                {
                    propDic.Add(propInfo.Name, propInfo);
                }

                try
                {
                    XDocument doc = XDocument.Parse(content);
                    foreach (var element in doc.Root.Elements())
                    {
                        string propName = element.Name.LocalName;
                        string propText = element.Value;
                        if (propDic.ContainsKey(propName))
                        {
                            var results = TryDeserializeProperty(propName, propText);
                            if (!results.Success)
                            {
                                logger.Warn($"Deserialize {propName}={propText} failed: {results.Message}");
                                continue;
                            }
                        }
                    }// foreach (var element in doc.Root.Elements())
                }
                catch (Exception deserializeEx)
                {
                    logger.Warn($"Deserialize config failed.\r\n{deserializeEx}");
                }
            }// if (!string.IsNullOrEmpty(content))
        }
        #endregion
    }
}
