using AppCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace AppShell
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            Core.Instance.Run();
        }

        private System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            AssemblyName asmName = new AssemblyName(args.Name);
            Assembly asmExist = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName == args.Name);
            if (asmExist != null) return asmExist;

            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string asmFileName = asmName.Name + ".dll";
            string interfaceFilePath = Path.Combine(basePath, "Interfaces", asmFileName);

            if (File.Exists(interfaceFilePath)) return Assembly.LoadFrom(interfaceFilePath);

            string moduleFilePath = Path.Combine(basePath, "Modules", asmFileName);

            if (File.Exists(moduleFilePath)) return Assembly.LoadFrom(moduleFilePath);

            return null;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Core.Instance.Shutdown();

            base.OnExit(e);
        }
    }
}
