using AppCore;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace GameCenter.Library
{
    /// <summary>
    /// 模块必须继承自己定义的接口<see cref="ILibrary"/>，这个接口继承自<see cref="IModule"/>
    /// </summary>
    public class LibraryModule : ILibrary
    {
        public LibraryModule()
        {
            // 初始化事件管理器
            _eventAggregator = new ModuleEventAggregator();
            _games = new List<Game>();
        }

        /// <summary>
        /// 定义这个模块的事件管理器，用来集中管理这个模块的所有事件
        /// </summary>
        private IModuleEventAggregator _eventAggregator;
        private List<Game> _games;
        private CancellationTokenSource _scanToken;

        /// <summary>
        /// 利用事件管理器实现<see cref="ILibrary.GameAddedEvent"/>，这个事件继承自
        /// <see cref="PubSubEvent{TPayload}"/>，这个自定义事件具有与委托相似的功能，
        /// 除此之外添加了更多利于集中管理的接口
        /// </summary>
        public GameAddedEvent GameAddedEvent
        {
            get { return _eventAggregator.GetEvent<GameAddedEvent>(); }
        }

        public GameRemovedEvent GameRemovedEvent
        {
            get { return _eventAggregator.GetEvent<GameRemovedEvent>(); }
        }

        public GameUpdatedEvent GameUpdatedEvent
        {
            get { return _eventAggregator.GetEvent<GameUpdatedEvent>(); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dependencies"></param>
        /// <remarks>
        /// 通过<see cref="ModuleInitializeAttribute"/>定义依赖模块，在这个函数被调用的时候
        /// 会按照定义顺序传入，框架会保证这些依赖的模块已经初始化好了
        /// </remarks>
        // [ModuleInitialize(new[] { typeof(IEnhancement) })]
        public void Initialize(IModule[] dependencies)
        {
            Console.WriteLine($"Library initialize.");
            StartScan();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// 在这里实现模块任务终止，保存配置等操作
        /// </remarks>
        public void Release()
        {
            Console.WriteLine("Library release.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <remarks>
        /// 利用事件管理器实现<see cref="IModule.UnsubscribeEvents(object)"/>
        /// </remarks>
        public void UnsubscribeEvents(object target)
        {
            _eventAggregator.Unsubscribe(target);
        }

        public void StartScan()
        {
            CancellationTokenSource newToken = new CancellationTokenSource();
            if (Interlocked.CompareExchange(ref _scanToken, newToken, null) != null)
            {
                // 正在扫描，直接退出
                newToken.Cancel();
                return;
            }

            ThreadPool.QueueUserWorkItem(s => InnerScan(newToken.Token));
        }

        public void StopScan()
        {
            CancellationTokenSource token = Volatile.Read(ref _scanToken);
            if (token != null && !token.IsCancellationRequested)
            {
                token.Cancel();
            }

            Interlocked.CompareExchange(ref _scanToken, null, token);
        }

        public List<Game> GetGames()
        {
            List<Game> items = null;

            lock (_games)
            {
                items = _games.Select(g => g.DeepClone()).ToList();
            }

            return items;
        }

        public void LaunchGame(GameID id)
        {
            if (id == null) throw new ArgumentNullException("id");

            Game game = null;
            lock (_games)
            {
                var gameTmp = _games.FirstOrDefault(g => g.ID == id);
                game = gameTmp != null ? gameTmp.DeepClone() : null;
            }

            if (game == null) throw new InvalidOperationException($"Game({id}) not found.");

            switch (id.PlatformMark)
            {
                case GamePlatformMarks.Self:
                    LaunchSelfGame(game.PlatformGameInfo);
                    break;
                case GamePlatformMarks.BattleNet:
                    LaunchBattleNetGame(game.PlatformGameInfo);
                    break;
                case GamePlatformMarks.Steam:
                    LaunchSteamGame(game.PlatformGameInfo);
                    break;
                default:
                    throw new InvalidOperationException($"Unspported platform({id.PlatformMark})");
            }
        }

        private void InnerScan(CancellationToken cancelToken)
        {
            ScanBattleNetGames(cancelToken);
            if (!cancelToken.IsCancellationRequested) ScanSteamGames(cancelToken);
            if (!cancelToken.IsCancellationRequested) ScanSelfGames(cancelToken);
        }

        private void ScanBattleNetGames(CancellationToken cancelToken)
        {
            // Find Hearthstone
            using (RegistryKey hearthstoneKey = Open64And32NodeOnRead(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Hearthstone"))
            {
                if (hearthstoneKey != null)
                {
                    string name = hearthstoneKey.GetValue("DisplayName") as string;
                    Game game = new Game();
                    game.Name = name;
                    game.ID = new BattleNetGameID(name);

                    var gameInfo = new BattleNetGameInfo();
                    gameInfo.Name = name;
                    game.PlatformGameInfo = gameInfo;

                    lock (_games) _games.Add(game);
                }
            }
        }

        private void ScanSteamGames(CancellationToken cancelToken)
        {
            string steamDirectory = null;
            // Find steam
            using (RegistryKey steamKey = Open64And32NodeOnRead(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam"))
            {
                if (steamKey != null)
                {
                    string displayIcon = steamKey.GetValue("DisplayIcon") as string;
                    if (!string.IsNullOrEmpty(displayIcon))
                    {
                        steamDirectory = Path.GetDirectoryName(displayIcon);
                    }
                }
            }

            if (string.IsNullOrEmpty(steamDirectory))
            {
                // Not found steam directory.
                return;
            }

            // Find games
            string appsDirectory = Path.Combine(steamDirectory, "steamapps");
            foreach (var appInfoPath in Directory.EnumerateFiles(appsDirectory, "*.acf"))
            {
                Game game = TryParseSteamGame(appInfoPath);
                if (game != null)
                {
                    lock (_games) _games.Add(game);
                }
            }
        }

        private Game TryParseSteamGame(string appInfoPath)
        {
            Game game = null;
            string name = null;
            Int64 appID = 0;
            Regex appIDRegex = new Regex(@"""appid""\s+""(?<id>\d+)""");
            Regex nameRegex = new Regex(@"""name""\s+""(?<name>.+)""");
            using (StreamReader r = new StreamReader(appInfoPath))
            {
                while (!r.EndOfStream)
                {
                    string line = r.ReadLine();
                    if (appID == 0)
                    {
                        Match mh = appIDRegex.Match(line);
                        if (mh.Success) appID = Int64.Parse(mh.Groups["id"].Value);
                    }

                    if (string.IsNullOrEmpty(name))
                    {
                        Match mh = nameRegex.Match(line);
                        if (mh.Success) name = mh.Groups["name"].Value;
                    }

                    if (!string.IsNullOrEmpty(name) && appID != 0)
                    {
                        game = new Game();
                        game.ID = new SteamGameID(appID);
                        game.Name = name;

                        SteamGameInfo gameInfo = new SteamGameInfo();
                        gameInfo.AppID = appID;
                        game.PlatformGameInfo = gameInfo;

                        break;
                    }
                }
            }

            return game;
        }

        private void ScanSelfGames(CancellationToken cancelToken)
        { }

        private RegistryKey Open64And32NodeOnRead(RegistryHive hive, string path)
        {
            using (RegistryKey baseKey32 = RegistryKey.OpenBaseKey(hive, RegistryView.Registry32))
            {
                RegistryKey node = baseKey32.OpenSubKey(path, false);
                if (node == null)
                {
                    using (RegistryKey baseKey64 = RegistryKey.OpenBaseKey(hive, RegistryView.Registry64))
                    {
                        node = baseKey32.OpenSubKey(path, false);
                    }
                }
                return node;
            }
        }

        private void LaunchBattleNetGame(PlatformGameInfo gameInfo)
        {
            if (gameInfo == null) throw new ArgumentNullException("gameInfo");
            BattleNetGameInfo battleNetGameInfo = gameInfo as BattleNetGameInfo;
            if (battleNetGameInfo == null)
                throw new ArgumentException($"gameInfo({gameInfo}) can not convert to {typeof(BattleNetGameInfo).FullName}");

            throw new NotImplementedException();
        }

        private void LaunchSteamGame(PlatformGameInfo gameInfo)
        {
            if (gameInfo == null) throw new ArgumentNullException("gameInfo");
            SteamGameInfo steamGameInfo = gameInfo as SteamGameInfo;
            if (steamGameInfo == null)
                throw new ArgumentException($"gameInfo({gameInfo}) can not convert to {typeof(SteamGameInfo).FullName}");

            System.Diagnostics.Process.Start($"steam://rungameid/{steamGameInfo.AppID}");
        }

        private void LaunchSelfGame(PlatformGameInfo gameInfo)
        {
            if (gameInfo == null) throw new ArgumentNullException("gameInfo");

            SelfGameInfo selfGameInfo = gameInfo as SelfGameInfo;
            if (selfGameInfo == null)
                throw new ArgumentException($"gameInfo({gameInfo}) can not convert to {typeof(SelfGameInfo).FullName}");

            if (string.IsNullOrEmpty(selfGameInfo.Launcher)) throw new InvalidOperationException("Launcher is empty.");

            System.Diagnostics.Process.Start(selfGameInfo.Launcher);
        }
    }
}
