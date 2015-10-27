using System.Drawing;
using WindesHeim_Game.Properties;

namespace WindesHeim_Game
{
    public class Player : GameObject
    {
        private const int originalSpeed = 4;
        private int speed = originalSpeed;
        private int speedDuration = 0;
        private int speedCooldown = 0;

        public Player(Point location, int height, int width) : base (location, height, width)
        {
            base.ObjectImage = Resources.Player;
        }

        public int Speed
        {
            get { return speed; }
            set { speed = value; }
        }
        public int OriginalSpeed
        {
            get { return originalSpeed; }

        }
        public int SpeedDuration
        {
            get { return speedDuration; }
            set { speedDuration = value; }
        }

        public int SpeedCooldown
        {
            get { return speedCooldown; }
            set { speedCooldown = value; }
        }



    }

}
