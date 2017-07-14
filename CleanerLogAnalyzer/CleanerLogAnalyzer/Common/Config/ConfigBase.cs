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
        #region fields

        #endregion

        #region properties

        #endregion

        #region public methods
        
        #endregion

        #region private methods
        protected abstract bool TrySerializeProperty(string propertyName, object value, out string text);

        protected abstract bool TryDeserializeProperty(string propertyName, string text, out object value);

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
                object value = propInfo.GetValue(this);
                string text;
                if (!TrySerializeProperty(propInfo.Name, value, out text))
                {
                    logger.Warn($"Serialize {propInfo.Name}={value} failed.");
                    continue;
                }
                root.Add(new XElement(propInfo.Name, text));
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
                            object value;
                            if (!TryDeserializeProperty(propName, propText, out value))
                            {
                                logger.Warn($"Deserialize {propName}={propText} failed.");
                                continue;
                            }
                            propDic[propName].SetValue(this, value);
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
