using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindesHeim_Game {

    class MovingExplodingObstacle : Obstacle {

        public MovingExplodingObstacle(Point location, int height, int width) : base (location, height, width)
        {
            base.ImageURL = AppDomain.CurrentDomain.BaseDirectory + "..\\..\\resources\\IconBike.png";
            base.collisionSize = 50;
            base.Name = "Fietser";
            base.Description = "Volgt character en ontploft";
        }
    }
}
