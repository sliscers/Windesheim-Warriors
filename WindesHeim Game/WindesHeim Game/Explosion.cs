using System;
using System.Drawing;
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
