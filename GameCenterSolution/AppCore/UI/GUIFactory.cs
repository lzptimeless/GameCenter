using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AppCore
{
    internal class GUIFactory : IGUIFactory
    {
        public GUIFactory()
        {
            _pages = new List<string>();
        }

        private string _splashScreen;
        private string _captionBar;
        private string _topBar;
        private string _bottomBar;
        private string _leftBar;
        private string _rightBar;

        private string _homePage;
        private List<string> _pages;

        public void Load()
        {
            List<string> uiAsmPaths = new List<string>();
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string uiDir = Path.Combine(baseDir, "UIs");
            uiAsmPaths.AddRange(Directory.GetFiles(baseDir, "*.ui.dll"));
            if (Directory.Exists(uiDir))
                uiAsmPaths.AddRange(Directory.GetFiles(uiDir, "*.ui.dll"));

            

            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += CurrentDomain_ReflectionOnlyAssemblyResolve;
            try
            {

            }
            finally
            {
                AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve -= CurrentDomain_ReflectionOnlyAssemblyResolve;
            }
        }

        public IBar CreateBottomBar()
        {
            if (string.IsNullOrEmpty(_bottomBar)) return null;

            IBar bar = InnerCreate(_bottomBar) as IBar;
            return bar;
        }

        public IBar CreateCaptionBar()
        {
            if (string.IsNullOrEmpty(_captionBar)) return null;

            IBar bar = InnerCreate(_captionBar) as IBar;
            return bar;
        }

        public IBar CreateLeftBar()
        {
            if (string.IsNullOrEmpty(_leftBar)) return null;

            IBar bar = InnerCreate(_leftBar) as IBar;
            return bar;
        }

        public IBar CreateRightBar()
        {
            if (string.IsNullOrEmpty(_rightBar)) return null;

            IBar bar = InnerCreate(_rightBar) as IBar;
            return bar;
        }

        public ISplashScreen CreateSplashScreen()
        {
            if (string.IsNullOrEmpty(_splashScreen)) return null;

            ISplashScreen ss = InnerCreate(_splashScreen) as ISplashScreen;
            return ss;
        }

        public IBar CreateTopBar()
        {
            if (string.IsNullOrEmpty(_topBar)) return null;

            IBar bar = InnerCreate(_topBar) as IBar;
            return bar;
        }

        public IPage CreateHomePage()
        {
            if (string.IsNullOrEmpty(_homePage))
                throw new InvalidOperationException("Can not found home page.");

            IPage page = InnerCreate(_homePage) as IPage;
            return page;
        }

        public IPage CreatePage(string pageTypeName)
        {
            if (!_pages.Contains(pageTypeName))
                throw new InvalidOperationException($"Can not found {pageTypeName}");

            IPage page = InnerCreate(pageTypeName) as IPage;
            return page;
        }

        private object InnerCreate(string name)
        {
            try
            { }
            finally
            {
            }
            return null;
        }

        private void CheckPageValid()
        {
            if (string.IsNullOrEmpty(_homePage)) 
        }

        private Assembly CurrentDomain_ReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
        {
            Assembly asm = Assembly.ReflectionOnlyLoad(args.Name);
            return asm;
        }
    }
}
