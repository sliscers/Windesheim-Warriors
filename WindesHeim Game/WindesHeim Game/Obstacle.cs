using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindesHeim_Game {

    public class Obstacle : GameObject 
    {
        private string name;
        private string description;

        public Obstacle(Point location, int height, int width) : base (location, height, width)
        {
            
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public void ChasePlayer(Player player) {
            if (Location.X >= player.Location.X)
                Location = new Point(Location.X - 1, Location.Y);

            if (Location.X <= player.Location.X)
                Location = new Point(Location.X + 1, Location.Y);

            if (Location.Y >= player.Location.Y)
                Location = new Point(Location.X, Location.Y - 1);

            if (Location.Y <= player.Location.Y)
                Location = new Point(Location.X, Location.Y + 1);
        }
    }
}
