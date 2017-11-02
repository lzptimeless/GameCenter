using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCenter.Library
{
    public class GameCover
    {
        public string Icon { get; set; }
        public string Small { get; set; }
        public string Normal { get; set; }
        public string Full { get; set; }

        public GameCover DeepClone()
        {
            return (GameCover)MemberwiseClone();
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Icon)) return $"I:{Icon}";
            if (!string.IsNullOrEmpty(Small)) return $"S:{Small}";
            if (!string.IsNullOrEmpty(Normal)) return $"N:{Normal}";
            if (!string.IsNullOrEmpty(Full)) return $"F:{Full}";

            return "None";
        }
    }
}
