using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore
{
    /// <summary>
    /// Represents a reference to a <see cref="Delegate"/>.
    /// </summary>
    internal interface IDelegateReference
    {
        /// <summary>
        /// Gets the referenced <see cref="Delegate" /> object.
        /// </summary>
        /// <value>A <see cref="Delegate"/> instance if the target is valid; otherwise <see langword="null"/>.</value>
        Delegate Func { get; }

        /// <summary>
        /// 委托对应的对象实例
        /// The target instance referenced Delegate (Func)
        /// </summary>
        object Target { get; }
    }
}
