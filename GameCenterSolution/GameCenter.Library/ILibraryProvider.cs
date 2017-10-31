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

        void Scan(CancellationToken ct);
        void Launch(GameID id);
    }
}
