using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCenter.Library
{
    public class BattleNetGameInfo : PlatformGameInfo
    {
        public override GamePlatformFlags PlatformFlag
        {
            get { return GamePlatformFlags.BattleNet; }
        }

        public string Name { get; set; }

        protected override object CloneInner()
        {
            return MemberwiseClone();
        }
    }
}
