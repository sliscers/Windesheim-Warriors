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
            base.collisionSize = 40;
            base.Name = "Auto";
            base.Description = "Ontploft bij aanraking";
            base.PanelIcon = global::WindesHeim_Game.Properties.Resources.carEdited;
        }
    }
}
