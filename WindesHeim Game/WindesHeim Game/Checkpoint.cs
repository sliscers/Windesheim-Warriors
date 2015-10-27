using System.Drawing;

namespace WindesHeim_Game
{
    class Checkpoint : GameObject
    {
        private bool start;

        public Checkpoint (Point location, Bitmap image, int height, int width, bool start) : base(location, height, width)
        {
            base.collisionSize = 40;
            base.ObjectImage = image;
            this.start = start;
        }

        public bool Start {
            get { return start; }
            set { start = value; }
        }
    }
}
