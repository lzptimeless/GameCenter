using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCenter.Library
{
    public class SelfGameInfo : PlatformGameInfo
    {
        public override GamePlatformMarks Mark
        {
            get { return GamePlatformMarks.Self; }
        }

        public string Launcher { get; set; }

        public override PlatformGameInfo DeepClone()
        {
            SelfGameInfo clone = MemberwiseClone() as SelfGameInfo;

            return clone;
        }
    }
}
