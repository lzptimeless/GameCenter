using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCenter.Library
{
    public class BattleNetGameID : GameID
    {
        /// <summary>
        /// 通过元数据创建游戏ID
        /// </summary>
        /// <param name="metadata">元数据</param>
        public BattleNetGameID(UInt64 metadata)
        {
            Metadata = metadata;
            if (PlatformFlag != GamePlatformFlags.BattleNet)
                throw new ArgumentException($"Platform should be {GamePlatformFlags.BattleNet}:{metadata:X}");
        }
    }
}
