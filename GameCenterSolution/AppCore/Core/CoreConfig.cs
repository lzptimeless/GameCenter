using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AppCore
{
    /// <summary>
    /// 核心配置
    /// </summary>
    public class CoreConfig : ConfigBase
    {
        public CoreConfig()
        {
            InterfaceFolder = "Interfaces";
            ModuleFolder = "Modules";
            UIFolder = "UIs";
            AppName = Assembly.GetEntryAssembly().GetName().Name;
        }

        public const string AppNameName = "AppName";
        /// <summary>
        /// 应用程序名，用以确定用户数据文件夹名，默认为主程序集名
        /// </summary>
        public string AppName { get; private set; }

        public const string InterfaceFolderName = "InterfaceFolder";
        /// <summary>
        /// 接口文件夹路径，null或Empty或'/'或'\'表示与根目录相同
        /// </summary>
        public string InterfaceFolder { get; private set; }

        public const string ModuleFolderName = "ModuleFolder";
        /// <summary>
        /// 模块文件夹路径，null或Empty或'/'或'\'表示与根目录相同
        /// </summary>
        public string ModuleFolder { get; private set; }

        public const string UIFolderName = "UIFolder";
        /// <summary>
        /// UI文件夹路径，null或Empty或'/'或'\'表示与根目录相同
        /// </summary>
        public string UIFolder { get; private set; }

        /// <summary>
        /// 获取接口文件夹的完整路径，如果路径与更目录相同则返回null
        /// </summary>
        /// <returns>接口文件夹的完整路径，如果路径与更目录相同则返回null</returns>
        public string GetInterfaceFolderFullPath()
        {
            string folder = TrimSlash(InterfaceFolder);
            if (string.IsNullOrEmpty(folder)) return null;

            string baseDir = TrimSlash(AppDomain.CurrentDomain.BaseDirectory);
            string fullPath = Path.Combine(baseDir, folder);

            if (Path.IsPathRooted(folder))
            {
                if (string.Equals(folder, baseDir, StringComparison.OrdinalIgnoreCase))
                    return null;

                return folder;
            }

            return fullPath;
        }

        /// <summary>
        /// 获取模块文件夹的完整路径，如果路径与更目录相同则返回null
        /// </summary>
        /// <returns>模块文件夹的完整路径，如果路径与更目录相同则返回null</returns>
        public string GetModuleFolderFullPath()
        {
            string folder = TrimSlash(ModuleFolder);
            if (string.IsNullOrEmpty(folder)) return null;

            string baseDir = TrimSlash(AppDomain.CurrentDomain.BaseDirectory);
            string fullPath = Path.Combine(baseDir, folder);

            if (Path.IsPathRooted(folder))
            {
                if (string.Equals(folder, baseDir, StringComparison.OrdinalIgnoreCase))
                    return null;

                return folder;
            }

            return fullPath;
        }

        /// <summary>
        /// 获取UI文件夹的完整路径，如果路径与更目录相同则返回null
        /// </summary>
        /// <returns>UI文件夹的完整路径，如果路径与更目录相同则返回null</returns>
        public string GetUIFolderFullPath()
        {
            string folder = TrimSlash(UIFolder);
            if (string.IsNullOrEmpty(folder)) return null;

            string baseDir = TrimSlash(AppDomain.CurrentDomain.BaseDirectory);
            string fullPath = Path.Combine(baseDir, folder);

            if (Path.IsPathRooted(folder))
            {
                if (string.Equals(folder, baseDir, StringComparison.OrdinalIgnoreCase))
                    return null;

                return folder;
            }

            return fullPath;
        }

        internal void Load(string path)
        {
            base.Load(path, false);
        }

        internal void Save(string path)
        {
            base.Save(path, false);
        }

        protected override DeserializePropertyResults TryDeserializeProperty(string propertyName, string text)
        {
            switch (propertyName)
            {
                case AppNameName:
                    {
                        text = text?.Trim();
                        if (!string.IsNullOrEmpty(text))
                        {
                            if (CheckPath(text)) AppName = text;
                            else return DeserializePropertyResults.Fail($"Has invalid path chars:{text}");
                        }

                        return DeserializePropertyResults.Complete();
                    }
                case InterfaceFolderName:
                    {
                        text = text?.Trim();
                        if (!string.IsNullOrEmpty(text))
                        {
                            if (CheckPath(text)) InterfaceFolder = text;
                            else return DeserializePropertyResults.Fail($"Path has invalid chars:{text}");
                        }

                        return DeserializePropertyResults.Complete();
                    }
                case ModuleFolderName:
                    {
                        text = text?.Trim();
                        if (!string.IsNullOrEmpty(text))
                        {
                            if (CheckPath(text)) ModuleFolder = text;
                            else return DeserializePropertyResults.Fail($"Path has invalid chars:{text}");
                        }

                        return DeserializePropertyResults.Complete();
                    }
                case UIFolderName:
                    {
                        text = text?.Trim();
                        if (!string.IsNullOrEmpty(text))
                        {
                            if (CheckPath(text)) UIFolder = text;
                            else return DeserializePropertyResults.Fail($"Path has invalid chars:{text}");
                        }

                        return DeserializePropertyResults.Complete();
                    }
                default:
                    return DeserializePropertyResults.Fail($"Property not exists:{propertyName}");
            }
        }

        protected override SerializePropertyResults TrySerializeProperty(string propertyName)
        {
            switch (propertyName)
            {
                case AppNameName:
                    return SerializePropertyResults.Complete(AppName ?? string.Empty);
                case InterfaceFolderName:
                    return SerializePropertyResults.Complete(InterfaceFolder ?? string.Empty);
                case ModuleFolderName:
                    return SerializePropertyResults.Complete(ModuleFolder ?? string.Empty);
                case UIFolderName:
                    return SerializePropertyResults.Complete(UIFolder ?? string.Empty);
                default:
                    return SerializePropertyResults.Fail($"Property not exists:{propertyName}");
            }
        }

        /// <summary>
        /// 验证路径格式是否正确
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns>true:路径正确，false:路径含有非法字符</returns>
        private bool CheckPath(string path)
        {
            if (path == null) throw new ArgumentNullException("path");

            var validChars = Path.GetInvalidPathChars();
            foreach (var c in path)
            {
                if (validChars.Contains(c)) return false;
            }

            return true;
        }

        /// <summary>
        /// 移除开头和结尾的'\'和'/'符号
        /// </summary>
        /// <param name="value">字符串</param>
        /// <returns>移除了'\'和'/'符号之后的结果</returns>
        private string TrimSlash(string value)
        {
            return value?.Trim('/', '\\');
        }
    }
}
