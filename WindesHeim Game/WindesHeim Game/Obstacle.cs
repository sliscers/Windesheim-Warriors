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

        private List<string> historyMovement = new List<string>();

        public Obstacle(Point location, int height, int width) : base (location, height, width)
        {
            
        }

        public List<string> HistoryMovement {
            get { return historyMovement; }
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

        public string ProcessCollision(List<GameObject> safeListArray, string axis) {
            string hitpoint = "";

            if (axis == "x") {
                foreach (GameObject potentialCollision in safeListArray) {
                    if (this != potentialCollision) {
                        if (this.CollidesWith(potentialCollision)) {
                            if (this.CollisionRectangle.Left < potentialCollision.CollisionRectangle.Left) {
                                this.Location = new Point(this.Location.X - 1 - this.movingSpeed, this.Location.Y);
                                hitpoint = "left";
                            }
                            else if (this.CollisionRectangle.Right > potentialCollision.CollisionRectangle.Right) {
                                this.Location = new Point(this.Location.X + 1 + this.movingSpeed, this.Location.Y);
                                hitpoint = "right";
                            }
                        }
                    }
                }
            }
            else if (axis == "y") {
                foreach (GameObject potentialCollision in safeListArray) {
                    if (this != potentialCollision) {
                        if (this.CollidesWith(potentialCollision)) {
                            if (this.CollisionRectangle.Bottom > potentialCollision.CollisionRectangle.Bottom) {
                                this.Location = new Point(this.Location.X, this.Location.Y + 1 + this.movingSpeed);
                                hitpoint = "up";
                            }
                            else if (this.CollisionRectangle.Top < potentialCollision.CollisionRectangle.Top) {
                                this.Location = new Point(this.Location.X, this.Location.Y - 1 - this.movingSpeed);
                                hitpoint = "down";
                            }
                        }
                    }
                }
            }

            return hitpoint;
        }

        public void ChasePlayer(Player player, string axis) {
            if(axis == "x") {
                if (Location.X >= player.Location.X) {
                    Location = new Point(Location.X - 1 - movingSpeed, Location.Y);
                }
                if (Location.X <= player.Location.X) {
                    Location = new Point(Location.X + 1 + movingSpeed, Location.Y);
                }
            }
            else if(axis == "y") {
                if (Location.Y >= player.Location.Y) {
                    Location = new Point(Location.X, Location.Y - 1 - movingSpeed);
                }
                if (Location.Y <= player.Location.Y) {
                    Location = new Point(Location.X, Location.Y + 1 + movingSpeed);
                }
            }
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
