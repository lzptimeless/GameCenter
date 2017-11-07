using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AppCore
{
    public class PropertyPath
    {
        public PropertyPath(IEnumerable<PropertyInfo> names)
        {
            if (names == null || !names.Any()) throw new ArgumentException("names can not be null or empty.");

            var localNames = new List<PropertyInfo>();
            Names = localNames;
            foreach (var propertyInfo in names)
            {
                if (propertyInfo == null) throw new ArgumentException("names contains null item.");

                _hashCode += propertyInfo.MetadataToken;
                localNames.Add(propertyInfo);
            }
        }

        /// <summary>
        /// 缓存HashCode，节省计算HashCode的时间，在Equals方法中用来快速排除不相等的对象
        /// </summary>
        private int _hashCode;
        /// <summary>
        /// 路径中的各个属性名
        /// </summary>
        public IReadOnlyList<PropertyInfo> Names { get; private set; }

        public override bool Equals(object obj)
        {
            PropertyPath other = obj as PropertyPath;
            if (object.ReferenceEquals(other, null)) return false;
            // 通过HashCode快速排除不相等的对象
            if (_hashCode != other._hashCode) return false;

            return Equals(other.Names);
        }

        public bool Equals<T>(Expression<Func<T>> propertyExpression)
        {
            List<PropertyInfo> names;
            Exception ex;
            if (!PropertySupport.TryExtractPropertyPath(propertyExpression, false, out names, out ex))
                return false;

            return this.Equals(names);
        }

        public bool Equals(IEnumerable<PropertyInfo> names)
        {
            if (names == null || !names.Any()) return false;

            var otherIt = names.GetEnumerator();
            var it = Names.GetEnumerator();
            while (true)
            {
                bool otherNext = otherIt.MoveNext();
                if (otherNext != it.MoveNext()) return false;
                if (!otherNext) break;
                if (otherIt.Current == null) return false;
                if (otherIt.Current.MetadataToken != it.Current.MetadataToken) return false;
            }

            return true;
        }

        public bool IsAncestorOfOrEquals(PropertyPath children)
        {
            if (children == null) throw new ArgumentNullException("children");

            var childrenIt = children.Names.GetEnumerator();
            var it = Names.GetEnumerator();

            while (true)
            {
                bool childrenNext = childrenIt.MoveNext();
                bool next = it.MoveNext();

                if (!next && !childrenNext) break;
                if (next && !childrenNext) return false;
                if (!next && childrenNext) return true;
                if (it.Current == null && childrenIt.Current == null) continue;
                if (it.Current == null || childrenIt.Current == null) return false;
                if (it.Current.MetadataToken != childrenIt.Current.MetadataToken) return false;
            }

            return true;
        }

        public static bool operator ==(PropertyPath left, PropertyPath right)
        {
            if (object.ReferenceEquals(left, null) && object.ReferenceEquals(right, null))
                return true;

            if (object.ReferenceEquals(left, null) || object.ReferenceEquals(right, null))
                return false;

            return left.Equals(right);
        }

        public static bool operator !=(PropertyPath left, PropertyPath right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('[');
            sb.Append(Names[0].DeclaringType.FullName);
            sb.Append(']');
            foreach (var propertyInfo in Names)
            {
                sb.Append('.');
                sb.Append(propertyInfo.Name);
            }

            return sb.ToString();
        }
    }
}
