using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCenter.Library
{
    public class BattleNetGameID : GameID
    {
        public BattleNetGameID(string name)
        {
            Name = name;
        }

        public override GamePlatformFlags PlatformFlag
        {
            get { return GamePlatformFlags.BattleNet; }
        }

        public string Name { get; private set; }

        public override string ToString()
        {
            return $"{PlatformFlag}, {Name}";
        }

        protected override object CloneInner()
        {
            return MemberwiseClone();
        }

        protected override bool InnerEquals(GameID other)
        {
            BattleNetGameID other1 = other as BattleNetGameID;
            if (object.ReferenceEquals(other1, null)) return false;

            return string.Equals(other1.Name, Name, StringComparison.OrdinalIgnoreCase);
        }

        protected override int InnerGetHashCode()
        {
            return Name != null ? Name.GetHashCode() : 0;
        }
    }
}
