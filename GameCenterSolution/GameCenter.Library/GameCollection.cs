using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCenter.Library
{
    /// <summary>
    /// 辅助管理Game集合，确保游戏不重复，注意这个类不支持线程安全
    /// </summary>
    internal class GameCollection : ICollection<Game>
    {
        private Dictionary<GameID, Game> _dic = new Dictionary<GameID, Game>();

        /// <summary>
        /// 游戏数量
        /// </summary>
        public int Count
        {
            get { return _dic.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// 通过<see cref="GameID"/>获取游戏，如果游戏不存在则返回null
        /// 通过<see cref="GameID"/>修改游戏，如果游戏不存在则抛出异常，存在则修改
        /// </summary>
        /// <param name="id">游戏ID</param>
        /// <returns>游戏，游戏不存在返回null</returns>
        /// <exception cref="ArgumentException">修改游戏时，游戏不存在时抛出的异常</exception>
        /// <exception cref="ArgumentNullException">id为null时或value为null时抛出异常</exception>
        public Game this[GameID id]
        {
            get
            {
                if (id == null) throw new ArgumentNullException("id");

                if (!_dic.ContainsKey(id)) return null;

                return _dic[id];
            }
            set
            {
                if (id == null) throw new ArgumentNullException("id");
                if (value == null) throw new ArgumentNullException("value");

                if (_dic.ContainsKey(id)) _dic[id] = value;
                else throw new ArgumentException($"Game not exists:{id}");
            }
        }

        /// <summary>
        /// 添加游戏
        /// </summary>
        /// <param name="game">游戏</param>
        /// <exception cref="ArgumentNullException">game为null时抛出异常</exception>
        /// <exception cref="ArgumentException">如果游戏已经存在则抛出异常</exception>
        public void Add(Game game)
        {
            Add(game, false);
        }

        /// <summary>
        /// 添加游戏
        /// </summary>
        /// <param name="game">游戏</param>
        /// <param name="ignoreIfExists">如果游戏已经存在，true:忽略这次操作，false:抛出异常</param>
        /// <returns>true:添加成功，false:游戏已经存在</returns>
        /// <exception cref="ArgumentNullException">game为null时抛出异常</exception>
        /// <exception cref="ArgumentException">如果游戏已经存在并且ignoreIfExists=false则抛出异常</exception>
        public bool Add(Game game, bool ignoreIfExists)
        {
            if (game == null) throw new ArgumentNullException("game");

            if (_dic.ContainsKey(game.ID))
            {
                if (ignoreIfExists) return false;
                else throw new ArgumentException($"Game already exists:{game.ID}");
            }

            _dic.Add(game.ID, game);

            return true;
        }

        /// <summary>
        /// 移除游戏
        /// </summary>
        /// <param name="game">游戏</param>
        /// <returns>true:游戏存在并移除成功，false:游戏不存在</returns>
        /// <exception cref="ArgumentNullException">game为null时抛出异常</exception>
        public bool Remove(Game game)
        {
            if (game == null) throw new ArgumentNullException("game");

            return _dic.Remove(game.ID);
        }

        /// <summary>
        /// 移除游戏
        /// </summary>
        /// <param name="id">游戏ID</param>
        /// <returns>true:游戏存在并移除成功，false:游戏不存在</returns>
        /// <exception cref="ArgumentNullException">id为null时抛出异常</exception>
        public bool Remove(GameID id)
        {
            if (id == null) throw new ArgumentNullException("id");

            return _dic.Remove(id);
        }

        /// <summary>
        /// 清空所有游戏
        /// </summary>
        public void Clear()
        {
            _dic.Clear();
        }

        /// <summary>
        /// 是否已经存在这个游戏
        /// </summary>
        /// <param name="game">游戏</param>
        /// <returns>true:游戏已经存在，false:游戏不存在</returns>
        /// <exception cref="ArgumentNullException">game为null时抛出异常</exception>
        public bool Contains(Game game)
        {
            if (game == null) throw new ArgumentNullException("game");

            return _dic.ContainsKey(game.ID);
        }

        /// <summary>
        /// 是否已经存在这个游戏
        /// </summary>
        /// <param name="id">游戏ID</param>
        /// <returns>true:游戏已经存在，false:游戏不存在</returns>
        /// <exception cref="ArgumentNullException">id为null时抛出异常</exception>
        public bool Contains(GameID id)
        {
            if (id == null) throw new ArgumentNullException("id");

            return _dic.ContainsKey(id);
        }

        /// <summary>
        /// 复制游戏到指定数组
        /// </summary>
        /// <param name="array">复制目的地数组</param>
        /// <param name="arrayIndex">复制目的地起始索引</param>
        public void CopyTo(Game[] array, int arrayIndex)
        {
            ((ICollection<Game>)_dic.Values).CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 复制游戏数据到List
        /// </summary>
        /// <returns>游戏List</returns>
        public List<Game> CloneToList()
        {
            List<Game> list = new List<Game>(_dic.Count);
            foreach (var game in _dic.Values)
            {
                list.Add(game.DeepClone());
            }

            return list;
        }

        public IEnumerator<Game> GetEnumerator()
        {
            return _dic.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dic.Values.GetEnumerator();
        }

        public override string ToString()
        {
            return $"Count={_dic.Count}";
        }
    }
}
