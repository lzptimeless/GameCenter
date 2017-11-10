using AppCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCenter.Library
{
    public abstract class GameID : IEquatable<GameID>
    {
        public const byte DefaultVersion = 1;

        /// <summary>
        /// 表示整个GameID的元数据
        /// </summary>
        public UInt64 Metadata { get; protected set; }

        /// <summary>
        /// GameID版本号，Metadata(63~60)
        /// </summary>
        public byte Version
        {
            get { return (byte)(Metadata >> 60); }
            protected set
            {
                if (value > 0x0F)
                    throw new ArgumentOutOfRangeException("value", $"value should in (0~15):{value}");

                Metadata = Metadata & 0x0FFFFFFFFFFFFFFF | (((UInt64)value) << 60);
            }
        }

        /// <summary>
        /// 游戏平台，Metadata(59~56)
        /// </summary>
        public byte Platform
        {
            get { return (byte)((Metadata >> 56) & 0x0F); }
            protected set
            {
                if (value > 0x0F)
                    throw new ArgumentOutOfRangeException("value", $"value should in (0~15):{value}");

                Metadata = Metadata & 0xF0FFFFFFFFFFFFFF | (((UInt64)value) << 56);
            }
        }

        /// <summary>
        /// 游戏平台，是对属性<see cref="Platform"/>的扩展，方面使用
        /// </summary>
        public GamePlatformFlags PlatformFlag
        {
            get
            {
                switch (Platform)
                {
                    case 0: return GamePlatformFlags.Isolation;
                    case 1: return GamePlatformFlags.Steam;
                    case 2: return GamePlatformFlags.BattleNet;
                    case 3: return GamePlatformFlags.TGA;
                    case 4: return GamePlatformFlags.Origin;
                    case 5: return GamePlatformFlags.Uplay;
                    default: return GamePlatformFlags.Unkown;
                }
            }
            protected set
            {
                switch (value)
                {
                    case GamePlatformFlags.Isolation:
                        Platform = 0;
                        break;
                    case GamePlatformFlags.Steam:
                        Platform = 1;
                        break;
                    case GamePlatformFlags.BattleNet:
                        Platform = 2;
                        break;
                    case GamePlatformFlags.TGA:
                        Platform = 3;
                        break;
                    case GamePlatformFlags.Origin:
                        Platform = 4;
                        break;
                    case GamePlatformFlags.Uplay:
                        Platform = 5;
                        break;
                    default:
                        throw new ArgumentException($"Not supported Platform:{value}", "value");
                }
            }
        }

        /// <summary>
        /// 平台自定义数据，Metadata(55~48)
        /// </summary>
        public byte Custom
        {
            get { return (byte)(Metadata >> 48); }
            protected set
            {
                Metadata = Metadata & 0xFF00FFFFFFFFFFFF | (((UInt64)value) << 48);
            }
        }

        /// <summary>
        /// 游戏编号，Metadata(47~0)
        /// </summary>
        public Int64 Number
        {
            get { return (Int64)(Metadata & 0x0000FFFFFFFFFFFF); }
            protected set
            {
                if (value < 0 || value > 0x0000FFFFFFFFFFFF)
                    throw new ArgumentOutOfRangeException("value", $"value should in (0~(2^48 - 1)):{value}");

                Metadata = Metadata & 0xFFFF000000000000 | (UInt64)value;
            }
        }

        public override string ToString()
        {
            return $"{Metadata:X}:{Version},{Platform}({PlatformFlag}),{Custom},{Number}";
        }

        public override int GetHashCode()
        {
            return Metadata.GetHashCode();
        }

        public bool Equals(GameID other)
        {
            if (object.ReferenceEquals(other, null)) return false;

            return other.Metadata.Equals(Metadata);
        }

        public override bool Equals(object obj)
        {
            GameID other = (GameID)obj;
            if (object.ReferenceEquals(other, null)) return false;

            return other.Metadata.Equals(Metadata);
        }

        public static bool operator ==(GameID left, GameID right)
        {
            if (object.ReferenceEquals(left, null) && object.ReferenceEquals(right, null)) return true;
            if (object.ReferenceEquals(left, null) || object.ReferenceEquals(right, null)) return false;

            return left.Equals(right);
        }

        public static bool operator !=(GameID left, GameID right)
        {
            return !(left == right);
        }
    }
}
