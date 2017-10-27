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

        private IUI _ui;
        private ISplashScreen _splashScreen;
        private IBar _captionBar;
        private IBar _topBar;
        private IBar _bottomBar;
        private IBar _leftBar;
        private IBar _rightBar;
        private IUIFactory _uiFactory;
        private INavigator _navigator;

        internal void Load()
        {
            string iuiFullName = typeof(IUI).FullName;
            string isplashScreenFullName = typeof(ISplashScreen).FullName;
            Type uiType = null, splashScreenType = null;
            foreach (var type in Assembly.GetEntryAssembly().GetTypes())
            {
                if (type.FindInterfaces((i, c) => i.FullName == iuiFullName, null).Length > 0)
                    uiType = type;
                else if (type.FindInterfaces((i, c) => i.FullName == isplashScreenFullName, null).Length > 0)
                    splashScreenType = type;

                if (uiType != null && splashScreenType != null) break;
            }

            if (uiType == null) throw new InvalidOperationException("Can not found IUI.");

            _ui = Activator.CreateInstance(uiType) as IUI;

            if (splashScreenType != null) _splashScreen = Activator.CreateInstance(splashScreenType) as ISplashScreen;
            _ui.Initialize(_splashScreen);

            var uiFactory = new UIFactory();
            uiFactory.Load();
            _uiFactory = uiFactory;

            _navigator = new Navigator(_ui.PageJournals, _uiFactory);
        }

        internal void StartWork()
        {
            // 进入工作页面
            _ui.GoWorkplace();
            // 启动画面已经不需要了，释放掉
            if (_splashScreen != null)
            {
                _splashScreen.Release();
                _splashScreen = null;
            }
            // 设置各个边栏
            _captionBar = _uiFactory.CreateCaptionBar();
            _topBar = _uiFactory.CreateTopBar();
            _bottomBar = _uiFactory.CreateBottomBar();
            _leftBar = _uiFactory.CreateLeftBar();
            _rightBar = _uiFactory.CreateRightBar();
            _ui.SetBars(_captionBar, _topBar, _bottomBar, _leftBar, _rightBar);
            // 加载Home页面
            _navigator.Home();
        }

        internal void Release()
        {
            if (_splashScreen != null)
            {
                _splashScreen.Release();
                _splashScreen = null;
            }
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
            if (_ui != null)
            {
                _ui.Release();
                _ui = null;
            }
        }
    }
}
