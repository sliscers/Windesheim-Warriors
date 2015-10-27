using System.Drawing;
using WindesHeim_Game.Properties;

namespace WindesHeim_Game {

    class MovingExplodingObstacle : Obstacle {

        public MovingExplodingObstacle(Point location, int height, int width) : base (location, height, width)
        {
            base.ObjectImage = Resources.IconBike;
            base.collisionSize = 50;
            base.CollisionX = 0;
            base.CollisionY = -2;
            base.Name = "Cyclist";
            base.Description = "Follows character and detonates on collision";
            base.PanelIcon = Resources.bikeEdited;
        }
    }
}
