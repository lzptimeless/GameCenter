using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCenter.Library
{
    public class SteamGameID : GameID
    {
        public SteamGameID(Int64 appID)
        {
            AppID = appID;
        }

        public override GamePlatformFlags PlatformFlag
        {
            get { return GamePlatformFlags.Steam; }
        }

        public Int64 AppID { get; private set; }

        public override string ToString()
        {
            return $"{PlatformFlag}, {AppID}";
        }

        public override GameID Clone()
        {
            SteamGameID clone = MemberwiseClone() as SteamGameID;

            return clone;
        }

        protected override int InnerGetHashCode()
        {
            return AppID.GetHashCode();
        }

        protected override bool InnerEquals(GameID other)
        {
            SteamGameID other1 = other as SteamGameID;
            if (object.ReferenceEquals(other1, null)) return false;

            return other1.AppID == AppID;
        }
    }
}
