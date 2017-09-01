using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore
{
    /// <summary>
    /// Provides a way to retrieve a <see cref="Delegate"/> to execute an action depending
    /// on the value of a second filter predicate that returns true if the action should execute.
    /// </summary>
    /// <typeparam name="TPayload">The type to use for the generic <see cref="System.Action{TPayload}"/> type.</typeparam>
    internal class EventSubscription<TPayload> : IEventSubscription where TPayload : EventData
    {
        private readonly IDelegateReference _actionReference;

        ///<summary>
        /// Creates a new instance of <see cref="EventSubscription{TPayload}"/>.
        ///</summary>
        ///<param name="actionReference">A reference to a delegate of type <see cref="System.Action{TPayload}"/>.</param>
        ///<exception cref="ArgumentNullException">When <paramref name="actionReference"/> is <see langword="null" />.</exception>
        ///<exception cref="ArgumentException">When the target of <paramref name="actionReference"/> is not of type <see cref="System.Action{TPayload}"/>.</exception>
        public EventSubscription(IDelegateReference actionReference)
        {
            if (actionReference == null)
                throw new ArgumentNullException("actionReference");
            if (!(actionReference.Func is Action<TPayload>))
                throw new ArgumentException($"actionReference type must be {typeof(Action<TPayload>).FullName}");

            _actionReference = actionReference;
        }

        /// <summary>
        /// Gets the <see cref="System.Action{T}"/> that is referenced by the <see cref="IDelegateReference"/>.
        /// </summary>
        /// <value>An <see cref="System.Action{T}"/> or <see langword="null" /> if the referenced target is not alive.</value>
        public Action<TPayload> Action
        {
            get { return (Action<TPayload>)_actionReference.Func; }
        }

        /// <summary>
        /// The target that is referenced by the <see cref="IDelegateReference"/>.
        /// </summary>
        public object Target
        {
            get { return _actionReference.Target; }
        }

        /// <summary>
        /// Gets the execution strategy to publish this event.
        /// </summary>
        /// <returns>An <see cref="System.Action{T}"/> with the execution strategy, or <see langword="null" /> if the <see cref="IEventSubscription"/> is no longer valid.</returns>
        /// <remarks>
        /// If <see cref="Action"/> or <see cref="Filter"/> are no longer valid because they were
        /// garbage collected, this method will return <see langword="null" />.
        /// Otherwise it will return a delegate that evaluates the <see cref="Filter"/> and if it
        /// returns <see langword="true" /> will then call <see cref="InvokeAction"/>. The returned
        /// delegate holds hard references to the <see cref="Action"/> and <see cref="Filter"/> target
        /// <see cref="Delegate">delegates</see>. As long as the returned delegate is not garbage collected,
        /// the <see cref="Action"/> and <see cref="Filter"/> references delegates won't get collected either.
        /// </remarks>
        public virtual Action<object[]> GetExecutionStrategy()
        {
            Action<TPayload> action = this.Action;
            if (action != null)
            {
                return arguments =>
                {
                    TPayload argument = default(TPayload);
                    if (arguments != null && arguments.Length > 0 && arguments[0] != null)
                    {
                        argument = (TPayload)arguments[0];
                    }
                    InvokeAction(action, argument);
                };
            }
            return null;
        }

        /// <summary>
        /// Invokes the specified <see cref="System.Action{TPayload}"/> synchronously when not overridden.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="argument">The payload to pass <paramref name="action"/> while invoking it.</param>
        /// <exception cref="ArgumentNullException">An <see cref="ArgumentNullException"/> is thrown if <paramref name="action"/> is null.</exception>
        public virtual void InvokeAction(Action<TPayload> action, TPayload argument)
        {
            if (action == null) throw new System.ArgumentNullException("action");

            action(argument);
        }
    }
}
