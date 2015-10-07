using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindesHeim_Game {

    class ExplodingObstacle : Obstacle {

        public ExplodingObstacle(Point location, int height, int width) : base (location, height, width)
        {
            base.ImageURL = AppDomain.CurrentDomain.BaseDirectory + "..\\..\\resources\\IconCar.png";
        }

        public bool CollidesWith(Player player) {
            if (GetDistance(player.Location) < 40) {
                return true;
            }
            else {
                return false;
            }
        }
    }
}
