using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameCenter.Library
{
    internal interface ILibraryProvider
    {
        GamePlatformFlags PlatformFlag { get; }

        event EventHandler<GameAddedEventData> GameAdded;
        event EventHandler<GameRemovedEventData> GameRemoved;
        event EventHandler<GameUpdatedEventData> GameUpdated;

        Task ScanAsync(CancellationToken ct);
        Task LaunchAsync(GameID id);
    }
}
