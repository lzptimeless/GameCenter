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
using System.Collections;

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
            _pageJournals.ActiveEntryChanged += _pageJournals_ActiveEntryChanged;
        }

        private PageJournalCollection _pageJournals = new PageJournalCollection();

        public IPageJournalCollection PageJournals
        {
            get { return _pageJournals; }
        }

        public void Initialize(object splashScreen)
        {
            PageHost.Content = splashScreen;
            Application.Current.MainWindow = this;
            Show();
        }

        public void GoWorkplace()
        {
            PageHost.Content = null;
        }

        public void SetBars(IBar captionBar, IBar topBar, IBar bottomBar, IBar leftBar, IBar rightBar)
        {
            CaptionBarHost.Content = captionBar;
            TopBarHost.Content = topBar;
            BottomBarHost.Content = bottomBar;
            LeftBarHost.Content = leftBar;
            RightBarHost.Content = rightBar;
        }

        public void Release()
        {
            // 释放所有剩余页面
            if (_pageJournals.Count > 0)
            {
                var pages = _pageJournals.Remove(0, _pageJournals.Count);
                foreach (var page in pages)
                {
                    page.Page.Release();
                }
            }
        }

        private void _pageJournals_ActiveEntryChanged(object sender, PageJournalActiveEntryArgs e)
        {
            if (e.ActiveEntry == null) PageHost.Content = null;
            else PageHost.Content = e.ActiveEntry.Page;
        }
    }
}
