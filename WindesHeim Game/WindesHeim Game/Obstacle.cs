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

        public string ProcessCollision(GameObject gameObject) {

            string hitpoint = "";

            if (this.Location.Y == gameObject.Location.Y + Height
                && (this.Location.X <= gameObject.Location.X && this.Location.X + this.Width >= gameObject.Location.X
                || this.Location.X >= gameObject.Location.X && this.Location.X <= gameObject.Location.X + gameObject.Width)) {
                if(!SmartMovingEnabled)
                    Location = new Point(Location.X, Location.Y + 1);
                //ProcessCollision(gameObject);
                hitpoint = "up";
            }

            if (this.Location.Y + this.Height == gameObject.Location.Y
                && (this.Location.X + this.Width >= gameObject.Location.X && this.Location.X <= gameObject.Location.X
                || this.Location.X >= gameObject.Location.X && this.Location.X <= gameObject.Location.X + gameObject.Width)) {
                if (!SmartMovingEnabled)
                    Location = new Point(Location.X, Location.Y - 1);
                //ProcessCollision(gameObject);
                hitpoint = "down";
            }

            if (this.Location.X == gameObject.Location.X + gameObject.Width
                && (this.Location.Y >= gameObject.Location.Y && this.Location.Y <= gameObject.Location.Y + gameObject.Height
                || this.Location.Y + this.Height >= gameObject.Location.Y && this.Location.Y <= gameObject.Location.Y)) {
                if (!SmartMovingEnabled)
                    Location = new Point(Location.X + 1, Location.Y);
                //ProcessCollision(gameObject);
                hitpoint = "left";
            }

            if (this.Location.X + this.Width == gameObject.Location.X
                && (this.Location.Y >= gameObject.Location.Y && this.Location.Y <= gameObject.Location.Y + gameObject.Height
                || this.Location.Y + this.Height >= gameObject.Location.Y && this.Location.Y <= gameObject.Location.Y)) {
                if (!SmartMovingEnabled)
                    Location = new Point(Location.X - 1, Location.Y);
                //ProcessCollision(gameObject);
                hitpoint = "right";
            }
            return hitpoint;
        }

        public void TryToEscape()
        {
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
