using AppCore;
using GameCenter.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCenter.UI
{
    internal class GameViewModel : BindableModel<Game>
    {
        public GameViewModel(Game game)
        {
            Update(game, null);
        }

        #region ID
        private GameID _id;
        /// <summary>
        /// 游戏ID
        /// </summary>
        public GameID ID
        {
            get { return _id; }
            set { SetProperty(ref _id, value, false); }
        }
        #endregion

        #region Name
        private string _name;
        /// <summary>
        /// 游戏名
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value, false); }
        }
        #endregion

        #region CoverCapsule
        private string _coverCapsule;
        /// <summary>
        /// 简略游戏封面，用于列表模式
        /// </summary>
        public string CoverCapsule
        {
            get { return _coverCapsule; }
            set { SetProperty(ref _coverCapsule, value, false); }
        }
        #endregion

        #region CoverHeader
        private string _coverHeader;
        /// <summary>
        /// 游戏封面，用于缩略图模式
        /// </summary>
        public string CoverHeader
        {
            get { return _coverHeader; }
            set { SetProperty(ref _coverHeader, value, false); }
        }
        #endregion

        protected override IReadOnlyList<PropertyBindingBase<Game>> CreatePropertyBindings()
        {
            PropertyBindingCollection<Game, GameViewModel> bindings = new PropertyBindingCollection<Game, GameViewModel>();

            bindings.Add(g => g.ID, (vm, id) => vm.ID = id);
            bindings.Add(g => g.Name, (vm, name) => vm.Name = name);
            bindings.Add(g => g.Cover.Capsule, (vm, capsule) => vm.CoverCapsule = capsule);
            bindings.Add(g => g.Cover.Header, (vm, header) => vm.CoverHeader = header);

            return bindings;
        }
    }
}
