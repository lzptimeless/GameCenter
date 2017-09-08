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
            _uiInfos = new List<UIInfo>();
            _uis = new List<IUI>();
        }

        private List<UIInfo> _uiInfos;
        private List<IUI> _uis;

        internal void LoadUIs()
        {
            SetReflectionOnlyAssemblyResolve();
            SearchUIInfos();
            CreateUIs();
            ClearReflectionOnlyAssemblyResolve();
        }

        internal void PreInitializeModule(IModuleManager moduleManager)
        {
            if (moduleManager == null) throw new ArgumentNullException("moduleManager");

            foreach (var ui in _uis)
            {
                ui.PreInitializeModule(moduleManager);
            }
        }

        internal void ReleaseUIs()
        {
            foreach (var ui in _uis)
            {
                ui.Release();
            }
        }

        private void CreateUIs()
        {
            foreach (var uiInfo in _uiInfos)
            {
                // 加载程序集
                Assembly asm = GetOrLoadAssembly(uiInfo.File);
                Type uiType = asm.GetType(uiInfo.Type);

                // 创建UI
                IUI ui = Activator.CreateInstance(uiType) as IUI;

                // 初始化UI
                ui.Initialize();

                // 添加到UI集合
                _uis.Add(ui);
            }
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

        private void SearchUIInfos()
        {
            string iuiFullName = typeof(IUI).FullName;
            var assemblyPaths = GetUIAssemblies();
            foreach (var asmPath in assemblyPaths)
            {
                Assembly asm = Assembly.ReflectionOnlyLoadFrom(asmPath);
                foreach (var type in asm.GetTypes())
                {
                    if (type.FindInterfaces((i, c) => i.FullName == iuiFullName, null).Length > 0)
                    {
                        _uiInfos.Add(new UIInfo { Type = type.FullName, File = asm.CodeBase });
                    }
                }
            }

            Assembly entryAsm = Assembly.GetEntryAssembly();
            foreach (var type in entryAsm.GetTypes())
            {
                if (type.FindInterfaces((i, c) => i.FullName == iuiFullName, null).Length > 0)
                {
                    _uiInfos.Add(new UIInfo { Type = type.FullName, File = entryAsm.CodeBase });
                }
            }
        }

        private List<string> GetUIAssemblies()
        {
            List<string> asms = new List<string>();

            string baseDirPath = AppDomain.CurrentDomain.BaseDirectory;
            string uisDirPath = Path.Combine(baseDirPath, "UIs");

            if (Directory.Exists(uisDirPath))
                asms.AddRange(Directory.GetFiles(uisDirPath, "*.dll", SearchOption.TopDirectoryOnly));

            asms.AddRange(Directory.GetFiles(baseDirPath, "*.ui.dll", SearchOption.TopDirectoryOnly));

            return asms;
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
