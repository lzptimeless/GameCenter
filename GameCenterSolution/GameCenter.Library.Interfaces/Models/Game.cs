using AppCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCenter.Library
{
    public class Game : ModelBase, ICloneable
    {
        public Game()
        {
            Cover = new GameCover();
        }

        #region ID
        private GameID _id;
        /// <summary>
        /// 游戏ID
        /// </summary>
        public GameID ID
        {
            get { return _id; }
            set
            {
                CheckSet();
                _id = value;
            }
        }
        #endregion

        #region Name
        private string _name;
        /// <summary>
        /// 游戏名
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                CheckSet();
                _name = value;
            }
        }
        #endregion

        #region Cover
        private GameCover _cover;
        /// <summary>
        /// 游戏封面组合
        /// </summary>
        public GameCover Cover
        {
            get { return _cover; }
            private set
            {
                CheckSet();
                _cover = value;
            }
        }
        #endregion

        #region PlatformGameInfo
        private PlatformGameInfo _platformGameInfo;
        /// <summary>
        /// 专属平台的游戏信息
        /// </summary>
        public PlatformGameInfo PlatformGameInfo
        {
            get { return _platformGameInfo; }
            set
            {
                CheckSet();
                _platformGameInfo = value;
            }
        }
        #endregion

        /// <summary>
        /// 复制游戏，复制的游戏的<see cref="ModelBase.IsReadOnly"/>属性将被重置
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            Game clone = (Game)MemberwiseClone();
            clone.ResetReadOnly();

            if (ID != null) clone.ID = ID.Clone();
            if (Cover != null) clone.Cover = Cover.Clone();
            if (PlatformGameInfo != null) clone.PlatformGameInfo = PlatformGameInfo.Clone();

            return clone;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(Name)) sb.Append(Name);
            if (ID != null)
            {
                if (sb.Length > 0) sb.Append(", ");
                sb.Append(ID);
            }

            return sb.ToString();
        }
    }
}
