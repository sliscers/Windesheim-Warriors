﻿using System.Drawing;
using WindesHeim_Game.Properties;

namespace WindesHeim_Game {

    class ExplodingObstacle : Obstacle {

        public ExplodingObstacle(Point location, int height, int width) : base (location, height, width)
        {
            base.ObjectImage = Resources.IconCar;
            base.CollisionX = 0;
            base.CollisionY = -8;
            base.Name = "Car";
            base.Description = "Detonates on collision";
            base.PanelIcon = Resources.carEdited;
        }


    }
}
