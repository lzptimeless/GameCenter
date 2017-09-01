using AppCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCenter.Library
{
    public class GameAddedEvent : PubSubEvent<GameAddedEventData>
    {
    }

    public class GameAddedEventData : EventData
    {
        public GameAddedEventData(string gameName)
        {
            GameName = gameName;
        }

        public string GameName { get; private set; }
    }
}
