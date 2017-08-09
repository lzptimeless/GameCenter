using AppCore;
using CleanerLogAnalyzer.Models;
using Microsoft.Win32;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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

namespace CleanerLogAnalyzer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 防止过多的【显示分析进度】任务堆积到UI线程
        /// </summary>
        private int _showAnalyzeProgressBusy;
        private List<CleanerLogItem> _originalItems;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AppConfig cfg = AppConfig.Default;
            CCleanerLogTextBox.Text = cfg.PreviousCCleanerLogPath;
            CortexLogTextBox.Text = cfg.PreviousCortexCleanerLogPath;
        }

        private void SelectCCleanerLogButton_Click(object sender, RoutedEventArgs e)
        {
            AppConfig cfg = AppConfig.Default;
            OpenFileDialog diag = new OpenFileDialog();
            string currentPath = CCleanerLogTextBox.Text;
            if (!string.IsNullOrWhiteSpace(currentPath))
            {
                try
                {
                    string dir = System.IO.Path.GetDirectoryName(currentPath);
                    diag.InitialDirectory = dir;
                }
                catch (Exception)
                { }
            }

            if (diag.ShowDialog() == true)
            {
                CCleanerLogTextBox.Text = diag.FileName.Trim();
                cfg.PreviousCCleanerLogPath = diag.FileName.Trim();
                cfg.Save();
            }
        }

        private void SelectCortexLogButton_Click(object sender, RoutedEventArgs e)
        {
            AppConfig cfg = AppConfig.Default;
            OpenFileDialog diag = new OpenFileDialog();
            string currentPath = CortexLogTextBox.Text;
            if (!string.IsNullOrWhiteSpace(currentPath))
            {
                try
                {
                    string dir = System.IO.Path.GetDirectoryName(currentPath);
                    diag.InitialDirectory = dir;
                }
                catch (Exception)
                { }
            }

            if (diag.ShowDialog() == true)
            {
                CortexLogTextBox.Text = diag.FileName.Trim();
                cfg.PreviousCortexCleanerLogPath = diag.FileName.Trim();
                cfg.Save();
            }
        }

        private async void AnalyzeButton_Click(object sender, RoutedEventArgs e)
        {
            string ccleanerLogPath = CCleanerLogTextBox.Text;
            string cortexLogPath = CortexLogTextBox.Text;
            if (string.IsNullOrWhiteSpace(ccleanerLogPath) || string.IsNullOrWhiteSpace(cortexLogPath))
            {
                MessageBox.Show("CCleaner和Cortex的日志路径不能为空。", Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            AnalyzeButton.IsEnabled = false;
            ProgressTitle.Text = "分析日志";
            ProgressAnimation.IsIndeterminate = true;
            ProgressHost.Visibility = Visibility.Visible;
            ShowProgressMessage("开始分析...");
            try
            {
                var items = await Task.Run(() => GetLogItems(ccleanerLogPath, cortexLogPath));
                _originalItems = items;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), Title, MessageBoxButton.OK, MessageBoxImage.Error);
                _originalItems = null;
            }

            ShowProgressMessage("分析完成.");
            await Task.Delay(100);
            UpdateDataGridItems();
            ProgressTitle.Text = string.Empty;
            ShowProgressMessage(string.Empty);
            ProgressAnimation.IsIndeterminate = false;
            ProgressHost.Visibility = Visibility.Hidden;
            AnalyzeButton.IsEnabled = true;
        }

        private void UpdateDataGridItems()
        {
            var realItems = new List<CleanerLogItem>();
            bool isHideSameItems = HideSameItemCheckBox.IsChecked == true;
            bool isHideCCleaner = HideCCleanerCheckBox.IsChecked == true;
            bool isHideCortex = HideCortexCheckBox.IsChecked == true;
            if (_originalItems != null)
            {
                foreach (var item in _originalItems)
                {
                    if (isHideSameItems && item.CortexContains && item.CCleanerContains) continue;
                    if (isHideCCleaner && (item.Parents & ~CleanerLogItemParents.CCleaner) == 0) continue;
                    if (isHideCortex && (item.Parents & ~CleanerLogItemParents.Cortex) == 0) continue;

                    realItems.Add(item);
                }
            }

            ItemsDataGrid.ItemsSource = realItems;
        }

        private bool ShowAnalyzeProgressIdle()
        {
            return Volatile.Read(ref _showAnalyzeProgressBusy) == 0;
        }

        private void ShowProgressMessage(string message)
        {
            if (Interlocked.CompareExchange(ref _showAnalyzeProgressBusy, 1, 0) == 0)
                Dispatcher.BeginInvoke(new Action<string>(DisplayProgressMessage), message);
        }

        private void DisplayProgressMessage(string message)
        {
            ProgressTextBlock.Text = message;
            Volatile.Write(ref _showAnalyzeProgressBusy, 0);
        }

        private List<CleanerLogItem> GetLogItems(string ccleanerLogPath, string cortexLogPath)
        {
            var items = new List<CleanerLogItem>();
            ShowProgressMessage("读取CCleaner日志...");
            var ccleanerItems = GetCCleanerLogItems(ccleanerLogPath);
            ShowProgressMessage("读取Cortex日志...");
            var cortexItems = GetCortexLogItems(cortexLogPath);
            ShowProgressMessage("排序日志项..");
            items.AddRange(ccleanerItems);
            items.AddRange(cortexItems);
            items.Sort((l, r) => string.Compare(l.Content, r.Content, true));

            ShowProgressMessage("为日志项设置属性和移除重复的日志项..");
            CleanerLogItem preItem = null;
            for (int i = 0; i < items.Count; i++)
            {
                if (preItem == null) preItem = items[i];
                else if (string.Equals(preItem.Content, items[i].Content, StringComparison.OrdinalIgnoreCase))
                {
                    preItem.Parents = preItem.Parents | items[i].Parents;
                    items.RemoveAt(i);
                    --i;
                }
                else
                {
                    preItem = items[i];
                }
            }

            return items;
        }

        private List<CleanerLogItem> GetCCleanerLogItems(string logPath)
        {
            if (string.IsNullOrEmpty(logPath)) throw new ArgumentNullException("logPath");
            if (!File.Exists(logPath)) throw new FileNotFoundException("CCleaner log not found.", logPath);

            Regex logItemRegex = new Regex(@"^(?<path>[A-Za-z]:\\.+)\t.+$", RegexOptions.IgnoreCase);
            List<CleanerLogItem> logItems = new List<CleanerLogItem>();
            using (StreamReader sw = new StreamReader(logPath))
            {
                while (!sw.EndOfStream)
                {
                    string line = (sw.ReadLine() ?? string.Empty).Trim();
                    Match mh = logItemRegex.Match(line);
                    if (mh.Success)
                    {
                        string content = mh.Groups["path"].Value;
                        logItems.Add(new CleanerLogItem { Content = content, Parents = CleanerLogItemParents.CCleaner });
                        if (ShowAnalyzeProgressIdle()) ShowProgressMessage($"读取{content}");
                    }
                }
            }

            return logItems;
        }

        private List<CleanerLogItem> GetCortexLogItems(string logPath)
        {
            if (string.IsNullOrEmpty(logPath)) throw new ArgumentNullException("logPath");
            if (!File.Exists(logPath)) throw new FileNotFoundException("Cortex log not found.", logPath);

            Regex logItemRegex = new Regex(@" (?<path>[A-Za-z]:\\.+) ", RegexOptions.IgnoreCase);
            List<CleanerLogItem> logItems = new List<CleanerLogItem>();
            using (StreamReader sw = new StreamReader(logPath))
            {
                while (!sw.EndOfStream)
                {
                    string line = (sw.ReadLine() ?? string.Empty).Trim();
                    Match mh = logItemRegex.Match(line);
                    if (mh.Success)
                    {
                        string content = mh.Groups["path"].Value;
                        logItems.Add(new CleanerLogItem { Content = content, Parents = CleanerLogItemParents.Cortex });
                        if (ShowAnalyzeProgressIdle()) ShowProgressMessage($"读取{content}");
                    }
                }
            }

            return logItems;
        }

        private void HideSameItemCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            UpdateDataGridItems();
        }

        private void HideSameItemCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateDataGridItems();
        }

        private void HideCCleanerCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CCleanerColumn.Visibility = Visibility.Collapsed;
            UpdateDataGridItems();
        }

        private void HideCCleanerCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CCleanerColumn.Visibility = Visibility.Visible;
            UpdateDataGridItems();
        }

        private void HideCortexCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CortexColumn.Visibility = Visibility.Collapsed;
            UpdateDataGridItems();
        }

        private void HideCortexCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CortexColumn.Visibility = Visibility.Visible;
            UpdateDataGridItems();
        }

        private void ItemsDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            string emptyText = "选择一项来查看它的完整内容";
            var ccs = ItemsDataGrid.SelectedCells;
            if (ccs == null)
            {
                SelectedItemDetailTextBox.Text = emptyText;
                return;
            }

            var cc = ccs.FirstOrDefault(p => p.Item is CleanerLogItem);
            if (cc != null && cc.IsValid)
            {
                if (cc.Column == CCleanerColumn) SelectedItemDetailTextBox.Text = ((CleanerLogItem)cc.Item).CCleanerContent;
                else SelectedItemDetailTextBox.Text = ((CleanerLogItem)cc.Item).CortexContent;

                if (string.IsNullOrEmpty(SelectedItemDetailTextBox.Text)) SelectedItemDetailTextBox.Text = emptyText;
            }
            else SelectedItemDetailTextBox.Text = emptyText;
        }

        private async void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<CleanerLogItem> items = ItemsDataGrid.ItemsSource as IEnumerable<CleanerLogItem>;
            if (items == null || !items.Any())
            {
                MessageBox.Show("没有需要导出的项。", Title, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SaveFileDialog saveDiag = new SaveFileDialog();
            saveDiag.Filter = "XLSX文件（*.xlsx）|*.xlsx";

            if (saveDiag.ShowDialog() != true) return;

            string savePath = saveDiag.FileName;
            ExportButton.IsEnabled = false;
            ProgressTitle.Text = "导出";
            ProgressAnimation.IsIndeterminate = true;
            ProgressHost.Visibility = Visibility.Visible;
            try
            {
                await Task.Run(() =>
                {
                    IWorkbook wb = new XSSFWorkbook();
                    ISheet st = wb.CreateSheet("Sheet1");
                    IRow r0 = st.CreateRow(0);
                    // 初始化列明
                    r0.CreateCell(0, CellType.String).SetCellValue("CCleaner");
                    r0.CreateCell(1, CellType.String).SetCellValue("Cortex");

                    int rowIndex = 1;
                    foreach (var item in items)
                    {
                        var r = st.CreateRow(rowIndex);
                        r.CreateCell(0, CellType.String).SetCellValue(item.CCleanerContent);
                        r.CreateCell(1, CellType.String).SetCellValue(item.CortexContent);

                        rowIndex++;
                        ShowProgressMessage($"写入{item.Content}");
                    }

                    ShowProgressMessage("保存文件");
                    using (var stream = new FileStream(savePath, FileMode.Create, FileAccess.Write))
                    {
                        wb.Write(stream);
                    }// using
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"导出失败。\r\n{ex}", Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            finally
            {
                ProgressTitle.Text = string.Empty;
                ProgressAnimation.IsIndeterminate = false;
                ShowProgressMessage(string.Empty);
                ProgressHost.Visibility = Visibility.Collapsed;
                ExportButton.IsEnabled = true;
            }

            MessageBox.Show("导出完成。", Title, MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
