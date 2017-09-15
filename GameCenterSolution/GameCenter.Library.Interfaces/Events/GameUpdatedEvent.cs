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
        public GameUpdatedEventData(Game game)
        {
            Game = game;
        }

        public Game Game { get; private set; }
    }
}
