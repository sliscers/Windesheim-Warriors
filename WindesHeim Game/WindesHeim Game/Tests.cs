using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using System.Diagnostics;
using System;
using System.IO;
using WindesHeim_Game.Properties;
using System.Text;

namespace WindesHeim_Game {

    // UNIT TEST CASES

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
                        gameObstacle.ChasePlayer(player, "x");
                        gameObstacle.ChasePlayer(player, "y");

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
                        gameObstacle.ChasePlayer(player, "x");
                        gameObstacle.ChasePlayer(player, "y");

                        if (gameObstacle.CollidesWith(player)) {
                            Assert.IsTrue(gameObstacle.CollidesWith(player), "X + Y is incorrect");

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

        [TestMethod]
        public void TestXMLParser_Save() {
            GameProperties gameProperties = new GameProperties("Test Level", "easy");

            List<GameObject> gameObjects;
            Player player;
            BuildGameField(out gameObjects, out player);

            GameHighscore gameHighscore = new GameHighscore("Test Speler", DateTime.Now.ToString(), 900);

            XMLParser xmlParser = new XMLParser("unitTestXML.xml");

            if(xmlParser == null)
                Assert.Fail("NRE");

            xmlParser.gameHighscores.Add(gameHighscore);
            xmlParser.gameProperties = gameProperties;
            xmlParser.gameObjects = gameObjects;

            if (!xmlParser.WriteXML())
                Assert.Fail("Kan niet Write XML uitvoeren");
        }

        [TestMethod]
        public void TestXMLParser_Load() {
            GameProperties gameProperties = new GameProperties("Test Level", "easy");

            List<GameObject> gameObjects;
            Player player;
            BuildGameField(out gameObjects, out player);

            GameHighscore gameHighscore = new GameHighscore("Test Speler", DateTime.Now.ToString(), 900);

            XMLParser xmlParser = new XMLParser("unitTestXML.xml");

            if (xmlParser == null)
                Assert.Fail("NRE");

            xmlParser.gameHighscores.Add(gameHighscore);
            xmlParser.gameProperties = gameProperties;
            xmlParser.gameObjects = gameObjects;

            if (!xmlParser.WriteXML())
                Assert.Fail("Kan niet Write XML uitvoeren");

            // Reset XML
            xmlParser = null;
            xmlParser = new XMLParser("unitTestXML.xml");

            if(!xmlParser.ReadXML())
                Assert.Fail("Kan niet Load XML uitvoeren");

            Assert.AreEqual(gameProperties, xmlParser.gameProperties, "GameProperties not equal");

            // We hebben checkpoints gehardcode, dus nu even verwijderen
            xmlParser.gameObjects.RemoveAt(0);
            xmlParser.gameObjects.RemoveAt(1);
            Assert.AreEqual(gameObjects.Count, xmlParser.gameObjects.Count, "GameObjects not equal");
        }

        [TestMethod]
        public void TextXMLParser_LoadAllLevels() {
            if (!System.Diagnostics.Debugger.IsAttached) {
                if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "/levels/")) {
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "/levels/");
                }

                int fileCount = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "/levels/", "*.xml", SearchOption.AllDirectories).Length;

                if (fileCount == 0) {
                    string xmlString = Resources.level1;
                    File.WriteAllBytes(AppDomain.CurrentDomain.BaseDirectory + "/levels/level1.xml", Encoding.UTF8.GetBytes(Resources.level1));
                }
            }
            Assert.IsTrue(XMLParser.LoadAllLevels(), "Kan levels niet loaden");
        }
    }
}
