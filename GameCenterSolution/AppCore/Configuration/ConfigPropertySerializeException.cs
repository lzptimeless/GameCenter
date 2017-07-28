using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore
{
    public class ConfigPropertySerializeException : Exception
    {
        public ConfigPropertySerializeException(string propName, object value, string errorMessage)
            : this(propName, value, errorMessage, null)
        { }

        public ConfigPropertySerializeException(string propName, object value, string errorMessage, Exception innerEx)
            : base($"Serialize {propName} = {value} failed: {errorMessage}", innerEx)
        { }
    }
}
