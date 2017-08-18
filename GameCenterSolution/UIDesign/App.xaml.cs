using AppCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace UIDesign
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ModulesConfigSection modulesSection = ModulesConfigSection.GetConfig();
            //var oldCulture = UIDesign.Properties.Resources.Culture;
            //UIDesign.Properties.Resources.Culture = new System.Globalization.CultureInfo("en-US");
        }
    }
}
