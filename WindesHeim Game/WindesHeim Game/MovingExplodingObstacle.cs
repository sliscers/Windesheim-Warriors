﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindesHeim_Game {

    class MovingExplodingObstacle : Obstacle {

        public MovingExplodingObstacle(Point location, int height, int width) : base (location, height, width)
        {
            base.ImageURL = AppDomain.CurrentDomain.BaseDirectory + "..\\..\\resources\\IconBike.png";
            base.Name = "Fietser";
            base.Description = "Volgt + ontploft.";
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

        public bool CollidesWith(Player player) {
            if(GetDistance(player.Location) < 50) {
                return true;
            }
            else {
                return false;
            }
        }
    }
}
