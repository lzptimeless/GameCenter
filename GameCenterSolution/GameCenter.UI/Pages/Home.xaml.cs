using AppCore;
using GameCenter.Library;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GameCenter.UI.Pages
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : UserControl, IPage
    {
        public Home()
        {
            InitializeComponent();
        }

        public void Initialize(NavigationParameters parameters)
        {
            HomeParameters homeParams = null;
            if (parameters != null)
            {
                homeParams = parameters as HomeParameters;
                if (homeParams == null) throw new ArgumentException($"parameters is not {typeof(HomeParameters).FullName}");
            }

            ILibrary library = Core.Instance.ModuleManager.GetModule<ILibrary>();
            GamesListBox.ItemsSource = library.GetGames();
        }

        public void OnComeback()
        {
        }

        public void OnLeave()
        {
        }

        public void Release()
        {
        }
    }

    public class HomeParameters : NavigationParameters
    {

    }
}
