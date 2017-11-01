using AppCore;
using GameCenter.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GameCenter.UI.Pages
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : UserControl, IPage
    {
        public Home()
        {
            InitializeComponent();
            _games = new GameObservableCollection();
            GamesListBox.ItemsSource = _games;
        }

        private GameObservableCollection _games;

        public void Initialize(NavigationParameters parameters)
        {
            HomeParameters homeParams = null;
            if (parameters != null)
            {
                homeParams = parameters as HomeParameters;
                if (homeParams == null) throw new ArgumentException($"parameters is not {typeof(HomeParameters).FullName}");
            }

            ILibrary library = Core.Instance.ModuleManager.GetModule<ILibrary>();
            var games = library.GetGames();
            // 在UI线程上注册游戏改变事件
            library.GameAddedEvent.Subscribe(OnGameAdded, ThreadOption.UIThread);
            library.GameRemovedEvent.Subscribe(OnGameRemoved, ThreadOption.UIThread);
            library.GameUpdatedEvent.Subscribe(OnGameUpdated, ThreadOption.UIThread);
            // 如果有重复游戏会崩溃，但也不用担心游戏改变事件与以下代码同时执行导致崩溃，
            // 由于以上游戏改变事件都注册在UIThread，所以不用担心以下代码在执行时，游戏改
            // 变事件也在同时执行
            _games.AddRange(games);
        }

        private void OnGameAdded(GameAddedEventData obj)
        {
            _games.Add(obj.Game, true);
        }

        private void OnGameRemoved(GameRemovedEventData obj)
        {
            _games.Remove(obj.Game);
        }

        private void OnGameUpdated(GameUpdatedEventData obj)
        {
            if (_games.Contains(obj.Game.ID))
                _games[obj.Game.ID] = obj.Game;
        }

        public void OnComeback()
        {
        }

        public void OnLeave()
        {
        }

        public void Release()
        {
        }
    }

    public class HomeParameters : NavigationParameters
    {

    }
}
