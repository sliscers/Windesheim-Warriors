using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WindesHeim_Game.Properties;

namespace WindesHeim_Game {

    public class NetworkPlayer {

        public ModelGame gameModel;
        public NetClient client;
        public bool host;
        public int playerId;

        public NetworkPlayer(ModelGame gameModel) {
            this.gameModel = gameModel;
        }

        public void Connect() {
            NetPeerConfiguration config = new NetPeerConfiguration("wh");
            config.AutoFlushSendQueue = false;

            client = new NetClient(config);
            client.RegisterReceivedCallback(new SendOrPostCallback(ReceivedMessageFromServer));
            client.Start();

            NetOutgoingMessage hail = client.CreateMessage("This is the hail message");
            client.Connect("151.80.45.175", 80, hail);
        }

        public void ReceivedMessageFromServer(object peer) {
            NetIncomingMessage im;
            while ((im = client.ReadMessage()) != null) {
                // handle incoming message
                switch (im.MessageType) {
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.ErrorMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.VerboseDebugMessage:
                        string text = im.ReadString();
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();

                        if (status == NetConnectionStatus.Connected)
                            Console.WriteLine("Connected");
                        else
                            Console.WriteLine("Disconnected");

                        string reason = im.ReadString();
                        Console.WriteLine(status.ToString() + ": " + reason);

                        break;
                    case NetIncomingMessageType.Data:
                        string message = im.ReadString();

                        // This message is intended for 1 client
                        if(message == "please_give_me_your_list_of_gameobjects") {
                            StringBuilder stringBuilder = new StringBuilder();
                            stringBuilder.Append("list_of_gameobjects:");

                            int newEntityId = 1;

                            foreach(GameObject gameObject in gameModel.GameObjects) {
                                gameObject.entityId = newEntityId;
                                stringBuilder.Append(gameObject.entityId + "," + gameObject.Location.X + "," + gameObject.Location.Y + "," + gameObject.GetType().Name + ":");
                                newEntityId++;
                            }

                            stringBuilder.Length--;

                            Console.WriteLine("Sending GameObjects list to Server: " + stringBuilder);

                            NetOutgoingMessage om = client.CreateMessage(stringBuilder.ToString());
                            client.SendMessage(om, NetDeliveryMethod.ReliableOrdered);
                            client.FlushSendQueue();
                        }

                        // This message is intended for 1 client
                        if (message.Contains("here_is_the_updated_list_of_gameobjects")) {
                            gameModel.GameObjects.Clear();                         

                            string[] messageData = message.Split(':');

                            foreach (string data in messageData) {
                                // Just ignore the first because of the split
                                if (data != "here_is_the_updated_list_of_gameobjects") {
                                    string[] gameObjectData = data.Split(',');

                                    int entityId = Convert.ToInt32(gameObjectData[0]);
                                    int posX = Convert.ToInt32(gameObjectData[1]);
                                    int posY = Convert.ToInt32(gameObjectData[2]);
                                    string gameObjectType = gameObjectData[3];

                                    if (gameObjectType == "ExplodingObstacle") {
                                        ExplodingObstacle explodingObstacle = new ExplodingObstacle(new Point(posX, posY), 40, 40);
                                        explodingObstacle.entityId = entityId;
                                        gameModel.GameObjects.Add(explodingObstacle);
                                    }
                                    else if (gameObjectType == "StaticObstacle") {
                                        StaticObstacle staticObstacle = new StaticObstacle(new Point(posX, posY), 40, 40);
                                        staticObstacle.entityId = entityId;
                                        gameModel.GameObjects.Add(staticObstacle);
                                    }
                                    else if (gameObjectType == "MovingExplodingObstacle") {
                                        MovingExplodingObstacle movingExplodingObstacle = new MovingExplodingObstacle(new Point(posX, posY), 40, 40);
                                        movingExplodingObstacle.entityId = entityId;
                                        gameModel.GameObjects.Add(movingExplodingObstacle);
                                    }
                                    else if (gameObjectType == "SlowingObstacle") {
                                        SlowingObstacle slowingObstacle = new SlowingObstacle(new Point(posX, posY), 40, 40);
                                        slowingObstacle.entityId = entityId;
                                        gameModel.GameObjects.Add(slowingObstacle);
                                    }
                                    else if (gameObjectType == "Checkpoint") {
                                        Checkpoint checkPoint = new Checkpoint(new Point(posX, posY), Resources.IconSP, 80, 80, true);
                                        checkPoint.entityId = entityId;
                                        gameModel.GameObjects.Add(checkPoint);
                                    }
                                    else if (gameObjectType == "Player") {
                                        Player player = new Player(new Point(posX, posY), 40, 40);
                                        player.entityId = entityId;
                                        gameModel.GameObjects.Add(player);
                                    }
                                }
                            }

                            //gameModel.GameObjects.Add(new Checkpoint(new Point(750, 400), Resources.IconWIN, 80, 80, false));
                            //gameModel.GameObjects.Add(new Checkpoint(new Point(5, -5), Resources.IconSP, 80, 80, true));
                        }

                        // This message is intended for 1 client
                        if (message == "you_are_the_host") {
                            Console.WriteLine(message);

                            host = true;
                            Console.WriteLine("I am the host");
                        }

                        // This message is intended for 1 client
                        if(message.Contains("playerid")) {
                            string[] arguments = message.Split(':');
                            this.playerId = Convert.ToInt32(arguments[1]);

                            Console.WriteLine("My assigned playerId: " + this.playerId);
                        }

                        // This message is intended for ALL clients
                        if (message.Contains("create_player_object_for")) {
                            string[] arguments = message.Split(':');
                            string[] data = arguments[1].Split(',');

                            Player player = new Player(new Point(10, 10), 44, 44);
                            player.entityId = Convert.ToInt32(data[1]);

                            // We asked for this player objected to be created, so give us control
                            if (playerId == Convert.ToInt32(data[0])) {
                                player.localPlayer = true;
                                Console.WriteLine("We are local player: " + player.entityId);
                            }

                            gameModel.GameObjects.Add(player);
                        }

                        // This message is intended for ALL clients
                        if (message.Contains("this_player_updated_position")) {
                            string[] messageData = message.Split(':');
                            string[] arguments = messageData[1].Split(',');

                            int entityId = Convert.ToInt32(arguments[0]); 
                            int posX = Convert.ToInt32(arguments[1]);
                            int posY = Convert.ToInt32(arguments[2]);

                            foreach(GameObject gameObject in gameModel.GameObjects) {
                                if(gameObject.entityId == entityId) {
                                    gameObject.Location = new Point(posX, posY);
                                }
                            }
                        }

                        break;
                    default:
                        Console.WriteLine("Unhandled type: " + im.MessageType + " " + im.LengthBytes + " bytes");
                        break;
                }
                client.Recycle(im);
            }
        }

        public void SendPositionToServer(Player localPlayer) {
            Console.WriteLine("player_position:" + localPlayer.entityId + "," + localPlayer.Location.X + "," + localPlayer.Location.Y);

            NetOutgoingMessage om = client.CreateMessage("player_position:" + localPlayer.entityId + "," + localPlayer.Location.X + "," + localPlayer.Location.Y);
            client.SendMessage(om, NetDeliveryMethod.Unreliable);
            client.FlushSendQueue();
        }

        public void Disconnect() {
            client.Disconnect("bye");
        }
    }
}
