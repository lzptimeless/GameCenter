using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCenter.Library
{
    public abstract class PlatformGameInfo
    {
        public abstract GamePlatformFlags PlatformFlag { get; }

        public abstract PlatformGameInfo Clone();
    }
}
