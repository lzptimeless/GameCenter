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
using AppCore;
using System.Threading;
using System.Windows.Threading;

namespace GameCenter.UI
{
    /// <summary>
    /// 可视游戏集合辅助类，不支持线程安全
    /// </summary>
    internal class GameObservableCollection : ICollection<GameViewModel>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        #region classes
        private enum GameEventDataFlags
        {
            Added,
            Removed,
            Updated
        }

        private class MixGameEventData
        {
            public MixGameEventData(GameEventDataFlags flag, Game game)
            {
                Flag = flag;
                Game = game;
            }

            public MixGameEventData(GameEventDataFlags flag, Game game, IReadOnlyList<PropertyPath> properties)
            {
                Flag = flag;
                Game = game;
                UpdatedProperties = properties;
            }

            public GameEventDataFlags Flag { get; private set; }
            public Game Game { get; private set; }
            public IReadOnlyList<PropertyPath> UpdatedProperties { get; private set; }

            public override string ToString()
            {
                return $"{Flag}, {Game}";
            }
        }
        #endregion

        public GameObservableCollection()
        {
            _syncEventObj = new object();
            _uiDispatcher = Dispatcher.CurrentDispatcher;
            _games = new ObservableCollection<GameViewModel>();
            _dic = new Dictionary<GameID, GameViewModel>();
            _eventDatas = new List<EventData>();
        }

        /// <summary>
        /// 用于UI显示的游戏集合，提供UI所需的通知功能
        /// </summary>
        private ObservableCollection<GameViewModel> _games;
        /// <summary>
        /// 辅助进行查询操作，在游戏数量很大时有优势
        /// </summary>
        private Dictionary<GameID, GameViewModel> _dic;
        /// <summary>
        /// 用以暂时存储UI线程来不及处理的事件数据
        /// </summary>
        private List<EventData> _eventDatas;
        /// <summary>
        /// 是否正在处理事件数据
        /// </summary>
        private bool _isProcessingEvent;
        /// <summary>
        /// UI线程任务管理器
        /// </summary>
        private Dispatcher _uiDispatcher;
        /// <summary>
        /// 事件处理同步对象
        /// </summary>
        private readonly object _syncEventObj;

        public int Count
        {
            get { return _games.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// 获取游戏：游戏不存在时返回null
        /// 设置游戏：游戏不存在时抛出异常
        /// </summary>
        /// <param name="id">游戏ID</param>
        /// <returns>id代表的游戏，游戏不存在时返回null</returns>
        /// <exception cref="ArgumentException">设置游戏时，游戏不存在时抛出的异常</exception>
        /// <exception cref="ArgumentNullException">参数id，value为null时抛出的异常</exception>
        public GameViewModel this[GameID id]
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
                if (!_dic.ContainsKey(id)) throw new ArgumentException($"Game not exist:{id}");

                var game = _dic[id];
                int index = _games.IndexOf(game);

                _dic[id] = value;
                _games[index] = value;
            }
        }

