using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore
{
    public class StringNullOrEmptyException : ArgumentException
    {
        public StringNullOrEmptyException(string paramName)
            : base($"{paramName} can not be null or empty.", paramName)
        { }
    }
}
