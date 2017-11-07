using AppCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCenter.Library
{
    public class GameRemovedEvent : PubSubEvent<GameRemovedEventData>
    {
    }

    public class GameRemovedEventData : ModelEventData<Game>
    {
        public GameRemovedEventData(Game model) : base(model)
        {
        }
    }
}
