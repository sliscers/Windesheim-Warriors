using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WindesHeim_Server {
    class Program {

        private NetServer server;
        private List<NetworkPlayer> networkPlayers;
        private List<SimpleGameObject> gameObjects;

        public Program() {
            networkPlayers = new List<NetworkPlayer>();
            gameObjects = new List<SimpleGameObject>();

            NetPeerConfiguration config = new NetPeerConfiguration("wh");
            config.MaximumConnections = 100;
            config.Port = 80;

            server = new NetServer(config);
            server.Start();

            Console.WriteLine("Server started; waiting 3 seconds...");
            Thread.Sleep(3000);

            while (Console.KeyAvailable == false || Console.ReadKey().Key != ConsoleKey.Escape) {
                NetIncomingMessage inc;
                while ((inc = server.ReadMessage()) != null) {
                    switch (inc.MessageType) {
                        case NetIncomingMessageType.VerboseDebugMessage:
                        case NetIncomingMessageType.DebugMessage:
                        case NetIncomingMessageType.WarningMessage:
                        case NetIncomingMessageType.ErrorMessage:
                            Console.WriteLine(inc.ReadString());
                            break;

                        case NetIncomingMessageType.StatusChanged:
                            NetConnectionStatus status = (NetConnectionStatus)inc.ReadByte();

                            string reason = inc.ReadString();
                            Console.WriteLine(NetUtility.ToHexString(inc.SenderConnection.RemoteUniqueIdentifier) + " " + status + ": " + reason);

                            if (status == NetConnectionStatus.Connected) {
                                // A new player has connected! Create an object for the server to keep track of said player
                                NetworkPlayer networkPlayer = new NetworkPlayer();
                                networkPlayer.netConnection = inc.SenderConnection;

                                // Let's send him some initial data we deem necessary
                                Random random = new Random();
                                int playerId = random.Next(10000);
                                networkPlayer.playerId = playerId;
                                AssignPlayerIdForPlayer(networkPlayer, playerId);

                                // Is this the first player? If so, make him the authority
                                if (networkPlayers.Count == 0) {
                                    networkPlayer.isHost = true;
                                    CreatePlayerObject(networkPlayer.playerId);
                                    SetPlayerAsAuthority(networkPlayer);
                                    RequestGameObjectData(networkPlayer);
                                }
                                // There are players active on the field, give the connected player all current gameObjects data
                                else {
                                    GiveCurrentGameObjectDataToPlayer(networkPlayer, gameObjects);
                                    CreatePlayerObject(networkPlayer.playerId);
                                }

                                networkPlayers.Add(networkPlayer);

                                Console.WriteLine("Added player to array: " + networkPlayer.isHost);
                            }

                            if (status == NetConnectionStatus.Disconnected) {
                                List<NetworkPlayer> safeNetworkPlayers = new List<NetworkPlayer>(networkPlayers);

                                foreach(NetworkPlayer networkPlayer in safeNetworkPlayers) {
                                    if(networkPlayer.netConnection == inc.SenderConnection) {
                                        Console.WriteLine("Found player, removing");
                                        networkPlayers.Remove(networkPlayer);
                                    }
                                }
                            }
                            break;

                        case NetIncomingMessageType.Data:
                            string message = inc.ReadString();

                            NetworkPlayer networkPlayerInQuestion = Find(inc.SenderConnection);
                            if (networkPlayerInQuestion != null) {
                                if (networkPlayerInQuestion.isHost) {

                                    if (message.Contains("list_of_gameobjects")) {
                                        string[] messageData = message.Split(':');

                                        foreach (string data in messageData) {
                                            // Just ignore the first because of the split
                                            if (data != "list_of_gameobjects") {
                                                string[] gameObjectData = data.Split(',');

                                                gameObjects.Add(new SimpleGameObject(Convert.ToInt32(gameObjectData[0]), Convert.ToInt32(gameObjectData[1]), Convert.ToInt32(gameObjectData[2]), gameObjectData[3]));
                                                Console.WriteLine(data);
                                            }
                                        }
                                    }
                                }

                                if (message.Contains("player_position")) {
                                    SendPlayerPositionDataToClients(message);
                                }
                            }
                            break;
                    }
                }
            }
        }

        private void SendPlayerPositionDataToClients(string message) {
            string[] messageData = message.Split(':');
            string[] playerData = messageData[1].Split(',');

            Console.WriteLine("Receiving player data: " + messageData[1]);

            List<NetConnection> allConnections = server.Connections;

            if (allConnections.Count > 0) {
                NetOutgoingMessage outgoingMessage = server.CreateMessage();
                outgoingMessage.Write("this_player_updated_position:" + messageData[1]);
                server.SendMessage(outgoingMessage, allConnections, NetDeliveryMethod.ReliableOrdered, 0);
            }
        }

        private void GiveCurrentGameObjectDataToPlayer(NetworkPlayer networkPlayer, List<SimpleGameObject> gameObjects) {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("here_is_the_updated_list_of_gameobjects:");
            foreach (SimpleGameObject gameObject in gameObjects) {
                stringBuilder.Append(gameObject.id + "," + gameObject.x + "," + gameObject.y + "," + gameObject.gameObjectType + ":");
            }
            stringBuilder.Length--;

            NetOutgoingMessage outgoingMessage = server.CreateMessage();
            outgoingMessage.Write(stringBuilder.ToString());
            server.SendMessage(outgoingMessage, networkPlayer.netConnection, NetDeliveryMethod.ReliableOrdered, 0);
        }

        public NetworkPlayer Find(NetConnection netConnection) {
            foreach (NetworkPlayer networkPlayer in networkPlayers) {
                if (networkPlayer.netConnection == netConnection) {
                    return networkPlayer;
                }
            }
            return null;
        }

        private void SetPlayerAsAuthority(NetworkPlayer networkPlayer) {
            NetOutgoingMessage outgoingMessage = server.CreateMessage();
            outgoingMessage.Write("you_are_the_host");
            server.SendMessage(outgoingMessage, networkPlayer.netConnection, NetDeliveryMethod.ReliableOrdered, 0);
        }

        private void RequestGameObjectData(NetworkPlayer networkPlayer) {
            NetOutgoingMessage outgoingMessage = server.CreateMessage();
            outgoingMessage.Write("please_give_me_your_list_of_gameobjects");
            server.SendMessage(outgoingMessage, networkPlayer.netConnection, NetDeliveryMethod.ReliableOrdered, 0);
        }

        private void AssignPlayerIdForPlayer(NetworkPlayer networkPlayer, int playerId) {
            NetOutgoingMessage outgoingMessage = server.CreateMessage();
            outgoingMessage.Write("playerid:" + playerId);
            server.SendMessage(outgoingMessage, networkPlayer.netConnection, NetDeliveryMethod.ReliableOrdered, 0);

            Console.WriteLine("AssignPlayerIdForPlayer:: Sending data for " + NetUtility.ToHexString(networkPlayer.netConnection.RemoteUniqueIdentifier) + ", giving him this playerId: " + playerId);
        }

        private void CreatePlayerObject(int forPlayerId) {
            List<NetConnection> allConnections = server.Connections;

            Random random = new Random();
            int newEntityId = random.Next(100, 1000); // use entityId++ for this next time

            if (allConnections.Count > 0) {
                NetOutgoingMessage outgoingMessage = server.CreateMessage();
                outgoingMessage.Write("create_player_object_for:" + forPlayerId + "," + newEntityId);
                server.SendMessage(outgoingMessage, allConnections, NetDeliveryMethod.ReliableOrdered, 0);
                Console.WriteLine("create_player_object_for:" + forPlayerId + "," + newEntityId);
            }
        }

        static void Main(string[] args) {
            new Program();
        }
    }
}
