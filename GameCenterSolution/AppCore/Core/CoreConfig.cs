using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore
{
    public class CoreConfig : ConfigBase
    {
        public CoreConfig()
        {
            InterfaceFolder = "Interfaces";
            ModuleFolder = "Modules";
        }

        public const string InterfaceFolderName = "InterfaceFolder";
        public string InterfaceFolder { get; private set; }

        public const string ModuleFolderName = "ModuleFolder";
        public string ModuleFolder { get; private set; }

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
                default:
                    return DeserializePropertyResults.Fail($"Property not exists:{propertyName}");
            }
        }

        protected override SerializePropertyResults TrySerializeProperty(string propertyName)
        {
            switch (propertyName)
            {
                case InterfaceFolderName:
                    return SerializePropertyResults.Complete(InterfaceFolder ?? string.Empty);
                case ModuleFolderName:
                    return SerializePropertyResults.Complete(ModuleFolder ?? string.Empty);
                default:
                    return SerializePropertyResults.Fail($"Property not exists:{propertyName}");
            }
        }

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
    }
}
