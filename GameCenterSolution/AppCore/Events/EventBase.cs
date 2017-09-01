using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AppCore
{
    ///<summary>
    /// Defines a base class to publish and subscribe to events.
    ///</summary>
    public abstract class EventBase
    {
        private readonly List<IEventSubscription> _subscriptions = new List<IEventSubscription>();

        /// <summary>
        /// Allows the SynchronizationContext to be set by the EventAggregator for UI Thread Dispatching
        /// </summary>
        public SynchronizationContext SynchronizationContext { get; set; }

        /// <summary>
        /// Gets the list of current subscriptions.
        /// </summary>
        /// <value>The current subscribers.</value>
        protected ICollection<IEventSubscription> Subscriptions
        {
            get { return _subscriptions; }
        }

        /// <summary>
        /// Removes the all subscriber matching target from the subscribers' list.
        /// </summary>
        /// <param name="target">The target instance request to subscribing.</param>
        /// <exception cref="ArgumentNullException">Throw when target is null.</exception>
        public void Unsubscribe(object target)
        {
            if (target == null) throw new ArgumentNullException("target");

            lock (Subscriptions)
            {
                IEventSubscription eventSubscription = Subscriptions.FirstOrDefault(evt => object.ReferenceEquals(evt.Target, target));
                if (eventSubscription != null)
                {
                    Subscriptions.Remove(eventSubscription);
                }
            }
        }

        /// <summary>
        /// Returns <see langword="true"/> if there is a subscribing requested by target
        /// </summary>
        /// <param name="target">The target request subscribe</param>
        /// <returns></returns>
        public bool Contains(object target)
        {
            if (target == null) throw new ArgumentNullException("target");
            lock (Subscriptions)
            {
                return Subscriptions.Any(evt => object.ReferenceEquals(evt.Target, target));
            }
        }

        /// <summary>
        /// Adds the specified <see cref="IEventSubscription"/> to the subscribers' collection.
        /// </summary>
        /// <param name="eventSubscription">The subscriber.</param>
        /// <returns>The <see cref="SubscriptionToken"/> that uniquely identifies every subscriber.</returns>
        /// <remarks>
        /// Adds the subscription to the internal list and assigns it a new <see cref="SubscriptionToken"/>.
        /// </remarks>
        protected virtual void InternalSubscribe(IEventSubscription eventSubscription)
        {
            if (eventSubscription == null) throw new System.ArgumentNullException("eventSubscription");

            lock (Subscriptions)
            {
                // 防止同一个对象实例重复订阅一个事件，从而导致不可预知的错误
                object target = eventSubscription.Target;
                if (target != null && Contains(target))
                    throw new InvalidOperationException($"{target} already subscribed.");
                // 完成订阅
                Subscriptions.Add(eventSubscription);
            }
        }

        /// <summary>
        /// Calls all the execution strategies exposed by the list of <see cref="IEventSubscription"/>.
        /// </summary>
        /// <param name="arguments">The arguments that will be passed to the listeners.</param>
        /// <remarks>Before executing the strategies, this class will prune all the subscribers from the
        /// list that return a <see langword="null" /> <see cref="Action{T}"/> when calling the
        /// <see cref="IEventSubscription.GetExecutionStrategy"/> method.</remarks>
        protected virtual void InternalPublish(params object[] arguments)
        {
            List<Action<object[]>> executionStrategies = PruneAndReturnStrategies();
            foreach (var executionStrategy in executionStrategies)
            {
                executionStrategy(arguments);
            }
        }

        private List<Action<object[]>> PruneAndReturnStrategies()
        {
            List<Action<object[]>> returnList = new List<Action<object[]>>();

            lock (Subscriptions)
            {
                for (var i = Subscriptions.Count - 1; i >= 0; i--)
                {
                    Action<object[]> listItem =
                        _subscriptions[i].GetExecutionStrategy();

                    if (listItem == null)
                    {
                        // Prune from main list. Log?
                        _subscriptions.RemoveAt(i);
                    }
                    else
                    {
                        returnList.Add(listItem);
                    }
                }
            }

            return returnList;
        }
    }
}
