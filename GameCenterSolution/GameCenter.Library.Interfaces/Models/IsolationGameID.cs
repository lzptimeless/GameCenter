using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCenter.Library
{
    public class IsolationGameID : GameID
    {
        /// <summary>
        /// 通过元数据创建游戏ID
        /// </summary>
        /// <param name="metadata">元数据</param>
        public IsolationGameID(UInt64 metadata)
        {
            Metadata = metadata;
            if (PlatformFlag != GamePlatformFlags.Isolation)
                throw new ArgumentException($"Platform should be {GamePlatformFlags.Isolation}:{metadata:X}");
        }

        /// <summary>
        /// 创建一个临时ID
        /// </summary>
        /// <param name="number">临时编号</param>
        public IsolationGameID(Int64 number)
        {
            Version = DefaultVersion;
            PlatformFlag = GamePlatformFlags.Isolation;
            Temporary = true;
            Number = number;
        }

        /// <summary>
        /// 这个ID是否是临时的（不是来自数据库定义），如果是用户手动添加的游戏则游戏ID是临时的，Metadata(55)
        /// </summary>
        public bool Temporary
        {
            get { return (Custom & 0x80) > 0; }
            private set
            {
                Custom = (byte)(Custom & 0x7F | (value ? 0x80 : 0x00));
            }
        }
    }
}
