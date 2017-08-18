using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore
{
    [Serializable]
    public class CyclicDependencyFoundException : ModularityException
    {
        public CyclicDependencyFoundException()
            : base("Cyclic dependency found.", null)
        { }
    }
}
