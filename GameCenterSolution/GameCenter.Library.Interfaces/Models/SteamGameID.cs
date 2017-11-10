using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCenter.Library
{
    public class SteamGameID : GameID
    {
        /// <summary>
        /// 通过元数据创建游戏ID
        /// </summary>
        /// <param name="metadata">元数据</param>
        public SteamGameID(UInt64 metadata)
        {
            Metadata = metadata;
            if (PlatformFlag != GamePlatformFlags.Steam)
                throw new ArgumentException($"Platform should be {GamePlatformFlags.Steam}:{metadata:X}");
        }

        /// <summary>
        /// 通过Steam AppID创建游戏ID
        /// </summary>
        /// <param name="appID">Steam AppID</param>
        public SteamGameID(Int64 appID)
        {
            Version = DefaultVersion;
            PlatformFlag = GamePlatformFlags.Steam;
            Number = appID;
        }

        /// <summary>
        /// Steam AppID，等于<see cref="GameID.Number"/>
        /// </summary>
        public Int64 AppID
        {
            get { return Number; }
        }
    }
}
