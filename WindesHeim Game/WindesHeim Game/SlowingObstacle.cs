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
           
            base.Name = "Eerstejaars studenten";
            base.Description = "Volgt character en vertraagt";
            base.CollisionX = 10;
            base.CollisionY = 10;
            base.PanelIcon = global::WindesHeim_Game.Properties.Resources.IconESEdited;
        }


    }
}
