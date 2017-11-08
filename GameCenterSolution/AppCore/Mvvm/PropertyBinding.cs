using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AppCore
{
    /// <summary>
    /// 用以在<see cref="BindableModel{TModel}"/>中绑定数据模型属性
    /// </summary>
    public abstract class PropertyBindingBase<TModel>
    {
        public IReadOnlyList<PropertyPath> Properties { get; protected set; }

        /// <summary>
        /// 是否需要更新
        /// </summary>
        /// <param name="propertyPath">变更的属性</param>
        /// <returns>true:需要更新，false:不需要更新</returns>
        public bool IsNeedUpdate(PropertyPath propertyPath)
        {
            if (propertyPath == null) throw new ArgumentNullException("propertyPath");

            foreach (var p in Properties)
            {
                if (propertyPath.IsAncestorOfOrEquals(p)) return true;
            }

            return false;
        }

        /// <summary>
        /// 是否需要更新
        /// </summary>
        /// <param name="propertyPaths">变更的属性集合</param>
        /// <returns>true:需要更新，false:不需要更新</returns>
        public bool IsNeedUpdate(IEnumerable<PropertyPath> propertyPaths)
        {
            if (propertyPaths == null) throw new ArgumentNullException("propertyPaths");

            foreach (var propertyPath in propertyPaths)
            {
                if (IsNeedUpdate(propertyPath)) return true;
            }

            return false;
        }

        public abstract void Update(TModel model, object updateTarget);
    }

    public class PropertyBinding<TModel, TProperty, TUpdateTarget> : PropertyBindingBase<TModel>
    {
        public PropertyBinding(Expression<Func<TModel, TProperty>> propertyExpression, Action<TUpdateTarget, TProperty> update)
        {
            if (propertyExpression == null) throw new ArgumentNullException("propertyExpression");
            if (update == null) throw new ArgumentNullException("update");

            var propertyInfo = PropertySupport.ExtractPropertyPath(propertyExpression);
            var properties = new List<PropertyPath>();
            properties.Add(propertyInfo);

            Properties = properties;
            _getProperty = propertyExpression.Compile();
            _update = update;
        }

        private Func<TModel, TProperty> _getProperty;
        private Action<TUpdateTarget, TProperty> _update;

        public override void Update(TModel model, object updateTarget)
        {
            if (model == null) throw new ArgumentNullException("model");
            if (updateTarget == null) throw new ArgumentNullException("updateTarget");

            TProperty propValue = _getProperty(model);
            _update((TUpdateTarget)updateTarget, propValue);
        }
    }

    public class PropertyBinding<TModel, TProperty1, TProperty2, TUpdateTarget> : PropertyBindingBase<TModel>
    {
        public PropertyBinding(Expression<Func<TModel, TProperty1>> propertyExpression1,
            Expression<Func<TModel, TProperty2>> propertyExpression2,
            Action<TUpdateTarget, TProperty1, TProperty2> update)
        {
            if (propertyExpression1 == null) throw new ArgumentNullException("propertyExpression1");
            if (propertyExpression2 == null) throw new ArgumentNullException("propertyExpression2");
            if (update == null) throw new ArgumentNullException("update");

            var propertyInfo1 = PropertySupport.ExtractPropertyPath(propertyExpression1);
            var propertyInfo2 = PropertySupport.ExtractPropertyPath(propertyExpression2);
            var properties = new List<PropertyPath>();
            properties.Add(propertyInfo1);
            properties.Add(propertyInfo2);

            Properties = properties;
            _getProperty1 = propertyExpression1.Compile();
            _getProperty2 = propertyExpression2.Compile();
            _update = update;
        }

        private Func<TModel, TProperty1> _getProperty1;
        private Func<TModel, TProperty2> _getProperty2;
        private Action<TUpdateTarget, TProperty1, TProperty2> _update;

        public override void Update(TModel model, object updateTarget)
        {
            if (model == null) throw new ArgumentNullException("model");
            if (updateTarget == null) throw new ArgumentNullException("updateTarget");

            TProperty1 propValue1 = _getProperty1(model);
            TProperty2 propValue2 = _getProperty2(model);
            _update((TUpdateTarget)updateTarget, propValue1, propValue2);
        }
    }

    public class PropertyBinding<TModel, TProperty1, TProperty2, TProperty3, TUpdateTarget> : PropertyBindingBase<TModel>
    {
        public PropertyBinding(Expression<Func<TModel, TProperty1>> propertyExpression1,
            Expression<Func<TModel, TProperty2>> propertyExpression2,
            Expression<Func<TModel, TProperty3>> propertyExpression3,
            Action<TUpdateTarget, TProperty1, TProperty2, TProperty3> update)
        {
            if (propertyExpression1 == null) throw new ArgumentNullException("propertyExpression1");
            if (propertyExpression2 == null) throw new ArgumentNullException("propertyExpression2");
            if (propertyExpression3 == null) throw new ArgumentNullException("propertyExpression3");
            if (update == null) throw new ArgumentNullException("update");

            var propertyInfo1 = PropertySupport.ExtractPropertyPath(propertyExpression1);
            var propertyInfo2 = PropertySupport.ExtractPropertyPath(propertyExpression2);
            var propertyInfo3 = PropertySupport.ExtractPropertyPath(propertyExpression3);
            var properties = new List<PropertyPath>();
            properties.Add(propertyInfo1);
            properties.Add(propertyInfo2);
            properties.Add(propertyInfo3);

            Properties = properties;
            _getProperty1 = propertyExpression1.Compile();
            _getProperty2 = propertyExpression2.Compile();
            _getProperty3 = propertyExpression3.Compile();
            _update = update;
        }

        private Func<TModel, TProperty1> _getProperty1;
        private Func<TModel, TProperty2> _getProperty2;
        private Func<TModel, TProperty3> _getProperty3;
        private Action<TUpdateTarget, TProperty1, TProperty2, TProperty3> _update;

        public override void Update(TModel model, object updateTarget)
        {
            if (model == null) throw new ArgumentNullException("model");
            if (updateTarget == null) throw new ArgumentNullException("updateTarget");

            TProperty1 propValue1 = _getProperty1(model);
            TProperty2 propValue2 = _getProperty2(model);
            TProperty3 propValue3 = _getProperty3(model);
            _update((TUpdateTarget)updateTarget, propValue1, propValue2, propValue3);
        }
    }

    /// <summary>
    /// 数据绑定集合，不支持线程安全
    /// </summary>
    /// <typeparam name="TModel">数据模型类型</typeparam>
    /// <typeparam name="TUpdateTarget">需要更新的对象类型，这个类型就是继承了<see cref="BindableModel{TModel}"/>
    /// 的类型</typeparam>
    public class PropertyBindingCollection<TModel, TUpdateTarget> : ICollection<PropertyBindingBase<TModel>>, IReadOnlyList<PropertyBindingBase<TModel>>
    {
        private List<PropertyBindingBase<TModel>> _bindings = new List<PropertyBindingBase<TModel>>();

        public int Count
        {
            get { return _bindings.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public PropertyBindingBase<TModel> this[int index]
        {
            get { return _bindings[index]; }
        }

        public void Add<TProperty>(Expression<Func<TModel, TProperty>> propertyExpression,
            Action<TUpdateTarget, TProperty> update)
        {
            PropertyBinding<TModel, TProperty, TUpdateTarget> binding = new PropertyBinding<TModel, TProperty, TUpdateTarget>(
                propertyExpression,
                update);
            _bindings.Add(binding);
        }

        public void Add<TProperty1, TProperty2>(Expression<Func<TModel, TProperty1>> propertyExpression1,
            Expression<Func<TModel, TProperty2>> propertyExpression2,
            Action<TUpdateTarget, TProperty1, TProperty2> update)
        {
            PropertyBinding<TModel, TProperty1, TProperty2, TUpdateTarget> binding = new PropertyBinding<TModel, TProperty1, TProperty2, TUpdateTarget>(
                propertyExpression1,
                propertyExpression2,
                update);
            _bindings.Add(binding);
        }

        public void Add<TProperty1, TProperty2, TProperty3>(Expression<Func<TModel, TProperty1>> propertyExpression1,
            Expression<Func<TModel, TProperty2>> propertyExpression2,
            Expression<Func<TModel, TProperty3>> propertyExpression3,
            Action<TUpdateTarget, TProperty1, TProperty2, TProperty3> update)
        {
            PropertyBinding<TModel, TProperty1, TProperty2, TProperty3, TUpdateTarget> binding = new PropertyBinding<TModel, TProperty1, TProperty2, TProperty3, TUpdateTarget>(
                propertyExpression1,
                propertyExpression2,
                propertyExpression3,
                update);
            _bindings.Add(binding);
        }

        public void Add(PropertyBindingBase<TModel> item)
        {
            _bindings.Add(item);
        }

        public void Clear()
        {
            _bindings.Clear();
        }

        public bool Contains(PropertyBindingBase<TModel> item)
        {
            return _bindings.Contains(item);
        }

        public void CopyTo(PropertyBindingBase<TModel>[] array, int arrayIndex)
        {
            _bindings.CopyTo(array, arrayIndex);
        }

        public bool Remove(PropertyBindingBase<TModel> item)
        {
            return _bindings.Remove(item);
        }

        public IEnumerator<PropertyBindingBase<TModel>> GetEnumerator()
        {
            return _bindings.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _bindings.GetEnumerator();
        }
    }
}
