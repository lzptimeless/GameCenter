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
        /// true: 这个实例的属性只读，任何修改操作都会抛出异常
        /// false: 这个实例的属性可以修改
        /// </summary>
        public bool IsReadOnly { get; private set; }

        /// <summary>
        /// 设置对象为只读，之后任何修改对象属性的行为都会抛出异常，并且不能解除这个
        /// 设置，只有通过<see cref="Clone"/>函数获得一份全新的实例，这个新的实例
        /// 的属性可以修改
        /// </summary>
        public virtual void SetReadOnly()
        {
            IsReadOnly = true;
        }

        /// <summary>
        /// 复制实例，复制实例的<see cref="IsReadOnly"/>将被重置为false
        /// </summary>
        /// <returns>复制的数据模型</returns>
        public object Clone()
        {
            ModelBase mb = CloneInner() as ModelBase;
            if (mb == null)
                throw new InvalidCastException("Clone result should not be null and be ModelBase.");

            mb.ResetReadOnly();
            return mb;
        }

        /// <summary>
        /// 复制实例
        /// </summary>
        /// <returns>复制的数据模型</returns>
        protected abstract object CloneInner();

        /// <summary>
        /// 重置<see cref="IsReadOnly"/>属性
        /// </summary>
        private void ResetReadOnly()
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
