using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCenter.Library
{
    internal class SteamCoverDownloadedArgs : EventArgs
    {
        public SteamCoverDownloadedArgs(Int64 appID, string capsulePath, string headerPath, SteamCoverDownloadResultStates state)
        {
            AppID = appID;
            CapsulePath = capsulePath;
            HeaderPath = headerPath;
            State = state;
        }

        public Int64 AppID { get; private set; }
        public string CapsulePath { get; private set; }
        public string HeaderPath { get; private set; }
        public SteamCoverDownloadResultStates State { get; private set; }
    }
}
