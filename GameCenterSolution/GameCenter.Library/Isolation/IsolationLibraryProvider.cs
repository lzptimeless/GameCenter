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
            _games = new List<Game>();
        }

        private readonly object _syncObj;
        private List<Game> _games;

        public GamePlatformFlags PlatformFlag
        {
            get { return GamePlatformFlags.Isolation; }
        }

        public void Scan(CancellationToken ct)
        {

        }

        public void Launch(GameID id)
        {
            if (id == null) throw new ArgumentNullException("id");

            Game game;
            lock (_syncObj)
            {
                game = _games.FirstOrDefault(g => g.ID == id)?.DeepClone();
            }

            if (game == null) throw new InvalidOperationException($"Can not found game:{id}");

            IsolationGameInfo gameInfo = game.PlatformGameInfo as IsolationGameInfo;
            if (gameInfo == null)
                throw new ArgumentException($"gameInfo({gameInfo}) can not convert to {typeof(IsolationGameInfo).FullName}");

            if (string.IsNullOrEmpty(gameInfo.Launcher)) throw new InvalidOperationException("Launcher is empty.");

            System.Diagnostics.Process.Start(gameInfo.Launcher);
        }
    }
}
