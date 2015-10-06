using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindesHeim_Game
{
    class PlayerController : Controller
    {
        PlayerModel model;
        public PlayerController(GameWindow gameWindow) : base(gameWindow)
        {

        }

        public void SetPosition()
        {
            model.Location.X = 15;
        }
    }
}
