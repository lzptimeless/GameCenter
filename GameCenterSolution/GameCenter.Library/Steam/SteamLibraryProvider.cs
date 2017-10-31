﻿using Microsoft.Win32;
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
    internal class SteamLibraryProvider : ILibraryProvider
    {
        public SteamLibraryProvider()
        {
            _syncObj = new object();
            _games = new List<Game>();
            _coverDownloader = new SteamCoverDownloader();
            _coverDownloader.Downloaded += _coverDownloader_Downloaded;
        }

        private readonly object _syncObj;
        private List<Game> _games;
        private SteamCoverDownloader _coverDownloader;

        public GamePlatformFlags PlatformFlag
        {
            get { return GamePlatformFlags.Steam; }
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


        public void Scan(CancellationToken cancelToken)
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
                    lock (_syncObj) _games.Add(game);

                    Int64 steamAppID = (game.ID as SteamGameID).AppID;
                    _coverDownloader.Download(steamAppID, true);

                    OnGameAdded(game);
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

            SteamGameInfo gameInfo = game.PlatformGameInfo as SteamGameInfo;
            if (gameInfo == null)
                throw new ArgumentException($"gameInfo({gameInfo}) can not convert to {typeof(SteamGameInfo).FullName}");

            System.Diagnostics.Process.Start($"steam://rungameid/{gameInfo.AppID}");
        }

        private void _coverDownloader_Downloaded(object sender, SteamCoverDownloadedArgs e)
        {
            if (e.State == SteamCoverDownloadResultStates.Successed)
            {
                SteamGameID gameID = new SteamGameID(e.AppID);
                Game game;
                lock (_syncObj)
                {
                    game = _games.FirstOrDefault(g => g.ID == gameID);
                    if (game != null)
                    {
                        game.Cover.Small = e.SmallPath;
                        game.Cover.Normal = e.NormalPath;
                    }
                }

                if (game != null) OnGameUpdated(game, GameUpdatedFields.Cover);
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
