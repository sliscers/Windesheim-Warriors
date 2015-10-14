using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindesHeim_Game.Properties;

namespace WindesHeim_Game {

    class SlowingObstacle : Obstacle {

        private int slowingSpeed;

        public SlowingObstacle(Point location, int height, int width) : base (location, height, width)
        {
            base.ObjectImage = Resources.IconES;
           
            base.Name = "Eerstejaars studenten";
            base.Description = "Volgt character en vertraagt";
            base.CollisionX = 10;
            base.CollisionY = 10;
            base.PanelIcon = Resources.IconESEdited;

            // Standaard player nieuwe speed is 2
            this.slowingSpeed = 2;
        }

        public int SlowingSpeed {
            get { return slowingSpeed; }
            set { slowingSpeed = value; }
        }


    }
}
