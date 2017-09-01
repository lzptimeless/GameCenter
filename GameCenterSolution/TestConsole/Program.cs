using AppCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            ModuleManager moduleManager = new ModuleManager();
            moduleManager.LoadModules();
            
            Console.ReadKey();
        }
    }
}
