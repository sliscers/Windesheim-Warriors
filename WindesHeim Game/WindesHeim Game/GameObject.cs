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
        private string imageURL;
        private int height;
        private int width;

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

        public string ImageURL {
            get { return imageURL; }
            set { imageURL = value; }
        }
        public int Height
        {
            get { return height; }

        }

        public int Width
        {
            get { return width; }
        }
    }
}
