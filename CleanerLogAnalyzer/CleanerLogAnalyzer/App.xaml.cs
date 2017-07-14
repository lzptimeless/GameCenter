using AppCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace CleanerLogAnalyzer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, IApp
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // 初始化IApp.Logger
            string loggerPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "CleanerLogAnalyzer.log");
            _logger = new FileLogger(loggerPath, 1000);

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            // 释放IApp.Logger
            _logger.Dispose();
        }

        #region IApp
        #region Logger
        private FileLogger _logger;

        public ILogger Logger
        {
            get { return _logger; }
        }
        #endregion
        #endregion
    }
}
