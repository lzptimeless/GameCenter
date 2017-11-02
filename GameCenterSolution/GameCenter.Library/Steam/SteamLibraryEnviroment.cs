using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCenter.Library
{
    internal static class SteamLibraryEnviroment
    {
        private static string _steamLibraryUserDataFolder;

        public static string GetUserDataFolder(bool create)
        {
            if (string.IsNullOrEmpty(_steamLibraryUserDataFolder))
            {
                string libraryFolder = LibraryEnviroment.ModuleEnviroment.GetUserDataFolder(false);
                _steamLibraryUserDataFolder = Path.Combine(libraryFolder, "Steam");
            }

            if (create && !Directory.Exists(_steamLibraryUserDataFolder))
                Directory.CreateDirectory(_steamLibraryUserDataFolder);

            return _steamLibraryUserDataFolder;
        }

        public static string GetGameCoverFolder(bool create)
        {
            string userDataFolder = GetUserDataFolder(false);
            string gameCoverFolder = Path.Combine(userDataFolder, "GameCovers");

            if (create && !Directory.Exists(gameCoverFolder))
                Directory.CreateDirectory(gameCoverFolder);

            return gameCoverFolder;
        }

        public static string GetGameSmallCoverPath(Int64 appID)
        {
            string coverFolder = GetGameCoverFolder(false);
            string coverPath = Path.Combine(coverFolder, $"{appID}_small.jpg");

            return coverPath;
        }

        public static string GetGameNormalCoverPath(Int64 appID)
        {
            string coverFolder = GetGameCoverFolder(false);
            string coverPath = Path.Combine(coverFolder, $"{appID}_normal.jpg");

            return coverPath;
        }
    }
}
