using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore
{
    public class ConfigPropertyDeserializeException : Exception
    {
        public ConfigPropertyDeserializeException(string propName, string text, string errorMessage, Exception innerEx)
            : base($"Deserialize {propName} = {text} failed: {errorMessage}", innerEx)
        { }

        public ConfigPropertyDeserializeException(string propName, string text, string errorMessage)
            : this(propName, text, errorMessage, null)
        { }
    }
}
