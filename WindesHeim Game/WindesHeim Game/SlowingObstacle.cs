using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindesHeim_Game {

    class SlowingObstacle : Obstacle {

        public SlowingObstacle(Point location, int height, int width) : base (location, height, width)
        {
            base.ImageURL = AppDomain.CurrentDomain.BaseDirectory + "..\\..\\resources\\IconES.png";
            base.collisionSize = 75;
            base.Name = "Eerstejaars studenten";
            base.Description = "Volgen + vertragen";
        }
    }
}
