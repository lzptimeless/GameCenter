using AppCore;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameCenter.Library
{
    internal class BattleNetLibraryProvider : ILibraryProvider
    {
        public BattleNetLibraryProvider()
        {
            _syncObj = new object();
            _games = new GameCollection();
        }

        private readonly object _syncObj;
        private GameCollection _games;

        public GamePlatformFlags PlatformFlag
        {
            get { return GamePlatformFlags.BattleNet; }
        }

        public event EventHandler<GameAddedEventData> GameAdded;
        private void OnGameAdded(Game game)
        {
            GameAddedEventData data = new GameAddedEventData(game);
            Volatile.Read(ref GameAdded)?.Invoke(this, data);
        }

        public event EventHandler<GameRemovedEventData> GameRemoved;
        private void OnGameRemoved(Game game)
        {
            GameRemovedEventData data = new GameRemovedEventData(game);
            Volatile.Read(ref GameRemoved)?.Invoke(this, data);
        }

        public event EventHandler<GameUpdatedEventData> GameUpdated;
        private void OnGameUpdated(GameUpdatedEventData args)
        {
            Volatile.Read(ref GameUpdated)?.Invoke(this, args);
        }

        public Task ScanAsync(CancellationToken cancelToken)
        {
            return Task.Run(() =>
            {
                // Find Hearthstone
                using (RegistryKey hearthstoneKey = Open64And32NodeOnRead(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Hearthstone"))
                {
                    if (hearthstoneKey != null)
                    {
                        string name = hearthstoneKey.GetValue("DisplayName") as string;
                        string displayIcon = hearthstoneKey.GetValue("DisplayIcon") as string;

                        IconTools.IconExtractor iconExtractor = new IconTools.IconExtractor(displayIcon, 1);
                        var icon = iconExtractor.GetIcon(0);
                        var iconItems = IconTools.IconUtil.Split(icon);
                        System.Drawing.Icon maxIcon = iconItems[0];
                        int maxIconBit = IconTools.IconUtil.GetBitCount(maxIcon);
                        foreach (var iconItem in iconItems)
                        {
                            int iconItemBit = IconTools.IconUtil.GetBitCount(iconItem);
                            if (iconItemBit < maxIconBit) continue;
                            if (iconItem.Width < maxIcon.Width) continue;

                            maxIcon = iconItem;
                            maxIconBit = iconItemBit;
                        }

                        // 保存最清晰的图标到本地


                        // 释放所有临时资源
                        foreach (var iconItem in iconItems)
                        {
                            iconItem.Dispose();
                        }
                        icon.Dispose();

                        Game game = new Game();
                        game.Name = name;
                        game.ID = new BattleNetGameID(ulong.MaxValue);

                        var gameInfo = new BattleNetGameInfo();
                        gameInfo.Name = name;
                        game.PlatformGameInfo = gameInfo;

                        bool isAdded = false;
                        lock (_syncObj)
                        {
                            isAdded = _games.Add(game, true);
                            game = game.Clone() as Game;
                        }

                        if (isAdded)
                        {
                            game.SetReadOnly();
                            OnGameAdded(game);
                        }
                    }
                }
            });// Task.Run
        }

        public Task LaunchAsync(GameID id)
        {
            if (id == null) throw new ArgumentNullException("id");

            return Task.Run(() =>
            {
                Game game;
                lock (_syncObj)
                {
                    game = _games[id]?.Clone() as Game;
                }

                if (game == null) throw new InvalidOperationException($"Can not found game:{id}");

                BattleNetGameInfo gameInfo = game.PlatformGameInfo as BattleNetGameInfo;
                if (gameInfo == null)
                    throw new ArgumentException($"gameInfo({gameInfo}) can not convert to {typeof(BattleNetGameInfo).FullName}");

                throw new NotImplementedException();
            });// Task.Run
        }

        private RegistryKey Open64And32NodeOnRead(RegistryHive hive, string path)
        {
            RegistryKey node = null;
            using (RegistryKey baseKey32 = RegistryKey.OpenBaseKey(hive, RegistryView.Registry32))
            {
                node = baseKey32.OpenSubKey(path, false);
            }

            if (node != null) return node;

            using (RegistryKey baseKey64 = RegistryKey.OpenBaseKey(hive, RegistryView.Registry64))
            {
                node = baseKey64.OpenSubKey(path, false);
            }

            return node;
        }
    }
}
