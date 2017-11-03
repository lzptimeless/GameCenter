using GameCenter.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCenter.UI
{
    internal class GameViewModel
    {
        public GameViewModel(Game game)
        { }

        private Game _game;



        public void Update(Game newGame, GameUpdatedFields fields)
        {
            _game = newGame;
        }
    }
}
