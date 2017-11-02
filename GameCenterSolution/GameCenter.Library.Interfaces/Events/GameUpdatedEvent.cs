using AppCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCenter.Library
{
    public class GameUpdatedEvent : PubSubEvent<GameUpdatedEventData>
    {
    }

    public class GameUpdatedEventData : EventData
    {
        public GameUpdatedEventData(Game game, GameUpdatedFields fields)
        {
            Game = game;
            Fields = fields;
        }

        public Game Game { get; private set; }
        public GameUpdatedFields Fields { get; private set; }
    }

    [Flags]
    public enum GameUpdatedFields
    {
        Name = 0x1,
        Cover = 0x2,
        PlatformGameInfo = 0x4,
        All = Name | Cover | PlatformGameInfo
    }
}
