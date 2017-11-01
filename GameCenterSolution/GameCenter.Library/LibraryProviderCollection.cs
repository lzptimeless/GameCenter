using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCenter.Library
{
    /// <summary>
    /// <see cref="ILibraryProvider"/>集合辅助类，不支持线程安全
    /// </summary>
    internal class LibraryProviderCollection : ICollection<ILibraryProvider>
    {
        private List<ILibraryProvider> _items = new List<ILibraryProvider>();

        public int Count
        {
            get { return _items.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// 获取平台，如果不存在则返回null
        /// 设置平台，如果不存在则添加，存在则覆盖
        /// </summary>
        /// <param name="platformFlag">平台标记</param>
        /// <returns>返回平台，如果不存在则返回null</returns>
        /// <exception cref="ArgumentNullException">如果value=null则抛出异常</exception>
        public ILibraryProvider this[GamePlatformFlags platformFlag]
        {
            get
            {
                int index = IndexOf(platformFlag);
                if (index == -1) return null;

                return _items[index];
            }
            set
            {
                if (value == null) throw new ArgumentNullException("value");

                int index = IndexOf(platformFlag);
                if (index == -1)
                {
                    _items.Add(value);
                }
                else
                {
                    _items[index] = value;
                }
            }
        }

        /// <summary>
        /// 添加平台
        /// </summary>
        /// <param name="item">平台</param>
        /// <exception cref="ArgumentNullException">item为空时抛出异常</exception>
        /// <exception cref="ArgumentException">平台已经存在时抛出异常</exception>
        public void Add(ILibraryProvider item)
        {
            if (item == null) throw new ArgumentNullException("item");
            if (Contains(item.PlatformFlag)) throw new ArgumentException($"Platform already exists:{item.PlatformFlag}");

            _items.Add(item);
        }

        /// <summary>
        /// 移除平台
        /// </summary>
        /// <param name="item">平台</param>
        /// <returns>true:平台移除成功，false:平台不存在</returns>
        public bool Remove(ILibraryProvider item)
        {
            return Remove(item.PlatformFlag);
        }

        /// <summary>
        /// 移除平台
        /// </summary>
        /// <param name="platformFlag">平台标签</param>
        /// <returns>true:平台移除成功，false:平台不存在</returns>
        public bool Remove(GamePlatformFlags platformFlag)
        {
            int index = IndexOf(platformFlag);

            if (index == -1) return false;

            _items.RemoveAt(index);
            return true;
        }

        /// <summary>
        /// 查找平台所在的索引
        /// </summary>
        /// <param name="platformFlag">平台标记</param>
        /// <returns>返回平台索引，没有找到返回-1</returns>
        public int IndexOf(GamePlatformFlags platformFlag)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i].PlatformFlag == platformFlag)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// 清空平台集合
        /// </summary>
        public void Clear()
        {
            _items.Clear();
        }

        /// <summary>
        /// 平台是否已经存在
        /// </summary>
        /// <param name="item">平台</param>
        /// <returns>true:平台已经存在，false:平台不存在</returns>
        public bool Contains(ILibraryProvider item)
        {
            return Contains(item.PlatformFlag);
        }

        /// <summary>
        /// 平台是否已经存在
        /// </summary>
        /// <param name="platformFlag">平台标记</param>
        /// <returns>true:平台已经存在，false:平台不存在</returns>
        public bool Contains(GamePlatformFlags platformFlag)
        {
            foreach (var lp in _items)
            {
                if (lp.PlatformFlag == platformFlag) return true;
            }

            return false;
        }

        public void CopyTo(ILibraryProvider[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        public IEnumerator<ILibraryProvider> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }
    }
}
