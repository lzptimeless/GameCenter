using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCenter.Library
{
    public class SteamGameInfo : PlatformGameInfo
    {
        public override GamePlatformFlags PlatformFlag
        {
            get { return GamePlatformFlags.Steam; }
        }

        public Int64 AppID { get; set; }

        protected override object CloneInner()
        {
            return MemberwiseClone();
        }
    }
}
