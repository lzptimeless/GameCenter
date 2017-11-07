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

    public class GameAddedEventData : ModelEventData<Game>
    {
        public GameAddedEventData(Game model) : base(model)
        {
        }
    }
}
