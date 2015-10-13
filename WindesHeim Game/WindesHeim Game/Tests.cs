using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using System.Timers;
using System.Diagnostics;

namespace WindesHeim_Game {

    // UNIT TEST CASES
    // Check of MovingExplodingObstacle en SlowingObstacle ook daadwerkelijk de player aanraken
    // Check wanneer de player een ExplodingObstacle aanraakt, het ook wordt registreerd

    [TestClass]
    public class Tests {

        private void BuildGameField(out List<GameObject> gameObjects, out Player player) {
            // Maak een test begin veld
            Debug.WriteLine("BuildGameField: Start");
            gameObjects = new List<GameObject>();
            gameObjects.Add(new MovingExplodingObstacle(new Point(100, 100), 40, 40));
            gameObjects.Add(new MovingExplodingObstacle(new Point(200, 100), 40, 40));
            gameObjects.Add(new MovingExplodingObstacle(new Point(300, 100), 40, 40));
            gameObjects.Add(new SlowingObstacle(new Point(300, 100), 40, 40));
            gameObjects.Add(new SlowingObstacle(new Point(300, 100), 40, 40));
            gameObjects.Add(new ExplodingObstacle(new Point(50, 0), 40, 40));

            player = new Player(new Point(0, 5), 40, 40);
            Debug.WriteLine("BuildGameField: End");
        }

        [TestMethod]
        public void ChasePlayerAsMovingExplodingObstacle() {
            Debug.WriteLine("ChasePlayerAsMovingExplodingObstacle: Start");

            List<GameObject> gameObjects;
            Player player;
            BuildGameField(out gameObjects, out player);

            bool gameLoop = true;

            // Simulatie van de gameloop
            while(gameLoop) {
                // Exact zelfde code als productie
                List<GameObject> safeListArray = new List<GameObject>(gameObjects);
                foreach (GameObject gameObject in safeListArray) {
                    if (gameObject is MovingExplodingObstacle) {
                        MovingExplodingObstacle gameObstacle = (MovingExplodingObstacle)gameObject;
                        gameObstacle.ChasePlayer(player);

                        if (gameObstacle.CollidesWith(player)) {
                            int widthOfPlayerObject = player.Location.X + player.Width;
                            int heightOfPlayerObject = player.Location.Y + player.Height;

                            // We hebben het daadwerkelijk aangeraakt, dus if statement is valide
                            bool checkIfXIsSuccess = gameObstacle.Location.X <= widthOfPlayerObject && gameObstacle.Location.Y <= heightOfPlayerObject;
                            Assert.IsTrue(checkIfXIsSuccess, "X + Y is incorrect");

                            gameLoop = false;
                        }
                    }
                }
            }
            Debug.WriteLine("ChasePlayerAsMovingExplodingObstacle: End");
        }

        [TestMethod]
        public void ChasePlayerAsSlowingObstacle() {
            Debug.WriteLine("ChasePlayerAsSlowingObstacle: Start");

            List<GameObject> gameObjects;
            Player player;
            BuildGameField(out gameObjects, out player);

            bool gameLoop = true;

            // Simulatie van de gameloop
            while (gameLoop) {
                // Exact zelfde code als productie
                List<GameObject> safeListArray = new List<GameObject>(gameObjects);
                foreach (GameObject gameObject in safeListArray) {
                    if (gameObject is SlowingObstacle) {
                        SlowingObstacle gameObstacle = (SlowingObstacle)gameObject;
                        gameObstacle.ChasePlayer(player);

                        if (gameObstacle.CollidesWith(player)) {

                            int widthOfPlayerObject = player.Location.X + player.Width;
                            int heightOfPlayerObject = player.Location.Y + player.Height;

                            // We hebben het daadwerkelijk aangeraakt, dus if statement is valide
                            bool checkIfXIsSuccess = gameObstacle.Location.X <= widthOfPlayerObject && gameObstacle.Location.Y <= heightOfPlayerObject;
                            Assert.IsTrue(checkIfXIsSuccess, "X + Y is incorrect");

                            gameLoop = false;
                        }
                    }
                }
            }
            Debug.WriteLine("ChasePlayerAsSlowingObstacle: End");
        }

        [TestMethod]
        public void LetPlayerDieByExplodingObstacle() {
            Debug.WriteLine("LetPlayerDieByExplodingObstacle: Start");

            List<GameObject> gameObjects;
            Player player;
            BuildGameField(out gameObjects, out player);

            bool gameLoop = true;

            // Simulatie van de gameloop
            while (gameLoop) {
                player.Location = new Point(player.Location.X + 1, player.Location.Y);

                List<GameObject> safeListArray = new List<GameObject>(gameObjects);
                foreach (GameObject gameObject in safeListArray) {
                    if (gameObject is ExplodingObstacle) {
                        Debug.WriteLine("Found Player X,Y at: " + (player.Location.X + player.Width) + ", " + (player.Location.Y + player.Height) + ", found Exploding Obstacle X,Y at: " + ((ExplodingObstacle)gameObject).Location.X + "," + ((ExplodingObstacle)gameObject).Location.Y);

                        ExplodingObstacle gameObstacle = (ExplodingObstacle)gameObject;
                        if (player.CollidesWith(gameObstacle)) {
                            Debug.WriteLine("COLLIDED Player X,Y at: " + (player.Location.X + player.Width) + ", " + (player.Location.Y + player.Height) + ", found Exploding Obstacle X,Y at: " + ((ExplodingObstacle)gameObject).Location.X + "," + ((ExplodingObstacle)gameObject).Location.Y);
                            Assert.IsTrue(player.CollidesWith(gameObstacle));
                            gameLoop = false;
                        }
                    }
                }

                if(player.Location.X >= 100) {
                    Assert.Fail("Player did not detect exploding obstacle");
                }
            }

            Debug.WriteLine("LetPlayerDieByExplodingObstacle: End");
        }
    }
}
