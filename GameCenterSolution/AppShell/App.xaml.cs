using AppCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
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

            Core.Instance.Run();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Core.Instance.Shutdown();

            base.OnExit(e);
        }
    }
}
