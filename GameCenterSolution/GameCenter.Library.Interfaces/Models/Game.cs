using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCenter.Library
{
    public class Game
    {
        public Game()
        {
            Cover = new GameCover();
        }

        public GameID ID { get; set; }

        public string Name { get; set; }

        public GameCover Cover { get; private set; }

        public PlatformGameInfo PlatformGameInfo { get; set; }

        public Game DeepClone()
        {
            Game clone = (Game)MemberwiseClone();

            if (ID != null) clone.ID = ID.DeepClone();
            if (Cover != null) clone.Cover = Cover.DeepClone();
            if (PlatformGameInfo != null) clone.PlatformGameInfo = PlatformGameInfo.DeepClone();

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
