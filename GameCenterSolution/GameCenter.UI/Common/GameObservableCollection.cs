using GameCenter.Library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace GameCenter.UI
{
    /// <summary>
    /// 可视游戏集合辅助类，不支持线程安全
    /// </summary>
    internal class GameObservableCollection : ICollection<Game>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private ObservableCollection<Game> _games = new ObservableCollection<Game>();
        /// <summary>
        /// 辅助进行查询操作，在游戏数量很大时有优势
        /// </summary>
        private Dictionary<GameID, Game> _dic = new Dictionary<GameID, Game>();

        public int Count
        {
            get { return _games.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public Game this[GameID id]
        {
            get
            {
                if (!_dic.ContainsKey(id)) throw new ArgumentException($"Game not exist:{id}");

                return _dic[id];
            }
            set
            {
                if (!_dic.ContainsKey(id)) throw new ArgumentException($"Game not exist:{id}");

                var game = _dic[id];
                int index = _games.IndexOf(game);

                _dic[id] = value;
                _games[index] = value;
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add { _games.CollectionChanged += value; }
            remove { _games.CollectionChanged -= value; }
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { ((INotifyPropertyChanged)_games).PropertyChanged += value; }
            remove { ((INotifyPropertyChanged)_games).PropertyChanged -= value; }
        }

        public void AddRange(IEnumerable<Game> array)
        {
            foreach (var g in array)
            {
                Add(g, false);
            }
        }

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
        public bool Add(Game game, bool ignoreIfExists)
        {
            if (game == null) throw new ArgumentNullException("item");
            if (game.ID == null) throw new ArgumentException("ID is null");

            if (_dic.ContainsKey(game.ID))
            {
                if (ignoreIfExists) return false;
                else throw new ArgumentException($"Game already exists:{game.ID}");
            }

            _dic.Add(game.ID, game);
            _games.Add(game);

            return true;
        }

        public bool Remove(Game game)
        {
            if (game == null) throw new ArgumentNullException("item");
            if (game.ID == null) throw new ArgumentException("ID is null");

            return Remove(game.ID);
        }

        public bool Remove(GameID id)
        {
            if (id == null) throw new ArgumentNullException("id");

            if (!_dic.ContainsKey(id)) return false;

            var game = _dic[id];
            _dic.Remove(id);
            _games.Remove(game);

            return true;
        }

        public void Clear()
        {
            _dic.Clear();
            _games.Clear();
        }

        public bool Contains(Game game)
        {
            if (game == null) throw new ArgumentNullException("item");
            if (game.ID == null) throw new ArgumentException("ID is null");

            return _dic.ContainsKey(game.ID);
        }

        public bool Contains(GameID id)
        {
            if (id == null) throw new ArgumentNullException("id");

            return _dic.ContainsKey(id);
        }

        public void CopyTo(Game[] array, int arrayIndex)
        {
            ((ICollection<Game>)_games).CopyTo(array, arrayIndex);
        }

        public IEnumerator<Game> GetEnumerator()
        {
            return _games.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _games.GetEnumerator();
        }
    }
}
