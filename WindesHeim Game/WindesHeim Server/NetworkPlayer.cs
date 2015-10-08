using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindesHeim_Server {
    class NetworkPlayer {
        public bool isHost = false;
        public NetConnection netConnection;
        public int playerId;
    }
}
