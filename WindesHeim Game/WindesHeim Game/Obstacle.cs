using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindesHeim_Game {

    public class Obstacle : GameObject 
    {
        public Obstacle(Point location, int height, int width) : base (location, height, width)
        {
            
        }

        protected double GetDistance(Point q) {
            double a = Location.X - q.X;
            double b = Location.Y - q.Y;
            double distance = Math.Sqrt(a * a + b * b);
            return distance;
        }
    }
}
