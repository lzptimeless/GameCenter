using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AppCore
{
    public class ModuleEnviroment
    {
        public ModuleEnviroment(string asmFullName, string moduleUserDataFolder)
        {
            if (asmFullName == null) throw new ArgumentNullException("moduleAsm");

            CheckFullPath(moduleUserDataFolder, "moduleUserDataFolder");

            _moduleAssemblyFullName = asmFullName;
            _moduleUserDataFolder = moduleUserDataFolder;
        }

        private string _moduleAssemblyFullName;
        private string _moduleUserDataFolder;

        /// <summary>
        /// 获取模块用户数据文件夹完整路径，不会返回空
        /// </summary>
        /// <param name="create">如果文件夹不存在，true：创建，false：不创建</param>
        /// <returns>模块用户数据文件夹完整路径</returns>
        public string GetUserDataFolder(bool create)
        {
            CheckCallingAssembly(Assembly.GetCallingAssembly());

            if (create && !Directory.Exists(_moduleUserDataFolder))
                Directory.CreateDirectory(_moduleUserDataFolder);

            return _moduleUserDataFolder;
        }

        private void CheckFullPath(string path, string pathName)
        {
            if (path == null) throw new ArgumentNullException("path", $"{pathName} is null.");

            path = path.Trim();
            if (string.IsNullOrEmpty(path)) throw new ArgumentException($"{pathName} is empty.");

            Char[] invalidChars = Path.GetInvalidPathChars();
            foreach (var c in path)
            {
                if (invalidChars.Contains(c))
                    throw new ArgumentException($"{pathName} contains invalid path chars:{path}");
            }

            if (!Path.IsPathRooted(path))
                throw new ArgumentException($"{pathName} is not a full path:{path}");
        }

        private void CheckCallingAssembly(Assembly callingAsm)
        {
            if (callingAsm == null) throw new ArgumentNullException("callingAsm");

            if (callingAsm.FullName != _moduleAssemblyFullName)
                throw new InvalidOperationException($"{callingAsm.FullName} can not call {typeof(ModuleEnviroment).Name} for {_moduleAssemblyFullName}");
        }
    }
}
