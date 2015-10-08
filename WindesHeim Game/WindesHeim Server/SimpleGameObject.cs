using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindesHeim_Server {
    class SimpleGameObject {
        public int id;
        public int x;
        public int y;
        public string gameObjectType;

        public SimpleGameObject(int id, int x, int y, string gameObjectType) {
            this.id = id;
            this.x = x;
            this.y = y;
            this.gameObjectType = gameObjectType;
        }
    }
}
