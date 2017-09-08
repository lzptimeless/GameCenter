using AppCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AppShell
{
    /// <summary>
    /// Interaction logic for Shell.xaml
    /// </summary>
    public partial class Shell : Window, IUI
    {
        public Shell()
        {
            InitializeComponent();
        }

        public void Initialize()
        {
            Application.Current.MainWindow = this;
            Show();
        }

        public void PreInitializeModule(IModuleManager moduleManager)
        {
            
        }

        public void Release()
        {
            
        }
    }
}
