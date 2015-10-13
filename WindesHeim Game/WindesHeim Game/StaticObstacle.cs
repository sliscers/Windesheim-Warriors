using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindesHeim_Game.Properties;

namespace WindesHeim_Game
{

    class StaticObstacle : Obstacle
    {

        public StaticObstacle(Point location, int height, int width) : base(location, height, width)
        {
            base.ObjectImage = Resources.IconTC;
            base.CollisionX = -6;
            base.CollisionY = 0;
            base.Name = "Verkeersregelaar";
            base.Description = "Doet niets";
            base.PanelIcon = Resources.IconTCEdited;
        }
    }
}
