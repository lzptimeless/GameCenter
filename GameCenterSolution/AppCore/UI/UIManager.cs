using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AppCore
{
    public class UIManager
    {
        public UIManager()
        {
        }

        private IGUI _gui;
        private ISplashScreen _splashScreen;
        private IGUIFactory _guiFactory;
        private INavigator _navigator;

        internal void Load()
        {
            SetReflectionOnlyAssemblyResolve();
            LoadGUI();
            ClearReflectionOnlyAssemblyResolve();
        }

        internal void StartWork()
        {
            // 加载边栏
            LoadBars();
            // 加载导航器
            LoadNavigator();
            // 进入工作页面
            _gui.GoWorkplace();
            // 启动画面已经不需要了，释放掉
            if (_splashScreen != null)
            {
                _splashScreen.Release();
                _splashScreen = null;
            }
            // 设置各个边栏
            var captionBar = _guiFactory.CreateCaptionBar();
            var topBar = _guiFactory.CreateTopBar();
            var bottomBar = _guiFactory.CreateBottomBar();
            var leftBar = _guiFactory.CreateLeftBar();
            var rightBar = _guiFactory.CreateRightBar();
            _gui.SetBars(captionBar, topBar, bottomBar, leftBar, rightBar);
            // 加载Home页面
            _navigator.Home();
        }

        internal void Release()
        {
            if (_gui.Count > 0) _gui.RemovePage(0, _gui.Count);

            if (_captionBar != null)
            {
                _captionBar.Release();
                _captionBar = null;
            }
            if (_topBar != null)
            {
                _topBar.Release();
                _topBar = null;
            }
            if (_bottomBar != null)
            {
                _bottomBar.Release();
                _bottomBar = null;
            }
            if (_leftBar != null)
            {
                _leftBar.Release();
                _leftBar = null;
            }
            if (_rightBar != null)
            {
                _rightBar.Release();
                _rightBar = null;
            }
            if (_gui != null)
            {
                _gui.Release();
                _gui = null;
            }
        }

        private void LoadSplashScreen()
        {
            List<string> uiPaths = new List<string>();
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string uiDir = Path.Combine(baseDir, "UIs");

            if (Directory.Exists(uiDir))
                uiPaths.AddRange(Directory.GetFiles(uiDir, "*.dll"));

            uiPaths.AddRange(Directory.GetFiles(baseDir, "*.ui.dll"));

            foreach (var asmPath in uiPaths)
            {
                Assembly asm = Assembly.ReflectionOnlyLoadFrom(asmPath);
            }
        }

        private void LoadGUI()
        {
            string iguiFullName = typeof(IGUI).FullName;
            Type guiType = null;
            foreach (var type in Assembly.GetEntryAssembly().GetTypes())
            {
                if (type.FindInterfaces((i, c) => i.FullName == iguiFullName, null).Length > 0)
                {
                    guiType = type;
                    break;
                }
            }

            if (guiType == null) throw new InvalidOperationException("Can not found IGUI.");

            // 创建UI
            IGUI gui = Activator.CreateInstance(guiType) as IGUI;
            // 初始化UI
            gui.Initialize(_splashScreen);
            _gui = gui;
        }

        private void LoadBars()
        { }

        private void LoadNavigator()
        {

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

        private void SetReflectionOnlyAssemblyResolve()
        {
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += CurrentDomain_ReflectionOnlyAssemblyResolve;
        }

        private void ClearReflectionOnlyAssemblyResolve()
        {
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve -= CurrentDomain_ReflectionOnlyAssemblyResolve;
        }

        private Assembly CurrentDomain_ReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
        {
            Assembly asm = Assembly.ReflectionOnlyLoad(args.Name);
            return asm;
        }
    }
}
