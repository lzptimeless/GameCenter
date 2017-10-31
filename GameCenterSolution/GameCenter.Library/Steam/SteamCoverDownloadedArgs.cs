using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCenter.Library
{
    internal class SteamCoverDownloadedArgs : EventArgs
    {
        public SteamCoverDownloadedArgs(Int64 appID, string smallPath, string normalPath, SteamCoverDownloadResultStates state)
        {
            AppID = appID;
            SmallPath = smallPath;
            NormalPath = normalPath;
            State = state;
        }

        public Int64 AppID { get; private set; }
        public string SmallPath { get; private set; }
        public string NormalPath { get; private set; }
        public SteamCoverDownloadResultStates State { get; private set; }
    }
}
