using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCenter.Library
{
    public abstract class GameID : IEquatable<GameID>
    {
        public abstract GamePlatformFlags PlatformFlag { get; }

        public abstract GameID DeepClone();

        protected abstract int InnerGetHashCode();

        protected abstract bool InnerEquals(GameID other);

        public override string ToString()
        {
            return $"GameID: {PlatformFlag}";
        }

        public override int GetHashCode()
        {
            return InnerGetHashCode();
        }

        public bool Equals(GameID other)
        {
            return InnerEquals(other);
        }

        public override bool Equals(object obj)
        {
            return InnerEquals(obj as GameID);
        }

        public static bool operator ==(GameID left, GameID right)
        {
            if (object.ReferenceEquals(left, null) && object.ReferenceEquals(right, null)) return true;
            if (object.ReferenceEquals(left, null) || object.ReferenceEquals(right, null)) return false;

            return left.InnerEquals(right);
        }

        public static bool operator !=(GameID left, GameID right)
        {
            return !(left == right);
        }
    }
}
