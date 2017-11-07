using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore
{
    /// <summary>
    /// 数据模型基础类
    /// </summary>
    public abstract class ModelBase : ICloneable
    {
        /// <summary>
        /// true: Can only read property, can not set property
        /// false: Can read and set property
        /// </summary>
        public bool IsReadOnly { get; private set; }

        /// <summary>
        /// Set <see cref="IsReadOnly"/> to true
        /// </summary>
        public void SetReadOnly()
        {
            IsReadOnly = true;
        }

        /// <summary>
        /// 复制实例，注意需要重置<see cref="IsReadOnly"/>属性
        /// </summary>
        /// <returns>复制的数据模型，<see cref="IsReadOnly"/>为false</returns>
        public abstract object Clone();

        /// <summary>
        /// Set <see cref="IsReadOnly"/> to false
        /// </summary>
        protected void ResetReadOnly()
        {
            IsReadOnly = false;
        }

        /// <summary>
        /// If <see cref="IsReadOnly"/> is true, throw exception
        /// </summary>
        /// <exception cref="InvalidOperationException">Throw when <see cref="IsReadOnly"/> is true.</exception>
        protected void CheckSet()
        {
            if (IsReadOnly) throw new InvalidOperationException("This model is readonly.");
        }
    }
}
