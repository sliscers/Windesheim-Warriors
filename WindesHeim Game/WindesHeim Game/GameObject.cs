using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindesHeim_Game
{
    public class GameObject
    {
        private Point location;
        private Bitmap objectImage;
        private int height;
        private int width;

        private int collisionX = 0;
        private int collisionY = 0;
       
        public int collisionSize;

        public GameObject(Point location, int height, int width)
        {
            this.location = location;
            this.height = height;
            this.width = width;
        }

        public Point Location
        {
            get { return location; }
            set { location = value; }
        }

        public Bitmap ObjectImage {
            get { return objectImage; }
            set { objectImage = value; }
        }

        public int Height
        {
            get { return height; }
            set { height = value;  }
        }

        public int Width
        {
            get { return width; }
            set { width = value; }
        }
        public int CollisionX
        {
            get { return collisionX; }
            set { collisionX = value; }
        }

        public int CollisionY
        {
            get { return collisionY; }
            set { collisionY = value; }
        }

        public double GetDistance(Point q) {
            double a = Location.X  - q.X;
            double b = Location.Y - q.Y;
            double distance = Math.Sqrt(a * a + b * b);
            return distance;
        }

        public bool CollidesWith(GameObject gameObject)
        {
            if((this.location.X >= (gameObject.location.X - gameObject.CollisionX)) && (this.location.X <= (gameObject.location.X + gameObject.Width + gameObject.CollisionX))
                && (this.location.Y >= (gameObject.location.Y - gameObject.CollisionY)) && (this.location.Y <= (gameObject.location.Y + gameObject.Height + gameObject.CollisionY))
                || ((this.location.X + this.Width) >= (gameObject.location.X - gameObject.CollisionX)) && ((this.location.X + this.Width) <= (gameObject.location.X + gameObject.Width + gameObject.CollisionX))
                && (this.location.Y >= (gameObject.location.Y - gameObject.CollisionY)) && (this.location.Y <= (gameObject.location.Y + gameObject.Height + gameObject.CollisionY))
                || (this.location.X >= (gameObject.location.X - gameObject.CollisionX)) && (this.location.X <= (gameObject.location.X + gameObject.Width + gameObject.CollisionX))
                && ((this.location.Y + this.Height) >= (gameObject.location.Y - gameObject.CollisionY)) && ((this.location.Y + this.Height) <= (gameObject.location.Y + gameObject.Height + gameObject.CollisionY))
                || ((this.location.X + this.Width) >= (gameObject.location.X - gameObject.CollisionX)) && ((this.location.X + this.Width) <= (gameObject.location.X + gameObject.Width + gameObject.CollisionX))
                && ((this.location.Y + this.Height) >= (gameObject.location.Y - gameObject.CollisionY)) && ((this.location.Y + this.Height) <= (gameObject.location.Y + gameObject.Height + gameObject.CollisionY))
                )
            {
                return true;
            }
            return false;
        }

        public void FadeSmall()
        {
            this.Height+=2;
            this.Width+=2;
        }
    }
}
