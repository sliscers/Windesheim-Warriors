using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindesHeim_Game
{
    class Checkpoint : GameObject
    {
        public Checkpoint (Point location, string imageURL, int height, int width) : base(location, imageURL, width, height)
        {
        
        }
    }
}
