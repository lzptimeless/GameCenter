using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCenter.Library
{
    public class SelfGameID : GameID
    {
        public SelfGameID(string launcher)
        {
            Launcher = launcher;
        }

        public override GamePlatformMarks PlatformMark
        {
            get { return GamePlatformMarks.Self; }
        }

        public string Launcher { get; private set; }

        public override string ToString()
        {
            return $"GameID: {PlatformMark}, {Launcher}";
        }

        public override GameID DeepClone()
        {
            SelfGameID clone = MemberwiseClone() as SelfGameID;

            return clone;
        }

        protected override int InnerGetHashCode()
        {
            return Launcher != null ? Launcher.GetHashCode() : 0;
        }

        protected override bool InnerEquals(GameID other)
        {
            SelfGameID other1 = other as SelfGameID;
            if (object.ReferenceEquals(other1, null)) return false;

            return string.Equals(other1.Launcher, Launcher, StringComparison.OrdinalIgnoreCase);
        }
    }
}