        /// <summary>
        /// 游戏集合改变事件
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add { _games.CollectionChanged += value; }
            remove { _games.CollectionChanged -= value; }
        }

        /// <summary>
        /// 游戏集合属性改变事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged
        {
            add { ((INotifyPropertyChanged)_games).PropertyChanged += value; }
            remove { ((INotifyPropertyChanged)_games).PropertyChanged -= value; }
        }

        /// <summary>
        /// 绑定<see cref="ILibrary"/>，自动监控游戏改变相关事件
        /// </summary>
        /// <param name="library">游戏库接口</param>
        public void BindLibrary(ILibrary library)
        {
            library.GameAddedEvent.Subscribe(OnGameAdded, ThreadOption.PublisherThread);
            library.GameRemovedEvent.Subscribe(OnGameRemoved, ThreadOption.PublisherThread);
            library.GameUpdatedEvent.Subscribe(OnGameUpdated, ThreadOption.PublisherThread);

            var games = library.GetGames();
            AddRange(games);
        }

        /// <summary>
        /// 添加游戏
        /// </summary>
        /// <param name="game">游戏</param>
        /// <exception cref="ArgumentException">game.ID为null，游戏已经存在时抛出异常</exception>
        /// <exception cref="ArgumentNullException">game为null时抛出异常</exception>
        public void Add(GameViewModel game)
        {
            Add(game, false);
        }

        /// <summary>
        /// 添加游戏
        /// </summary>
        /// <param name="game">游戏</param>
        /// <param name="ignoreIfExists">如果游戏已经存在，true:忽略这次操作，false:抛出异常</param>
        /// <returns>true:添加成功，false:游戏已经存在</returns>
        /// <exception cref="ArgumentException">game.ID为null，ignoreIfExists为false游戏已经存在时抛出异常</exception>
        /// <exception cref="ArgumentNullException">game为null时抛出异常</exception>
        public bool Add(Game game, bool ignoreIfExists)
        {
            return Add(new GameViewModel(game), ignoreIfExists);
        }

        /// <summary>
        /// 添加游戏
        /// </summary>
        /// <param name="game">游戏</param>
        /// <param name="ignoreIfExists">如果游戏已经存在，true:忽略这次操作，false:抛出异常</param>
        /// <returns>true:添加成功，false:游戏已经存在</returns>
        /// <exception cref="ArgumentException">game.ID为null，ignoreIfExists为false游戏已经存在时抛出异常</exception>
        /// <exception cref="ArgumentNullException">game为null时抛出异常</exception>
        public bool Add(GameViewModel game, bool ignoreIfExists)
        {
            if (game == null) throw new ArgumentNullException("game");
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

        /// <summary>
        /// 添加多个游戏
        /// </summary>
        /// <param name="array">游戏集合</param>
        /// <exception cref="ArgumentException">集合中game.ID为null，游戏已经存在时抛出异常</exception>
        /// <exception cref="ArgumentNullException">集合中有null时抛出异常</exception>
        public void AddRange(IEnumerable<Game> array)
        {
            foreach (var g in array)
            {
                Add(g, false);
            }
        }

        /// <summary>
        /// 移除游戏，移除成功返回true，游戏不存在返回false
        /// </summary>
        /// <param name="game">游戏</param>
        /// <returns>移除成功返回true，游戏不存在返回false</returns>
        /// <exception cref="ArgumentNullException">game为null时抛出的异常</exception>
        /// <exception cref="ArgumentException">game.ID为null时抛出的异常</exception>
        public bool Remove(GameViewModel game)
        {
            if (game == null) throw new ArgumentNullException("game");
            if (game.ID == null) throw new ArgumentException("ID is null");

            return Remove(game.ID);
        }

        /// <summary>
        /// 移除游戏，移除成功返回true，游戏不存在返回false
        /// </summary>
        /// <param name="game">游戏</param>
        /// <returns>移除成功返回true，游戏不存在返回false</returns>
        /// <exception cref="ArgumentNullException">game为null时抛出的异常</exception>
        /// <exception cref="ArgumentException">game.ID为null时抛出的异常</exception>
        public bool Remove(Game game)
        {
            if (game == null) throw new ArgumentNullException("game");
            if (game.ID == null) throw new ArgumentException("ID is null");

            return Remove(game.ID);
        }

        /// <summary>
        /// 移除游戏，移除成功返回true，游戏不存在返回false
        /// </summary>
        /// <param name="id">游戏ID</param>
        /// <returns>移除成功返回true，游戏不存在返回false</returns>
        /// <exception cref="ArgumentNullException">id为null时抛出的异常</exception>
        public bool Remove(GameID id)
        {
            if (id == null) throw new ArgumentNullException("id");

            if (!_dic.ContainsKey(id)) return false;

            var game = _dic[id];
            _dic.Remove(id);
            _games.Remove(game);

            return true;
        }

        /// <summary>
        /// 清空所有游戏
        /// </summary>
        public void Clear()
        {
            _dic.Clear();
            _games.Clear();
        }

        /// <summary>
        /// 游戏是否存在
        /// </summary>
        /// <param name="game">游戏</param>
        /// <returns>true:游戏存在，false:游戏不存在</returns>
        /// <exception cref="ArgumentException">game.ID为null时抛出的异常</exception>
        /// <exception cref="ArgumentNullException">game为null时抛出的异常</exception>
        public bool Contains(GameViewModel game)
        {
            if (game == null) throw new ArgumentNullException("game");
            if (game.ID == null) throw new ArgumentException("ID is null");

            return _dic.ContainsKey(game.ID);
        }

        /// <summary>
        /// 游戏是否存在
        /// </summary>
        /// <param name="game">游戏</param>
        /// <returns>true:游戏存在，false:游戏不存在</returns>
        /// <exception cref="ArgumentException">game.ID为null时抛出的异常</exception>
        /// <exception cref="ArgumentNullException">game为null时抛出的异常</exception>
        public bool Contains(Game game)
        {
            if (game == null) throw new ArgumentNullException("game");
            if (game.ID == null) throw new ArgumentException("ID is null");

            return _dic.ContainsKey(game.ID);
        }

        /// <summary>
        /// 游戏是否存在
        /// </summary>
        /// <param name="id">游戏ID</param>
        /// <returns>true:游戏存在，false:游戏不存在</returns>
        /// <exception cref="ArgumentNullException">id为null时抛出的异常</exception>
        public bool Contains(GameID id)
        {
            if (id == null) throw new ArgumentNullException("id");

            return _dic.ContainsKey(id);
        }

        public void CopyTo(GameViewModel[] array, int arrayIndex)
        {
            ((ICollection<GameViewModel>)_games).CopyTo(array, arrayIndex);
        }

        public IEnumerator<GameViewModel> GetEnumerator()
        {
            return _games.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _games.GetEnumerator();
        }

        private void OnGameAdded(GameAddedEventData obj)
        {
            lock (_syncEventObj)
            {
                _eventDatas.Add(obj);
            }
            ProcessingEventData();
        }

        private void OnGameRemoved(GameRemovedEventData obj)
        {
            lock (_syncEventObj)
            {
                _eventDatas.Add(obj);
            }
            ProcessingEventData();
        }

        private void OnGameUpdated(GameUpdatedEventData obj)
        {
            lock (_syncEventObj)
            {
                _eventDatas.Add(obj);
            }
            ProcessingEventData();
        }

        private void ProcessingEventData()
        {
            lock (_syncEventObj)
            {
                if (_isProcessingEvent) return;

                _isProcessingEvent = true;
            }

            _uiDispatcher.BeginInvoke(new Action(() =>
            {
                while (true)
                {
                    // 获取事件数据
                    List<EventData> eventDatas = new List<EventData>();
                    lock (_syncEventObj)
                    {
                        if (_eventDatas.Count > 0)
                        {
                            eventDatas.AddRange(_eventDatas);
                            _eventDatas.Clear();
                        }
                        else
                        {
                            // 重置处理事件状态标识
                            _isProcessingEvent = false;
                            break;
                        }
                    }// lock (_syncEventObj)

                    // 预处理，合并同一个游戏的事件数据，以减少对UI的改变次数
                    List<EventData> pretreatedEventDatas = PretreatEventDatas(eventDatas);

                    // 处理事件数据
                    foreach (var ed in pretreatedEventDatas)
                    {
                        if (ed is GameAddedEventData) OnUIGameAdded(ed as GameAddedEventData);
                        else if (ed is GameRemovedEventData) OnUIGameRemoved(ed as GameRemovedEventData);
                        else if (ed is GameUpdatedEventData) OnUIGameUpdated(ed as GameUpdatedEventData);
                        else throw new InvalidOperationException($"Not support event data type:{ed.GetType().FullName}");
                    }
                }// while true
            }));// _uiDispatcher.BeginInvoke
        }

        private void OnUIGameAdded(GameAddedEventData obj)
        {
            Add(obj.Model, true);
        }

        private void OnUIGameRemoved(GameRemovedEventData obj)
        {
            Remove(obj.Model);
        }

        private void OnUIGameUpdated(GameUpdatedEventData obj)
        {
            if (Contains(obj.Model))
                this[obj.Model.ID].Update(obj.Model, obj.UpdatedPropertyPaths?.ToArray());
        }

        private List<EventData> PretreatEventDatas(List<EventData> eventDatas)
        {
            List<EventData> pretreatedEventDatas = new List<EventData>();
            while (eventDatas.Count > 0)
            {
                EventData ed = eventDatas[0];
                eventDatas.RemoveAt(0);

                for (int i = 0; i < eventDatas.Count; i++)
                {
                    EventData edi = eventDatas[i];
                    if (MergeEventData(ed, edi, ref ed))
                    {
                        eventDatas.RemoveAt(i);
                        i--;
                    }
                }

                pretreatedEventDatas.Add(ed);
            }

            return pretreatedEventDatas;
        }

        private bool MergeEventData(EventData before, EventData after, ref EventData merged)
        {
            // 注意：合并原则并不是优先更新数据到最新，而是模拟出在不使用预处理情况下同样的结果
            var mixBefore = EventDataToMixGameEventData(before);
            var mixAfter = EventDataToMixGameEventData(after);

            if (mixBefore.Game.ID == mixAfter.Game.ID)
            {
                if (mixAfter.Flag == GameEventDataFlags.Added)
                {
                    if (mixBefore.Flag == GameEventDataFlags.Added) merged = before;
                    else if (mixBefore.Flag == GameEventDataFlags.Removed)
                    {
                        if (Contains(mixBefore.Game))
                            merged = new GameUpdatedEventData(mixAfter.Game, null);
                        else
                            merged = after;
                    }
                    else if (mixBefore.Flag == GameEventDataFlags.Updated)
                    {
                        if (Contains(mixBefore.Game))
                            merged = before;
                        else
                            merged = after;
                    }
                    else throw new InvalidOperationException($"Not support GameEventDataFlags:{mixBefore.Flag}");
                }
                else if (mixAfter.Flag == GameEventDataFlags.Removed)
                    merged = after;
                else if (mixAfter.Flag == GameEventDataFlags.Updated)
                {
                    if (mixBefore.Flag == GameEventDataFlags.Added)
                    {
                        if (Contains(mixBefore.Game))
                            merged = after;
                        else
                            merged = new GameAddedEventData(mixAfter.Game);
                    }
                    else if (mixBefore.Flag == GameEventDataFlags.Removed)
                        merged = before;
                    else if (mixBefore.Flag == GameEventDataFlags.Updated)
                        merged = after;
                }
                else throw new InvalidOperationException($"Not support GameEventDataFlags:{mixAfter.Flag}");

                return true;
            }// before game.ID == after game.ID
            else return false;
        }

        private MixGameEventData EventDataToMixGameEventData(EventData ed)
        {
            if (ed == null) throw new ArgumentNullException("ed");

            if (ed is GameAddedEventData)
            {
                return new MixGameEventData(GameEventDataFlags.Added, (ed as GameAddedEventData).Model);
            }
            else if (ed is GameRemovedEventData)
            {
                return new MixGameEventData(GameEventDataFlags.Removed, (ed as GameRemovedEventData).Model);
            }
            else if (ed is GameUpdatedEventData)
            {
                var updatedEd = ed as GameUpdatedEventData;
                return new MixGameEventData(GameEventDataFlags.Updated, updatedEd.Model, updatedEd.UpdatedPropertyPaths);
            }
            else throw new ArgumentException($"Not support game event data type:{ed.GetType().FullName}");
        }
    }
}
