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
        private Image panelIcon;
        private int movingSpeed;
        private bool smartMovingEnabled;
        private string smartmovingDirection;
        private DateTime smartmovingTime;
        private bool isSmart = true;

        public Obstacle(Point location, int height, int width) : base (location, height, width)
        {
            
        }
        public string SmartmovingDirection
        {
            get { return smartmovingDirection; }
            set { smartmovingDirection = value; }
        }

        public DateTime SmartmovingTime
        {
            get { return smartmovingTime; }
            set { smartmovingTime = value; }
        }

        public bool SmartMovingEnabled
        {
            get { return smartMovingEnabled; }
            set { smartMovingEnabled = value; }
        }

        public bool IsSmart
        {
            get { return isSmart; }
            set { isSmart = value; }
        }

        public int MovingSpeed {
            get { return movingSpeed; }
            set { movingSpeed = value; }
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

        public Image PanelIcon
        {
            get { return panelIcon; }
            set { panelIcon = value; }
        }

        public void ChasePlayer(Player player) {
            if (Location.X >= player.Location.X)
                Location = new Point(Location.X - 1 - movingSpeed, Location.Y);

            if (Location.X <= player.Location.X)
                Location = new Point(Location.X + 1 + movingSpeed, Location.Y);

            if (Location.Y >= player.Location.Y)
                Location = new Point(Location.X, Location.Y - 1 - movingSpeed);

            if (Location.Y <= player.Location.Y)
                Location = new Point(Location.X, Location.Y + 1 + movingSpeed);
        }

        public void TryToEscape()
        {
            Console.WriteLine((Location.X) + "::" + (Location.Y));
            if ((Location.X) >= 800)
            {                
                smartmovingDirection = "right";
            }
            if ((Location.Y) >= 430)
            {
                smartmovingDirection = "down";
            }
            Console.WriteLine(smartmovingDirection);
            switch (smartmovingDirection)
                {
                    case "up": //Als collision aan de bovenkant, beweeg naar beneden
                    Location = new Point(Location.X, Location.Y + 2 + movingSpeed); //down
                    break;
                    case "down": //Als collision aan de onderkant, beweeg naar boven                      
                    Location = new Point(Location.X, Location.Y - 2 - movingSpeed); //up
                    break;
                    case "left": //Als collision aan de linkerkant, beweeg naar rechts
                    Location = new Point(Location.X + 2 + movingSpeed, Location.Y); //right
                    break;
                    case "right": //Als collision aan de rechterkant, beweeg naar links
                    Location = new Point(Location.X - 2 - movingSpeed, Location.Y); //left
                    break;
                }
        }
    }
}
