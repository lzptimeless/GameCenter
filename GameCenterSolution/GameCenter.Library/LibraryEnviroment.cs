using AppCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCenter.Library
{
    internal static class LibraryEnviroment
    {
        private static ModuleEnviroment _moduleEnviroment;
        public static ModuleEnviroment ModuleEnviroment
        {
            get
            {
                if (_moduleEnviroment == null)
                {
                    _moduleEnviroment = Core.Instance.ModuleManager.GetModuleEnviroment<ILibrary>();
                }

                return _moduleEnviroment;
            }
        }
    }
}
