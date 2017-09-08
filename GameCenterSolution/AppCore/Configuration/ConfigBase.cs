using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace AppCore
{
    /// <summary>
    /// 配置文件基础类，此类允许在配置文件序列化或反序列化失败时能够忽略错误继续使用
    /// Config basic class, it can ignore serialize/deserialize error
    /// </summary>
    public abstract class ConfigBase
    {
        #region classes
        protected struct SerializePropertyResults
        {
            public bool Success { get; private set; }
            public string Text { get; private set; }
            public string Message { get; private set; }

            public static SerializePropertyResults Complete(string text)
            {
                return new SerializePropertyResults { Success = true, Text = text };
            }

            public static SerializePropertyResults Fail(string message)
            {
                return new SerializePropertyResults { Success = false, Message = message };
            }
        }

        protected struct DeserializePropertyResults
        {
            public bool Success { get; private set; }
            public string Message { get; private set; }

            public static DeserializePropertyResults Complete()
            {
                return new DeserializePropertyResults { Success = true };
            }

            public static DeserializePropertyResults Fail(string message)
            {
                return new DeserializePropertyResults { Success = false, Message = message };
            }
        }
        #endregion

        #region fields

        #endregion

        #region properties
        #region RootName
        protected virtual string RootName
        {
            get { return "Config"; }
        }
        #endregion
        #endregion

        #region public methods

        #endregion

        #region private methods
        protected abstract SerializePropertyResults TrySerializeProperty(string propertyName);

        protected abstract DeserializePropertyResults TryDeserializeProperty(string propertyName, string text);

        protected void Save(string configPath, bool ignoreError)
        {
            string rootName = RootName;
            if (string.IsNullOrWhiteSpace(rootName)) throw new InvalidOperationException("RootName is empty.");
            if (string.IsNullOrEmpty(configPath)) throw new InvalidOperationException("ConfigPath is empty.");

            ILogger logger = Core.Instance.Logger;
            XDocument doc = new XDocument();
            var root = new XElement(rootName);
            doc.Add(root);
            var properties = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var propInfo in properties)
            {
                var results = TrySerializeProperty(propInfo.Name);
                if (!results.Success)
                {
                    if (ignoreError)
                    {
                        logger.Warn($"Serialize {propInfo.Name} failed: {results.Message}");
                        continue;
                    }
                    else throw new ConfigPropertySerializeException(propInfo.Name, results.Message);
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
                if (ignoreError) logger.Warn($"Save config failed.\r\n{saveEx}");
                else throw;
            }
        }

        protected void Load(string configPath, bool ignoreError)
        {
            if (string.IsNullOrEmpty(configPath)) throw new InvalidOperationException("ConfigPath is empty.");

            ILogger logger = Core.Instance.Logger;
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
                if (ignoreError)
                {
                    logger.Warn($"Read config failed.\r\n{readEx}");
                    return;
                }
                else throw;
            }

            if (!string.IsNullOrEmpty(content))
            {
                var properties = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
                var propDic = new Dictionary<string, PropertyInfo>();
                foreach (var propInfo in properties)
                {
                    propDic.Add(propInfo.Name, propInfo);
                }

                XDocument doc;
                try
                {
                    doc = XDocument.Parse(content);
                }
                catch (Exception parseEx)
                {
                    if (ignoreError)
                    {
                        logger.Warn($"Parse config file failed.\r\n{parseEx}");
                        return;
                    }
                    else throw;
                }

                if (doc.Root == null) return;

                foreach (var element in doc.Root.Elements())
                {
                    string propName = element.Name.LocalName;
                    string propText = element.Value;
                    if (propDic.ContainsKey(propName))
                    {
                        var results = TryDeserializeProperty(propName, propText);
                        if (!results.Success)
                        {
                            if (ignoreError)
                            {
                                logger.Warn($"Deserialize {propName}={propText} failed: {results.Message}");
                                continue;
                            }
                            else throw new ConfigPropertyDeserializeException(propName, propText, results.Message);
                        }
                    }
                }// foreach (var element in doc.Root.Elements())
            }// if (!string.IsNullOrEmpty(content))
        }
        #endregion
    }
}
