using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCenter.Library
{
    public class IsolationGameInfo : PlatformGameInfo
    {
        public override GamePlatformFlags PlatformFlag
        {
            get { return GamePlatformFlags.Isolation; }
        }

        public string Launcher { get; set; }

        public override PlatformGameInfo DeepClone()
        {
            IsolationGameInfo clone = MemberwiseClone() as IsolationGameInfo;

            return clone;
        }
    }
}
