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
            _games = new List<Game>();
        }

        private readonly object _syncObj;
        private List<Game> _games;

        public GamePlatformFlags PlatformFlag
        {
            get { return GamePlatformFlags.BattleNet; }
        }

        public void Scan(CancellationToken cancelToken)
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

                    lock (_syncObj) _games.Add(game);
                }
            }
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

            BattleNetGameInfo gameInfo = game.PlatformGameInfo as BattleNetGameInfo;
            if (gameInfo == null)
                throw new ArgumentException($"gameInfo({gameInfo}) can not convert to {typeof(BattleNetGameInfo).FullName}");

            throw new NotImplementedException();
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
