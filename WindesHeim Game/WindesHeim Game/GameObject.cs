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
        private Bitmap image;
        private int height;
        private int width;

        protected int collisionSize;

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
            get { return image; }
            set { image = value; }
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

        protected double GetDistance(Point q) {
            double a = Location.X - q.X;
            double b = Location.Y - q.Y;
            double distance = Math.Sqrt(a * a + b * b);
            return distance;
        }

        public bool CollidesWith(GameObject gameObject) {
            if (GetDistance(gameObject.Location) < collisionSize) {
                return true;
            }
            else {
                return false;
            }
        }

        public void FadeSmall()
        {
            this.Height+=2;
            this.Width+=2;
        }
    }
}
