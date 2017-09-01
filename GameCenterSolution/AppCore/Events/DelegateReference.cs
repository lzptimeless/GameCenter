using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AppCore
{
    /// <summary>
    /// Represents a reference to a <see cref="Delegate"/> that may contain a
    /// <see cref="WeakReference"/> to the target. This class is used
    /// internally by the Prism Library.
    /// </summary>
    internal class DelegateReference : IDelegateReference
    {
        private readonly WeakReference _weakReference;
        private readonly MethodInfo _method;
        private readonly Type _delegateType;

        /// <summary>
        /// Initializes a new instance of <see cref="DelegateReference"/>.
        /// </summary>
        /// <param name="delegate">The original <see cref="Delegate"/> to create a reference for.</param>
        /// <exception cref="ArgumentNullException">If the passed <paramref name="delegate"/> is not assignable to <see cref="Delegate"/>.</exception>
        public DelegateReference(Delegate @delegate)
        {
            if (@delegate == null)
                throw new ArgumentNullException("delegate");

            _weakReference = new WeakReference(@delegate.Target);
            _method = @delegate.Method;
            _delegateType = @delegate.GetType();
        }

        /// <summary>
        /// Gets the <see cref="Delegate" /> (the function) referenced by the current <see cref="DelegateReference"/> object.
        /// </summary>
        /// <value><see langword="null"/> if the object referenced by the current <see cref="DelegateReference"/> object has been garbage collected; otherwise, a reference to the <see cref="Delegate"/> referenced by the current <see cref="DelegateReference"/> object.</value>
        public Delegate Func
        {
            get
            {
                return TryGetDelegate();
            }
        }

        /// <summary>
        /// 委托对应的对象实例
        /// </summary>
        public object Target
        {
            get
            {
                return _weakReference.Target;
            }
        }

        private Delegate TryGetDelegate()
        {
            if (_method.IsStatic)
            {
                return Delegate.CreateDelegate(_delegateType, null, _method);
            }
            object target = _weakReference.Target;
            if (target != null)
            {
                return Delegate.CreateDelegate(_delegateType, target, _method);
            }
            return null;
        }
    }
}
