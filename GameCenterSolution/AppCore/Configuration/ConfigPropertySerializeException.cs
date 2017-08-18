using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore
{
    public class ConfigPropertySerializeException : Exception
    {
        public ConfigPropertySerializeException(string propName, string errorMessage)
            : this(propName, errorMessage, null)
        { }

        public ConfigPropertySerializeException(string propName, string errorMessage, Exception innerEx)
            : base($"Serialize {propName} failed: {errorMessage}", innerEx)
        { }
    }
}
