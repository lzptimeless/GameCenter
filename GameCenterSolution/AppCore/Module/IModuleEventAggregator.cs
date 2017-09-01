using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore
{
    public interface IModuleEventAggregator
    {
        /// <summary>
        /// Gets the single instance of the event managed by this EventAggregator. Multiple calls to this method with the same <typeparamref name="TEventType"/> returns the same event instance.
        /// </summary>
        /// <typeparam name="TEventType">The type of event to get. This must inherit from <see cref="EventBase"/>.</typeparam>
        /// <returns>A singleton instance of an event object of type <typeparamref name="TEventType"/>.</returns>
        TEventType GetEvent<TEventType>() where TEventType : EventBase, new();

        /// <summary>
        /// 取消指定对象实例的所有事件订阅
        /// Unsubscribe all event that request by target.
        /// </summary>
        /// <param name="target">订阅了事件的对象实例，The target have requested events.</param>
        void Unsubscribe(object target);
    }
}
