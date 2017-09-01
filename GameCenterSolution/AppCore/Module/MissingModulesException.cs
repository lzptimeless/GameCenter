using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AppCore
{
    [Serializable]
    public class MissingModulesException : ModularityException
    {
        public MissingModulesException(IEnumerable<string> moduleInterfaces)
        {
            _moduleInterfaces.AddRange(moduleInterfaces);
        }

        protected MissingModulesException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            var moduleInterfaces = (info.GetString("ModuleInterfaces") ?? string.Empty).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            _moduleInterfaces.AddRange(moduleInterfaces);
        }

        private List<string> _moduleInterfaces = new List<string>();
        public IEnumerable<string> ModuleInterfaces
        {
            get { return _moduleInterfaces; }
        }

        public override string Message
        {
            get
            {
                return $"Missing modules: {string.Join(",", _moduleInterfaces)}";
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("ModuleInterfaces", string.Join(",", _moduleInterfaces));
        }
    }
}
