using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AppCore
{
    internal class UIFactory : IUIFactory
    {
        #region classes
        private class UIAssemblyInfo
        {
            public UIAssemblyInfo(string fullName, string codeBase)
            {
                FullName = fullName;
                CodeBase = codeBase;
            }
            public string FullName { get; private set; }
            public string CodeBase { get; private set; }
            public Assembly LoadedAssembly { get; set; }
        }

        private class UIPartInfo
        {
            public UIPartInfo(UIAssemblyInfo asmInfo, string typeFullName)
            {
                AssemblyInfo = asmInfo;
                TypeFullName = typeFullName;
            }
            public UIAssemblyInfo AssemblyInfo { get; private set; }
            public string TypeFullName { get; private set; }
        }
        #endregion

        public UIFactory()
        {
            _pages = new List<UIPartInfo>();
        }

        private const string HomeName = "Home";
        private const string CaptionBarName = "CaptionBar";
        private const string TopBarName = "TopBar";
        private const string BottomBarName = "BottomBar";
        private const string LefBarName = "LeftBar";
        private const string RightBarName = "RightBar";

        private UIPartInfo _captionBar;
        private UIPartInfo _topBar;
        private UIPartInfo _bottomBar;
        private UIPartInfo _leftBar;
        private UIPartInfo _rightBar;

        private UIPartInfo _homePage;
        private List<UIPartInfo> _pages;

        public void Load()
        {
            // 收集UI对象所在dll，这些dll以*.ui.dll的方式命名，可能存在与根目录和/UIs目录
            // Collect dlls that contains UI objects, these dll named by "*.ui.dll",
            // and location can be root directory or /UIs
            List<string> uiAsmPaths = new List<string>();
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string uiDir = Core.Instance.Config.GetUIFolderFullPath();
            uiAsmPaths.AddRange(Directory.GetFiles(baseDir, "*.ui.dll"));
            if (!string.IsNullOrEmpty(uiDir) && Directory.Exists(uiDir))
                uiAsmPaths.AddRange(Directory.GetFiles(uiDir, "*.ui.dll"));

            string pageTypeFullName = typeof(IPage).FullName;
            string barTypeFullName = typeof(IBar).FullName;
            foreach (var asmPath in uiAsmPaths)
            {
                Assembly asm = Assembly.ReflectionOnlyLoadFrom(asmPath);
                UIAssemblyInfo asmInfo = new UIAssemblyInfo(asm.FullName, asm.CodeBase);
                var allTypes = asm.GetTypes();
                foreach (var type in allTypes)
                {
                    UIPartInfo uiPartInfo = new UIPartInfo(asmInfo, type.FullName);
                    if (type.FindInterfaces((m, filterCriteria) => m.FullName == pageTypeFullName, null).Length > 0)
                    {
                        if (!_pages.Any(p => p.TypeFullName == type.FullName)) _pages.Add(uiPartInfo);
                        else throw new InvalidOperationException($"Already exist this page: {type.FullName}");

                        if (type.Name.Equals(HomeName)) _homePage = uiPartInfo;
                    }
                    else if (type.FindInterfaces((m, filterCriteria) => m.FullName == barTypeFullName, null).Length > 0)
                    {
                        if (type.Name.Equals(CaptionBarName))
                        {
                            _captionBar = uiPartInfo;
                        }
                        else if (type.Name.Equals(TopBarName))
                        {
                            _topBar = uiPartInfo;
                        }
                        else if (type.Name.Equals(BottomBarName))
                        {
                            _bottomBar = uiPartInfo;
                        }
                        else if (type.Name.Equals(LefBarName))
                        {
                            _leftBar = uiPartInfo;
                        }
                        else if (type.Name.Equals(RightBarName))
                        {
                            _rightBar = uiPartInfo;
                        }
                    }// else if IBar
                }// foreach (var type in allTypes)
            }// foreach (var asmPath in uiAsmPaths)

            CheckPageValid();
        }

        public IBar CreateBottomBar()
        {
            if (_bottomBar == null) return null;

            IBar bar = InnerCreate(_bottomBar) as IBar;
            return bar;
        }

        public IBar CreateCaptionBar()
        {
            if (_captionBar == null) return null;

            IBar bar = InnerCreate(_captionBar) as IBar;
            return bar;
        }

        public IBar CreateLeftBar()
        {
            if (_leftBar == null) return null;

            IBar bar = InnerCreate(_leftBar) as IBar;
            return bar;
        }

        public IBar CreateRightBar()
        {
            if (_rightBar == null) return null;

            IBar bar = InnerCreate(_rightBar) as IBar;
            return bar;
        }

        public IBar CreateTopBar()
        {
            if (_topBar == null) return null;

            IBar bar = InnerCreate(_topBar) as IBar;
            return bar;
        }

        public IPage CreateHomePage()
        {
            if (_homePage == null)
                throw new InvalidOperationException("Can not found home page.");

            IPage page = InnerCreate(_homePage) as IPage;
            return page;
        }

        public IPage CreatePage(string pageTypeName)
        {
            UIPartInfo pageInfo = _pages.FirstOrDefault(p => p.TypeFullName == pageTypeName);
            if (pageInfo == null)
                throw new InvalidOperationException($"Can not found {pageTypeName}");

            IPage page = InnerCreate(pageInfo) as IPage;
            return page;
        }

        private object InnerCreate(UIPartInfo uiPartInfo)
        {
            if (uiPartInfo.AssemblyInfo.LoadedAssembly == null)
                uiPartInfo.AssemblyInfo.LoadedAssembly = GetOrLoadAssembly(uiPartInfo.AssemblyInfo.CodeBase);

            Type type = uiPartInfo.AssemblyInfo.LoadedAssembly.GetType(uiPartInfo.TypeFullName);
            object instance = Activator.CreateInstance(type);
            return instance;
        }

        private void CheckPageValid()
        {
            if (_homePage == null) throw new InvalidOperationException("Home page can not found.");
        }

        private Assembly GetOrLoadAssembly(string codeBase)
        {
            Assembly asm = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.CodeBase == codeBase);
            if (asm == null)
            {
                asm = Assembly.LoadFrom(codeBase);
            }

            return asm;
        }
    }
}
