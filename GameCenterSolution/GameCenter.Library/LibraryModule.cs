﻿using AppCore;
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
            _libraryProviders = new List<ILibraryProvider>();
            _libraryProviders.Add(new IsolationLibraryProvider());
            _libraryProviders.Add(new SteamLibraryProvider());
            _libraryProviders.Add(new BattleNetLibraryProvider());
        }

        /// <summary>
        /// 定义这个模块的事件管理器，用来集中管理这个模块的所有事件
        /// </summary>
        private IModuleEventAggregator _eventAggregator;
        private List<Game> _games;
        private CancellationTokenSource _scanToken;
        private List<ILibraryProvider> _libraryProviders;

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
                newToken.Dispose();
                return;
            }

            ThreadPool.QueueUserWorkItem(s =>
            {
                foreach (var lp in _libraryProviders)
                {
                    lp.Scan(newToken.Token);
                }
            });
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

            var libraryProvider = _libraryProviders.FirstOrDefault(p => p.PlatformFlag == id.PlatformFlag);

            if (libraryProvider == null)
                throw new InvalidOperationException($"Unspported platform({id.PlatformFlag})");

            libraryProvider.Launch(id);
        }
    }
}
