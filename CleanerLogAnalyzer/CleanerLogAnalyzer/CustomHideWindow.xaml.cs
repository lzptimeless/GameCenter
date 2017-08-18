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

namespace CleanerLogAnalyzer
{
    /// <summary>
    /// Interaction logic for CustomHideWindow.xaml
    /// </summary>
    public partial class CustomHideWindow : Window
    {
        public CustomHideWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ExtensionsTextBox.Text = string.Join(",", AppConfig.Default.CustomHideFileExtensions);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            AppConfig cfg = AppConfig.Default;
            IEnumerable<string> extensions = (ExtensionsTextBox.Text ?? string.Empty).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            extensions = extensions.Select(p => p.Trim()).Where(p => !string.IsNullOrEmpty(p));
            cfg.CustomHideFileExtensions.Clear();
            cfg.CustomHideFileExtensions.AddRange(extensions);
            cfg.Save();
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
