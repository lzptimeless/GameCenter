using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore
{
    ///<summary>
    /// Defines a contract for an event subscription to be used by <see cref="EventBase"/>.
    ///</summary>
    public interface IEventSubscription
    {
        /// <summary>
        /// The target that is referenced by the <see cref="IDelegateReference"/>.
        /// </summary>
        object Target { get; }

        /// <summary>
        /// Gets the execution strategy to publish this event.
        /// </summary>
        /// <returns>An <see cref="Action{T}"/> with the execution strategy, or <see langword="null" /> if the <see cref="IEventSubscription"/> is no longer valid.</returns>
        Action<object[]> GetExecutionStrategy();
    }
}
