using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using WindesHeim_Game.Properties;

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

        //Lijst met ingeladen levels
        public static List<XMLParser> Levels { get; set; } = new List<XMLParser>();

        public XMLParser(String path)
        {
            this.path = path;
        }

        public override string ToString()
        {
            return gameProperties.title;
        }

        public List<GameObject> getCleanGameObjects()
        {
            List<GameObject> returnList = new List<GameObject>();

            //Hardcoded start en eindpunt toevoegen aan level
            returnList.Add(new Checkpoint(new Point(750, 400), Resources.IconWIN, 80, 80, false));
            returnList.Add(new Checkpoint(new Point(5, -5), Resources.IconSP, 80, 80, true));

            foreach (GameObject gameObject in gameObjects)
            {
                if (gameObject is ExplodingObstacle)
                {
                    returnList.Add(new ExplodingObstacle(new Point(gameObject.Location.X, gameObject.Location.Y), gameObject.Height, gameObject.Width));
                }
                else if (gameObject is MovingExplodingObstacle)
                {
                    returnList.Add(new MovingExplodingObstacle(new Point(gameObject.Location.X, gameObject.Location.Y), gameObject.Height, gameObject.Width));
                }
                else if (gameObject is StaticObstacle)
                {
                    returnList.Add(new StaticObstacle(new Point(gameObject.Location.X, gameObject.Location.Y), gameObject.Height, gameObject.Width));
                }
                else if (gameObject is SlowingObstacle)
                {
                    returnList.Add(new SlowingObstacle(new Point(gameObject.Location.X, gameObject.Location.Y), gameObject.Height, gameObject.Width));
                }
            }
            return returnList;
        }

        //Laad alle levels en stopt deze in de static property Levels
        //Belangerijk om deze eerst aan te roepen voordat je de static property gebruikt via XMLParser.Levels
        public static void LoadAllLevels()
        {
            Levels.Clear();
            string[] fileEntries = Directory.GetFiles("../levels/");
            foreach (string file in fileEntries)
            {
                XMLParser xml = new XMLParser(file);
                xml.ReadXML();
                Levels.Add(xml); //Ingeladen gegevens opslaan in lokale List voor hergebruik
            }
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
                            Y = Int32.Parse(r.Element("y").Value),
                            Height = Int32.Parse(r.Element("height").Value),
                            Width = Int32.Parse(r.Element("width").Value),
                            //If statements voor dynamische gegevens in xml <object>
                            Movingspeed = (r.Element("movingspeed") != null) ? Int32.Parse(r.Element("movingspeed").Value): 0,
                            Slowdown = (r.Element("slowdown") != null) ? Int32.Parse(r.Element("slowdown").Value): 0
                       };
            //Voegt de gameproperties toe aan de variable gameProperties
            foreach (var property in lproperties)
            {
                gameProperties = new GameProperties(property.Title, property.Difficulty);
            }

            //Voegt alle highscores toe in een List
            highscores.OrderBy(o => o.Score);
            foreach (var highscore in highscores)
            {
                gameHighscores.Add(new GameHighscores(highscore.Name, highscore.DateTime, highscore.Score));
            }
            //Sorteert highscores op volgorde van behaalde score
            gameHighscores = gameHighscores.OrderBy(highscore => highscore.score).ToList();

            //Hardcoded start en eindpunt toevoegen aan level
            gameObjects.Add(new Checkpoint(new Point(750, 400), Resources.IconWIN, 80, 80, false));
            gameObjects.Add(new Checkpoint(new Point(5, -5), Resources.IconSP, 80, 80, true));

            //Voegt alle gameObjecten toe in een List
            foreach (var gameObject in items)
            {
                    switch (gameObject.Type)
                    {

                        case "ExplodingObstacle":
                            gameObjects.Add(new ExplodingObstacle(new Point(gameObject.X, gameObject.Y), gameObject.Height, gameObject.Width));
                        break;

                        case "MovingExplodingObstacle":
                            gameObjects.Add(new MovingExplodingObstacle(new Point(gameObject.X, gameObject.Y), gameObject.Height, gameObject.Width));
                        break;

                        case "StaticObstacle":
                            gameObjects.Add(new StaticObstacle(new Point(gameObject.X, gameObject.Y), gameObject.Height, gameObject.Width));
                        break;

                        case "SlowingObstacle":
                            SlowingObstacle sb = new SlowingObstacle(new Point(gameObject.X, gameObject.Y), gameObject.Height, gameObject.Width);
                            sb.MovingSpeed = 30;                                               

                            gameObjects.Add(sb);
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
                if (gameObject is ExplodingObstacle)
                {
                    gameObjectType = "ExplodingObstacle";
                }
                else if (gameObject is MovingExplodingObstacle)
                {
                    gameObjectType = "MovingExplodingObstacle";
                }
                else if (gameObject is StaticObstacle)
                {
                    gameObjectType = "StaticObstacle";
                }
                else if (gameObject is SlowingObstacle)
                {
                    gameObjectType = "SlowingObstacle";
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

                xmlWriter.WriteStartElement("height");
                xmlWriter.WriteValue(gameObject); //Hoogte
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("width");
                xmlWriter.WriteValue(gameObject); //Breedte
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
