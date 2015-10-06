using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace WindesHeim_Game
{
    //Hoe te gebruiken:
    //Voorbeeld initieren: 
    //XMLParser level1 = new XMLParser("../levels/level1.xml"); //Geef hier het path naar het xml bestand mee
    //Voorbeeld voor titel: 
    //Console.Write(level1.gameProperties.title);
    //Voorbeeld om highscores te doorlopen
    //level1.ReadXML();
    //foreach(GameHighscores highscore in level1.gameHighscores)
    //{
    //    Console.Write(highscore.name + " " + highscore.score);
    //}

    public struct GameProperties
    {
        // Hier worden de Properties in opgeslagen
        public string title;
        public string difficulty;

        public GameProperties(string title, string difficulty)
        {
            this.title = title;
            this.difficulty = difficulty;
        }
    }

    public struct GameHighscores
    {
        //Hier worden de Highscores in opgeslagen en vervolgens gebruikt in een List
        public string name;
        public DateTime dateTime;
        public int score;

        public GameHighscores(string name, string dateTime, int score)
        {
            this.name = name;
            this.dateTime = Convert.ToDateTime(dateTime); //Converteerd Datetime.now (string) weer terug naar Datetime (DateTime)
            this.score = score;
        }
    }

    public class XMLParser
    {
        //Vastleggen van te gebruiken variablen.
        private String path;
        public GameProperties gameProperties;
        public List<GameObject> gameObjects;
        public List<GameHighscores> gameHighscores;

        public XMLParser(String path)
        {
            this.path = path;
        }

        //Funtie om XML bestand in te laden, daarna kan je de vastgelegde variablen in deze klasse gebruiken
        public void ReadXML()
        {
            //Laad het XML bestand in een document object
            XDocument doc = XDocument.Load(this.path);

            //Initieert de variablen
            gameHighscores = new List<GameHighscores>();
            gameObjects = new List<GameObject>();

            //Voert query uit op het XML document om de properties te laden in een var
            var lproperties = from r in doc.Descendants("properties")
                             select new
                             {
                                 Title = r.Element("title").Value,
                                 Difficulty = r.Element("difficulty").Value
                             };
            //Ditzelfde voor highscores
            var highscores = from r in doc.Descendants("highscore")
                            select new
                            {
                                Name = r.Element("name").Value,
                                DateTime = r.Element("datetime").Value,
                                //Score wordt direct geconverteert naar een int omdat deze ook zo wordt weggeschreven
                                Score = Int32.Parse(r.Element("score").Value),
                            };
            //en voor object
            var items = from r in doc.Descendants("object")
                        select new
                        {
                            Type = r.Element("type").Value,
                            //Directe conversie naar int omdat deze ook zo wordt weggeschreven
                            X = Int32.Parse(r.Element("x").Value),
                            Y = Int32.Parse(r.Element("y").Value)//,
                            //Hieronder volgen dynamische gegevens en is vaak null, hier moet ik nog verder naar kijken ~jonathan
                            //Movingspeed = Int32.Parse(r.Element("movingspeed").Value),
                            //Slowdown = Int32.Parse(r.Element("slowdown").Value)
                        };
            //Voegt de gameproperties toe aan de variable gameProperties
            foreach (var property in lproperties)
            {
                gameProperties = new GameProperties(property.Title, property.Difficulty);
            }

            //Voegt alle highscores toe in een List
            foreach (var highscore in highscores)
            {
                gameHighscores.Add(new GameHighscores(highscore.Name, highscore.DateTime, highscore.Score));
            }

            //Voegt alle gameObjecten toe in een List
            foreach (var gameObject in items)
            {
                    switch (gameObject.Type)
                    {
                        case "Player":
                            //gameObjects.Add(new Player(new Point(gameObject.X, gameObject.Y)));
                        break;

                        case "Car":
                            //gameObjects.Add(new ExplodingObstacle(new Point(20, 20), "../Player.png"));
                        break;

                        case "Cyclist":
                            //gameObjects.Add(new FollowingObstacle(new Point(20, 20), "../Player.png"));
                        break;

                        case "TrafficController":
                            //gameObjects.Add(new StaticObstacle(new Point(20, 20), "../Player.png"));
                        break;

                        case "Freshmen":
                            //SlowingObstacle sb = new SlowingObstacle(new Point(20, 20))
                            //sb.MovingSpeed = 10;                       

                            //gameObjects.Add(sb);
                        break;
                }
                
            }

        }

        //Deze functie schrijft een XML file weg
        //Geef hier de gameproperties mee in het objecdt GameProperties, vervolgens een List met GameObjects daarna eenzelfde lijst voor Highscores
        public void WriteXML(GameProperties gameProperties, List<GameObject> gameObjects, List<GameHighscores> gameHighscores)
        {
            //Instellingen voor XML voor een juiste opmaak
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;
            settings.NewLineOnAttributes = false;
            settings.IndentChars = "     ";

            //XMLwriter aanmaken
            XmlWriter xmlWriter = XmlWriter.Create(this.path, settings);

            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("level");

            xmlWriter.WriteStartElement("properties");

            xmlWriter.WriteStartElement("title");
            xmlWriter.WriteValue(gameProperties.title); //Title
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("difficulty");
            xmlWriter.WriteValue(gameProperties.difficulty); //Difficulty
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("items");

            foreach (GameObject gameObject in gameObjects)
            {
                xmlWriter.WriteStartElement("object");

                string gameObjectType = "";
                if (gameObject is Player)
                {
                    gameObjectType = "Player";
                }
                else if (gameObject is ExplodingObstacle)
                {
                    gameObjectType = "Car";
                }
                else if (gameObject is MovingExplodingObstacle)
                {
                    gameObjectType = "Cyclist";
                }
                else if (gameObject is StaticObstacle)
                {
                    gameObjectType = "TrafficController";
                }
                else if (gameObject is SlowingObstacle)
                {
                    gameObjectType = "Freshmen";
                }
                xmlWriter.WriteStartElement("type");
                xmlWriter.WriteValue(gameObjectType); //Type
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("x");
                xmlWriter.WriteValue(gameObject); //X positie
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("y"); 
                xmlWriter.WriteValue(gameObject); //Y positie
                xmlWriter.WriteEndElement();

                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("highscores");

            foreach(GameHighscores gameHighscore in gameHighscores)
            {
                xmlWriter.WriteStartElement("highscore");

                xmlWriter.WriteStartElement("name");
                xmlWriter.WriteValue(gameHighscore.name); //Name
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("datetime");
                xmlWriter.WriteValue(DateTime.Now); //DateTime
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("score");
                xmlWriter.WriteValue(gameHighscore.score); //Score
                xmlWriter.WriteEndElement();

                xmlWriter.WriteEndElement();
            }                      

            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndDocument();
            xmlWriter.Close(); //Sluiten van XMLWriter, dit is ook het moment dat het bestand wordt weggeschreven
        }
    }
}
