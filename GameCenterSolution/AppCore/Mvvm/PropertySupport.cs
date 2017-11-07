using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AppCore
{
    ///<summary>
    /// Provides support for extracting property information based on a property expression.
    ///</summary>
    public static class PropertySupport
    {
        /// <summary>
        /// Extracts the property name from a property expression.
        /// </summary>
        /// <typeparam name="T">The object type containing the property specified in the expression.</typeparam>
        /// <param name="propertyExpression">The property expression (e.g. p => p.PropertyName)</param>
        /// <returns>The name of the property.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="propertyExpression"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the expression is:<br/>
        ///     Not a <see cref="MemberExpression"/><br/>
        ///     The <see cref="MemberExpression"/> does not represent a property.<br/>
        ///     Or, the property is static.
        /// </exception>
        public static string ExtractPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            List<PropertyInfo> names;
            Exception ex;
            if (!TryExtractPropertyPath(propertyExpression, false, out names, out ex))
                throw ex;

            return names[0].Name;
        }

        /// <summary>
        /// Extracts the property name from a property expression.
        /// </summary>
        /// <typeparam name="T">The object type containing the property specified in the expression.</typeparam>
        /// <param name="propertyExpression">The property expression (e.g. p => p.PropertyName)</param>
        /// <returns>属性路径，其中<see cref="PropertyPath.Names"/>里面至少有一个值</returns>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="propertyExpression"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the expression is:<br/>
        ///     Not a <see cref="MemberExpression"/><br/>
        ///     The <see cref="MemberExpression"/> does not represent a property.<br/>
        ///     Or, the property is static.
        /// </exception>
        public static PropertyPath ExtractPropertyPath<T>(Expression<Func<T>> propertyExpression)
        {
            List<PropertyInfo> names;
            Exception ex;
            if (!TryExtractPropertyPath(propertyExpression, false, out names, out ex))
                throw ex;

            return new PropertyPath(names);
        }

        /// <summary>
        /// Extracts the property name from a property expression.
        /// </summary>
        /// <typeparam name="TProperty">The object type containing the property specified in the expression.</typeparam>
        /// <typeparam name="TModel">The object type define the property specified in the expression.</typeparam>
        /// <param name="propertyExpression">The property expression (e.g. p => p.PropertyName)</param>
        /// <returns>属性路径，其中<see cref="PropertyPath.Names"/>里面至少有一个值</returns>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="propertyExpression"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the expression is:<br/>
        ///     Not a <see cref="MemberExpression"/><br/>
        ///     The <see cref="MemberExpression"/> does not represent a property.<br/>
        ///     Or, the property is static.
        /// </exception>
        public static PropertyPath ExtractPropertyPath<TProperty, TModel>(Expression<Func<TProperty, TModel>> propertyExpression)
        {
            List<PropertyInfo> names;
            Exception ex;
            if (!TryExtractPropertyPath(propertyExpression, false, out names, out ex))
                throw ex;

            return new PropertyPath(names);
        }

        /// <summary>
        /// Extracts the property name from a property expression.
        /// </summary>
        /// <param name="propertyExpression">The property expression (e.g. p => p.PropertyName)</param>
        /// <param name="onlyPropertyName">是否只用返回路径最后的属性名</param>
        /// <param name="names">路径中包含的属性名数组，至少包含一个值</param>
        /// <param name="ex">如果提取失败，则返回失败原因</param>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="propertyExpression"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the expression is:<br/>
        ///     Not a <see cref="MemberExpression"/><br/>
        ///     The <see cref="MemberExpression"/> does not represent a property.<br/>
        ///     Or, the property is static.
        /// </exception>
        internal static bool TryExtractPropertyPath(LambdaExpression propertyExpression, bool onlyPropertyName, out List<PropertyInfo> names, out Exception ex)
        {
            names = null;
            ex = null;

            if (propertyExpression == null)
            {
                ex = new ArgumentNullException("propertyExpression");
                return false;
            }

            var memberExpression = propertyExpression.Body as MemberExpression;
            var namesTmp = new List<PropertyInfo>();
            while (true)
            {
                if (memberExpression == null)
                {
                    if (namesTmp.Count > 0) break;
                    else
                    {
                        ex = new ArgumentException("The expression is not a member access expression.", "propertyExpression");
                        return false;
                    }
                }

                var property = memberExpression.Member as PropertyInfo;
                if (property == null)
                {
                    if (namesTmp.Count > 0) break;
                    else
                    {
                        ex = new ArgumentException("The member access expression does not access a property.", "propertyExpression");
                        return false;
                    }
                }

                var getMethod = property.GetMethod;
                if (getMethod.IsStatic)
                {
                    if (namesTmp.Count > 0) break;
                    else
                    {
                        ex = new ArgumentException("The referenced property is a static property.", "propertyExpression");
                        return false;
                    }
                }

                namesTmp.Insert(0, property);

                if (namesTmp.Count > 0 && onlyPropertyName) break;

                memberExpression = memberExpression.Expression as MemberExpression;
            }

            names = namesTmp;
            return true;
        }
    }
}
