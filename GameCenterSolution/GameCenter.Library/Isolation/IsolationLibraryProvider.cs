using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameCenter.Library
{
    internal class IsolationLibraryProvider : ILibraryProvider
    {
        public IsolationLibraryProvider()
        {
            _syncObj = new object();
            _games = new GameCollection();
        }

        private readonly object _syncObj;
        private GameCollection _games;

        public GamePlatformFlags PlatformFlag
        {
            get { return GamePlatformFlags.Isolation; }
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

        public Task ScanAsync(CancellationToken ct)
        {
            return Task.FromResult(0);
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

                IsolationGameInfo gameInfo = game.PlatformGameInfo as IsolationGameInfo;
                if (gameInfo == null)
                    throw new ArgumentException($"gameInfo({gameInfo}) can not convert to {typeof(IsolationGameInfo).FullName}");

                if (string.IsNullOrEmpty(gameInfo.Launcher)) throw new InvalidOperationException("Launcher is empty.");

                System.Diagnostics.Process.Start(gameInfo.Launcher);
            });// Task.Run
        }
    }
}
