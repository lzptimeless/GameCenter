using AppCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCenter.Library
{
    public interface ILibrary : IModule
    {
        GameAddedEvent GameAddedEvent { get; }

        GameRemovedEvent GameRemovedEvent { get; }

        GameUpdatedEvent GameUpdatedEvent { get; }

        void StartScan();

        void StopScan();

        List<Game> GetGames();

        void LaunchGame(GameID id);
    }
}
