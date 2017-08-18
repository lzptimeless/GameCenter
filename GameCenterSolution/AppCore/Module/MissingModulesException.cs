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
        public MissingModulesException(IEnumerable<string> moduleNames)
        {
            _moduleNames.AddRange(moduleNames);
        }

        protected MissingModulesException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            var moduleNames = (info.GetString("ModuleNames") ?? string.Empty).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            _moduleNames.AddRange(moduleNames);
        }

        private List<string> _moduleNames = new List<string>();
        public IEnumerable<string> ModuleNames
        {
            get { return _moduleNames; }
        }

        public override string Message
        {
            get
            {
                return $"Missing modules: {string.Join(",", _moduleNames)}";
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("ModuleNames", string.Join(",", _moduleNames));
        }
    }
}
