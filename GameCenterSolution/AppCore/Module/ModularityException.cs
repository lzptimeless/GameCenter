using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AppCore
{
    [Serializable]
    public class ModularityException : Exception
    {
        public ModularityException()
        { }

        public ModularityException(string message, Exception innerEx)
            : base(message, innerEx)
        { }

        protected ModularityException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
