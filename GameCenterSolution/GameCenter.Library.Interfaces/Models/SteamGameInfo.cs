using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCenter.Library
{
    public class SteamGameInfo : PlatformGameInfo
    {
        public override GamePlatformMarks Mark
        {
            get { return GamePlatformMarks.Steam; }
        }

        public Int64 AppID { get; set; }

        public override PlatformGameInfo DeepClone()
        {
            SteamGameInfo clone = MemberwiseClone() as SteamGameInfo;

            return clone;
        }
    }
}
