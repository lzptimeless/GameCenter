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
        private void OnGameUpdated(Game game, GameUpdatedFields fields)
        {
            GameUpdatedEventData data = new GameUpdatedEventData(game, fields);
            Volatile.Read(ref GameUpdated)?.Invoke(this, data);
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
                        Game game = new Game();
                        game.Name = name;
                        game.ID = new BattleNetGameID(name);

                        var gameInfo = new BattleNetGameInfo();
                        gameInfo.Name = name;
                        game.PlatformGameInfo = gameInfo;

                        bool isAdded = false;
                        lock (_syncObj)
                        {
                            isAdded = _games.Add(game, true);
                            game = game.DeepClone();
                        }

                        if (isAdded)
                        {
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
                    game = _games[id]?.DeepClone();
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
