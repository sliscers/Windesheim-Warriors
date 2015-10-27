using System.Drawing;
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
            base.Name = "Traffic Controller";
            base.Description = "Blocks the character";
            base.PanelIcon = Resources.IconTCEdited;
        }
    }
}
