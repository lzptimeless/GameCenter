using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AppCore
{
    public class ConflictModuleException : ModularityException
    {
        public ConflictModuleException(string moduleType1, string moduleType2, string moduleInterface)
        {
            ModuleType1 = moduleType1;
            ModuleType2 = moduleType2;
            ModuleInterface = moduleInterface;
        }

        protected ConflictModuleException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ModuleType1 = info.GetString("ModuleType1");
            ModuleType2 = info.GetString("ModuleType2");
            ModuleInterface = info.GetString("ModuleInterface");
        }

        public string ModuleType1 { get; private set; }
        public string ModuleType2 { get; private set; }
        public string ModuleInterface { get; private set; }

        public override string Message
        {
            get
            {
                return $"{ModuleType1} and {ModuleType2} both implement {ModuleInterface}";
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("ModuleType1", ModuleType1);
            info.AddValue("ModuleType2", ModuleType2);
            info.AddValue("ModuleInterface", ModuleInterface);
        }
    }
}
