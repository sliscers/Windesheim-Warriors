using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindesHeim_Game.Properties;

namespace WindesHeim_Game {

    class Explosion : GameObject
    {

        private DateTime timeStamp = DateTime.Now;

        public Explosion(Point location, int height, int width) : base(location, height, width)
        {
            base.ObjectImage = Resources.explosion;

        }

        public DateTime TimeStamp
        {
            get { return timeStamp; }
        }
    }
}
