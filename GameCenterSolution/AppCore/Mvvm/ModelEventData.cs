using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AppCore
{
    public class ModelEventData<TModel> : EventData where TModel : ModelBase
    {
        public ModelEventData(TModel model)
        {
            if (model == null) throw new ArgumentNullException("model");
            if (!model.IsReadOnly) throw new ArgumentException("model should be read only.");

            Model = model;
        }

        public TModel Model { get; private set; }
    }

    /// <summary>
    /// 数据模型更新事件数据
    /// </summary>
    /// <typeparam name="TModel">数据模型类型</typeparam>
    public class ModelUpdatedEventData<TModel> : EventData where TModel : ModelBase
    {
        /// <summary>
        /// 创建数据模型更新事件数据
        /// </summary>
        /// <param name="model">新的数据模型实例</param>
        /// <param name="updatedPaths">变更属性路径集合，null和empty代表更新所有属性</param>
        public ModelUpdatedEventData(TModel model, IEnumerable<PropertyPath> updatedPaths)
        {
            if (model == null) throw new ArgumentNullException("model");
            if (!model.IsReadOnly) throw new ArgumentException("model should be read only.");

            Model = model;
            UpdatedPropertyPaths = updatedPaths != null ? new List<PropertyPath>(updatedPaths) : new List<PropertyPath>();
        }

        public TModel Model { get; private set; }
        public IReadOnlyList<PropertyPath> UpdatedPropertyPaths { get; private set; }

        public static ModelUpdatedEventData<TModel> Create<TProperty>(TModel model,
            params Expression<Func<TProperty>>[] propertyExpressions)
        {
            if (model == null) throw new ArgumentNullException("model");

            List<PropertyPath> propertyPaths = null;
            if (propertyExpressions != null)
            {
                propertyPaths = new List<PropertyPath>();
                foreach (var propertyExpression in propertyExpressions)
                {
                    PropertyPath propPath = PropertySupport.ExtractPropertyPath(propertyExpression);
                    propertyPaths.Add(propPath);
                }
            }

            ModelUpdatedEventData<TModel> data = new ModelUpdatedEventData<TModel>(model, propertyPaths);
            return data;
        }
    }
}
