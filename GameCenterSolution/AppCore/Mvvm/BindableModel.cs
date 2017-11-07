using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AppCore
{
    /// <summary>
    /// 提供与数据模型绑定的基本功能
    /// </summary>
    /// <typeparam name="TModel">数据模型类型</typeparam>
    public abstract class BindableModel<TModel> : BindableBase
    {
        /// <summary>
        /// 属性绑定缓存，加快获取属性绑定的速度
        /// </summary>
        private static PropertyBindingCollection<TModel> _propertyBindings = null;

        /// <summary>
        /// 获取属性绑定集合，如果还没有创建会自动调用<see cref="CreatePropertyBindings"/>
        /// </summary>
        protected IReadOnlyList<PropertyBindingBase<TModel>> PropertyBindings
        {
            get
            {
                var propertyBindings = Volatile.Read(ref _propertyBindings);
                if (propertyBindings == null)
                {
                    propertyBindings = CreatePropertyBindings();
                    Interlocked.CompareExchange(ref _propertyBindings, propertyBindings, null);
                }
                return Volatile.Read(ref _propertyBindings);
            }
        }

        /// <summary>
        /// 当前绑定的数据模型实例，可能为null
        /// </summary>
        protected TModel Model { get; private set; }

        /// <summary>
        /// 通过新的数据模型实例更新所有属性绑定
        /// </summary>
        /// <param name="model">数据模型</param>
        public virtual void Update(TModel model, params PropertyPath[] properties)
        {
            bool updateAll = properties == null || properties.Length == 0;
            foreach (var propertyBinding in PropertyBindings)
            {
                if (updateAll || propertyBinding.IsNeedUpdate(properties))
                {
                    propertyBinding.Update(model, this);
                }
            }
        }

        /// <summary>
        /// 创建属性绑定，一个类型可能只会调用一次
        /// </summary>
        /// <returns>属性绑定集合</returns>
        protected abstract PropertyBindingCollection<TModel> CreatePropertyBindings();
    }
}
