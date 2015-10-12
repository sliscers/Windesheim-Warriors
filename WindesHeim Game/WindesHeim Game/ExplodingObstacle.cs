using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindesHeim_Game.Properties;

namespace WindesHeim_Game {

    class ExplodingObstacle : Obstacle {

        public ExplodingObstacle(Point location, int height, int width) : base (location, height, width)
        {
            base.ObjectImage = Resources.IconCar;
            base.CollisionX = 0;
            base.CollisionY = 0;
            base.Name = "Auto";
            base.Description = "Ontploft bij aanraking";
            base.PanelIcon = global::WindesHeim_Game.Properties.Resources.carEdited;
        }


    }
}
