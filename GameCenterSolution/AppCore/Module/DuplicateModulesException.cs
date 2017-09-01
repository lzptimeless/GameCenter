using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AppCore
{
    [Serializable]
    public class DuplicateModulesException : ModularityException
    {
        public DuplicateModulesException(IEnumerable<string> moduleTypes)
        {
            _moduleTypes.AddRange(moduleTypes);
        }

        protected DuplicateModulesException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            var moduleTypes = (info.GetString("ModuleTypes") ?? string.Empty).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (moduleTypes.Length > 0) _moduleTypes.AddRange(moduleTypes);
        }

        private List<string> _moduleTypes = new List<string>();
        public IEnumerable<string> ModuleTypes
        {
            get { return _moduleTypes; }
        }

        public override string Message
        {
            get
            {
                return $"These modules duplicate: {string.Join(",", _moduleTypes)}";
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("ModuleTypes", string.Join(",", _moduleTypes));
        }
    }
}
