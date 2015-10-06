using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindesHeim_Game {

    class Explosion : GameObject {

        private DateTime timeStamp = DateTime.Now;

        public Explosion(Point location, int height, int width) : base (location, width, height)
        {
            base.ImageURL = AppDomain.CurrentDomain.BaseDirectory + "..\\..\\resources\\explosion.png";
        }

        public DateTime TimeStamp {
            get { return timeStamp; }
        }
    }
}
