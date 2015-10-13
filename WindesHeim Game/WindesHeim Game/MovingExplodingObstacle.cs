using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindesHeim_Game.Properties;

namespace WindesHeim_Game {

    class MovingExplodingObstacle : Obstacle {

        public MovingExplodingObstacle(Point location, int height, int width) : base (location, height, width)
        {
            base.ObjectImage = Resources.IconBike;
            base.collisionSize = 50;
            base.Name = "Fietser";
            base.Description = "Volgt character en ontploft";
            base.PanelIcon = Resources.bikeEdited;
        }
    }
}
