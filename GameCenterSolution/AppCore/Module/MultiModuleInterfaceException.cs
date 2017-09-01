using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AppCore
{
    [Serializable]
    public class MultiModuleInterfaceException : ModularityException
    {
        public MultiModuleInterfaceException(string moduleType, IEnumerable<string> moduleInterfaces)
        {
            ModuleType = moduleType;
            if (moduleInterfaces != null) _moduleInterfaces.AddRange(moduleInterfaces);
        }

        protected MultiModuleInterfaceException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ModuleType = info.GetString("ModuleType");
            string[] exports = (info.GetString("ModuleInterfaces") ?? string.Empty).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            _moduleInterfaces.AddRange(exports);
        }

        public string ModuleType { get; private set; }

        private List<string> _moduleInterfaces = new List<string>();
        public IReadOnlyList<string> ModuleInterfaces
        {
            get { return _moduleInterfaces; }
        }

        public override string Message
        {
            get
            {
                return $"{ModuleType} can not implement multi exported interfaces:{string.Join(",", _moduleInterfaces)}";
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("ModuleType", ModuleType);
            info.AddValue("ModuleInterfaces", string.Join(",", _moduleInterfaces));
        }
    }
}
