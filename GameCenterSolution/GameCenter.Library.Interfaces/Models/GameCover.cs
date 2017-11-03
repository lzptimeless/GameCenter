using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCenter.Library
{
    /// <summary>
    /// 游戏各种版本Cover集合，由于Steam游戏最多，以Steam游戏封面为基准
    /// </summary>
    public class GameCover
    {
        /// <summary>
        /// 一般来自从执行文件中提取，在缺少封面时备用，正方形，大小32x32到256x256都有可能
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// 简略版本封面，一般用于列表模式显示，大小231x87
        /// </summary>
        public string Capsule { get; set; }
        /// <summary>
        /// 游戏封面，一般用于缩略图模式显示，大小460x215
        /// </summary>
        public string Header { get; set; }
        /// <summary>
        /// 游戏封面全图，一般用于背景显示，大小不确定
        /// </summary>
        public string Full { get; set; }

        public GameCover DeepClone()
        {
            return (GameCover)MemberwiseClone();
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Icon)) return $"I:{Icon}";
            if (!string.IsNullOrEmpty(Capsule)) return $"C:{Capsule}";
            if (!string.IsNullOrEmpty(Header)) return $"H:{Header}";
            if (!string.IsNullOrEmpty(Full)) return $"F:{Full}";

            return "None";
        }
    }
}
