﻿using System;
using System.Collections.Generic;
using System.Drawing;

namespace WindesHeim_Game
{

    public class Obstacle : GameObject
    {
        private string name;
        private string description;
        private Image panelIcon;
        private int movingSpeed;
        private bool smartMovingEnabled = false;
        private string smartmovingDirection = "";
        private DateTime smartmovingTime;
        private DateTime directionTime;
        private bool isSmart = false;

        private List<string> historyMovement = new List<string>();

        public Obstacle(Point location, int height, int width) : base(location, height, width)
        {

        }

        public List<string> HistoryMovement
        {
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

        public DateTime DirectionTime
        {
            get { return directionTime; }
            set { directionTime = value; }
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

        public int MovingSpeed
        {
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

        public void ProcessCollision(List<GameObject> safeListArray, string axis)
        {           

            if (axis == "x")
            {
                //Doorloopt alle 
                foreach (GameObject potentialCollision in safeListArray)
                {
                    if (this != potentialCollision && !(potentialCollision is Checkpoint))
                    {
                        if (this.CollidesWith(potentialCollision))
                        {
                            if (this.CollisionRectangle.Left < potentialCollision.CollisionRectangle.Left)
                            {
                                if (!((this is MovingExplodingObstacle && potentialCollision is SlowingObstacle) || (this is SlowingObstacle && potentialCollision is MovingExplodingObstacle)))
                                {
                                    if (!smartMovingEnabled)
                                        this.Location = new Point(this.Location.X - 1 - this.movingSpeed, this.Location.Y); //left
                                    SetDirections("left");
                                }
                            }
                            else if (this.CollisionRectangle.Right > potentialCollision.CollisionRectangle.Right)
                            {
                                if (!((this is MovingExplodingObstacle && potentialCollision is SlowingObstacle) || (this is SlowingObstacle && potentialCollision is MovingExplodingObstacle)))
                                {
                                    if (!smartMovingEnabled)
                                        this.Location = new Point(this.Location.X + 1 + this.movingSpeed, this.Location.Y); //right
                                    SetDirections("right");
                                }
                            }
                        }
                    }
                }
            }
            else if (axis == "y")
            {
                foreach (GameObject potentialCollision in safeListArray)
                {
                    if (this != potentialCollision && !(potentialCollision is Checkpoint) || this is MovingExplodingObstacle && !(potentialCollision is SlowingObstacle))
                    {
                        if (this.CollidesWith(potentialCollision))
                        {
                            if (this.CollisionRectangle.Bottom > potentialCollision.CollisionRectangle.Bottom)
                            {
                                if (!((this is MovingExplodingObstacle && potentialCollision is SlowingObstacle) || (this is SlowingObstacle && potentialCollision is MovingExplodingObstacle)))
                                {
                                    if (!smartMovingEnabled)
                                        this.Location = new Point(this.Location.X, this.Location.Y + 1 + this.movingSpeed); //down
                                    SetDirections("down");
                                }
                            }
                            else if (this.CollisionRectangle.Top < potentialCollision.CollisionRectangle.Top)
                            {
                                if (!((this is MovingExplodingObstacle && potentialCollision is SlowingObstacle) || (this is SlowingObstacle && potentialCollision is MovingExplodingObstacle)))
                                {
                                    if (!smartMovingEnabled)
                                        this.Location = new Point(this.Location.X, this.Location.Y - 1 - this.movingSpeed); //up
                                    SetDirections("up");
                                }
                            }
                        }
                    }
                }
            }
            string[] directions = smartmovingDirection.Split(new string[] { "," }, StringSplitOptions.None);
            if (directionTime != default(DateTime) && isSmart && smartMovingEnabled)
            {
                if (directionTime <= DateTime.Now && directions.Length >= 2)
                {
                    smartmovingDirection = "";
                    foreach (string direction in directions)
                    {
                        if (!direction.Equals(directions[1]))
                            smartmovingDirection += "," + direction;
                    }
                    directionTime = default(DateTime);
                }
            }
        }

        private void SetDirections(string direction)
        {
            string[] directions = smartmovingDirection.Split(new string[] { "," }, StringSplitOptions.None);
            if (isSmart)
                if (!smartmovingDirection.Contains(direction))
                    if (directions.Length > 2)
                        smartmovingDirection = direction;
                    else
                    {
                        smartmovingDirection += "," + direction;
                        directionTime = DateTime.Now.AddMilliseconds(500);
                    }
        }

        public void ChasePlayer(Player player, string axis)
        {
            bool continueX = true;
            bool continueY = true;
            if (smartMovingEnabled)
            {
                if (smartmovingDirection.Contains("up") || smartmovingDirection.Contains("down"))
                {
                    continueY = false;
                }
                if (smartmovingDirection.Contains("left") || smartmovingDirection.Contains("right"))
                {
                    continueX = false;
                }
            }
            if (axis == "x" && continueX)
            {
                    if (Location.X >= player.Location.X)
                    {
                            Location = new Point(Location.X - 1 - movingSpeed, Location.Y);
                    }
                    if (Location.X <= player.Location.X)
                    {
                            Location = new Point(Location.X + 1 + movingSpeed, Location.Y);
                    }
            }
            else if (axis == "y" && continueY)
            {
                    if (Location.Y >= player.Location.Y)
                    {
                            Location = new Point(Location.X, Location.Y - 1 - movingSpeed);
                    }
                    if (Location.Y <= player.Location.Y)
                    {                    
                            Location = new Point(Location.X, Location.Y + 1 + movingSpeed);
                    }
            }
        }

        public void TryToEscape()
        {
            string[] directions = smartmovingDirection.Split(new string[] { "," }, StringSplitOptions.None);            
            foreach (string direction in directions)
            {
                switch (direction)
                {
                    case "down":
                        Location = new Point(Location.X, Location.Y + 1 + movingSpeed); //down
                        break;
                    case "up":
                        Location = new Point(Location.X, Location.Y - 1 - movingSpeed); //up
                        break;
                    case "right":
                        Location = new Point(Location.X + 1 + movingSpeed, Location.Y); //right
                        break;
                    case "left":
                        Location = new Point(Location.X - 1 - movingSpeed, Location.Y); //left
                        break;
                }
            }
        }
    }
}
