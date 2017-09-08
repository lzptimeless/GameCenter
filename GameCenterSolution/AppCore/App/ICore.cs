using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore
{
    public interface ICore
    {
        ILogger Logger { get; }

        IModuleManager ModuleManager { get; }

        void Run();

        void Shutdown();
    }
}
