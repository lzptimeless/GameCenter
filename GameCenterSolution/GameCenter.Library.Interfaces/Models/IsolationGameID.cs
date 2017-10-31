using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCenter.Library
{
    public class IsolationGameID : GameID
    {
        public IsolationGameID(string launcher)
        {
            Launcher = launcher;
        }

        public override GamePlatformFlags PlatformFlag
        {
            get { return GamePlatformFlags.Isolation; }
        }

        public string Launcher { get; private set; }

        public override string ToString()
        {
            return $"GameID: {PlatformFlag}, {Launcher}";
        }

        public override GameID DeepClone()
        {
            IsolationGameID clone = MemberwiseClone() as IsolationGameID;

            return clone;
        }

        protected override int InnerGetHashCode()
        {
            return Launcher != null ? Launcher.GetHashCode() : 0;
        }

        protected override bool InnerEquals(GameID other)
        {
            IsolationGameID other1 = other as IsolationGameID;
            if (object.ReferenceEquals(other1, null)) return false;

            return string.Equals(other1.Launcher, Launcher, StringComparison.OrdinalIgnoreCase);
        }
    }
}
