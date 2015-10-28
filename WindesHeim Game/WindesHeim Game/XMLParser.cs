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
    //Hoe te gebruiken (voorbeeld): 
    //XMLParser level1 = new XMLParser("../levels/level1.xml"); //Geef hier het path naar het xml bestand mee
    //Voorbeeld voor titel: 
    //Console.Write(level1.gameProperties.title);
    //Voorbeeld om highscores te doorlopen
    //level1.ReadXML();
    //foreach(GameHighscore highscore in level1.gameHighscores)
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

    public struct GameHighscore
    {
        //Hier worden de Highscores in opgeslagen en vervolgens gebruikt in een List
        public string name;
        public DateTime dateTime;
        public int score;

        public GameHighscore(string name, string dateTime, int score)
        {
            this.name = name;
            this.dateTime = Convert.ToDateTime(dateTime); //Converteerd Datetime.now (string) weer terug naar Datetime (DateTime)
            this.score = score;
        }
    }

    public class XMLParser
    {
        //Vastleggen van te gebruiken variablen.
        private string path;
        public GameProperties gameProperties;
        public List<GameObject> gameObjects;
        public List<GameHighscore> gameHighscores;

        //Lijst met ingeladen levels
        public static List<XMLParser> Levels { get; set; } = new List<XMLParser>();

        public XMLParser(String path = "")
        {
            this.path = path;
        }

        public override string ToString()
        {
            return gameProperties.title + " (" + gameProperties.difficulty +")";
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
                    MovingExplodingObstacle castedGameObject = (MovingExplodingObstacle)gameObject;

                    MovingExplodingObstacle moe = new MovingExplodingObstacle(new Point(gameObject.Location.X, gameObject.Location.Y), gameObject.Height, gameObject.Width);
                    moe.MovingSpeed = castedGameObject.MovingSpeed;
                    moe.IsSmart = ((MovingExplodingObstacle)gameObject).IsSmart;

                    returnList.Add(moe);
                }
                else if (gameObject is StaticObstacle)
                {
                    returnList.Add(new StaticObstacle(new Point(gameObject.Location.X, gameObject.Location.Y), gameObject.Height, gameObject.Width));
                }
                else if (gameObject is SlowingObstacle)
                {
                    SlowingObstacle castedGameObject = (SlowingObstacle)gameObject;

                    SlowingObstacle slowingObstacle = new SlowingObstacle(new Point(gameObject.Location.X, gameObject.Location.Y), gameObject.Height, gameObject.Width);
                    slowingObstacle.MovingSpeed = castedGameObject.MovingSpeed;
                    slowingObstacle.SlowingSpeed = castedGameObject.SlowingSpeed;
                    slowingObstacle.IsSmart = ((SlowingObstacle)gameObject).IsSmart;                  

                    returnList.Add(slowingObstacle);
                }
            }
            return returnList;
        }

        //Laad alle levels en stopt deze in de static property Levels
        //Belangerijk om deze eerst aan te roepen voordat je de static property gebruikt via XMLParser.Levels
        public static bool LoadAllLevels()
        {
            Levels.Clear();
            string dirPath = "";

            if (System.Diagnostics.Debugger.IsAttached) {
                dirPath = "../levels/";
            }
            else {
                dirPath = AppDomain.CurrentDomain.BaseDirectory + "/levels/";
            }

            string[] fileEntries = Directory.GetFiles(dirPath);
            try {
                foreach (string file in fileEntries)
                {
                    if (File.Exists(file))
                    {
                        if (isXML(file))
                        {
                            XMLParser xml = new XMLParser(file);
                            xml.ReadXML();
                            Levels.Add(xml); //Ingeladen gegevens opslaan in lokale List voor hergebruik
                        }
                    }
                }
                return true;
            }
            catch { return false; }
        }
        private static bool isXML(string file)
        {
            try { XDocument doc = XDocument.Load(file); }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool AddHighscore(GameHighscore highscore)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(this.path);

            XmlNode root = doc.DocumentElement;

            if (doc.SelectSingleNode("//highscores") == null)
            {
                XmlElement highScoresElement = doc.CreateElement("highscores");
                root.AppendChild(highScoresElement);                
            }

            XmlNode highscores = doc.SelectSingleNode("//highscores");

            XmlElement highScoreElement = doc.CreateElement("highscore");

            XmlElement nameElement = doc.CreateElement("name");
            nameElement.InnerText = highscore.name;

            XmlElement datetimeElement = doc.CreateElement("datetime");
            datetimeElement.InnerText = highscore.dateTime.ToString();

            XmlElement scoreElement = doc.CreateElement("score");
            scoreElement.InnerText = highscore.score.ToString();

            highScoreElement.AppendChild(nameElement);
            highScoreElement.AppendChild(datetimeElement);
            highScoreElement.AppendChild(scoreElement);

            //Add the node to the document.            
            highscores.AppendChild(highScoreElement);
            try { 
                doc.Save(this.path);
                return true;
            }
            catch
            { 
                return false;
            }
        }

        //Funtie om XML bestand in te laden, daarna kan je de vastgelegde variablen in deze klasse gebruiken
        public bool ReadXML()
        {
            XDocument doc;
            try { doc = XDocument.Load(this.path); }
            catch { return false; }
              

            //Initieert de variablen
            gameHighscores = new List<GameHighscore>();
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
                            Slowdown = (r.Element("slowingspeed") != null) ? Int32.Parse(r.Element("slowingspeed").Value): 0,
                            Smart = (r.Element("smart") != null) ? bool.Parse(r.Element("smart").Value) : false,
                            Image = (r.Element("image") != null) ? r.Element("image").Value : ""
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
                gameHighscores.Add(new GameHighscore(highscore.Name, highscore.DateTime, highscore.Score));
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
                            MovingExplodingObstacle moe = new MovingExplodingObstacle(new Point(gameObject.X, gameObject.Y), gameObject.Height, gameObject.Width);
                            moe.MovingSpeed = gameObject.Movingspeed;
                            moe.IsSmart = gameObject.Smart;
                            gameObjects.Add(moe);
                        break;

                        case "StaticObstacle":
                            gameObjects.Add(new StaticObstacle(new Point(gameObject.X, gameObject.Y), gameObject.Height, gameObject.Width));
                        break;

                        case "SlowingObstacle":
                            SlowingObstacle sb = new SlowingObstacle(new Point(gameObject.X, gameObject.Y), gameObject.Height, gameObject.Width);
                            sb.MovingSpeed = gameObject.Movingspeed;
                            sb.SlowingSpeed = gameObject.Slowdown;
                            sb.IsSmart = gameObject.Smart;
                            gameObjects.Add(sb);
                        break;
                }
                
            }
            return true;
        }

        //Deze functie schrijft een XML file weg
        //Geef hier de gameproperties mee in het objecdt GameProperties, vervolgens een List met GameObjects daarna eenzelfde lijst voor Highscores
        public bool WriteXML()
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

                    xmlWriter.WriteStartElement("movingspeed");
                    xmlWriter.WriteValue(((MovingExplodingObstacle)gameObject).MovingSpeed); //Movingspeed
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("smart");
                    xmlWriter.WriteValue(((MovingExplodingObstacle)gameObject).IsSmart); //Movingspeed
                    xmlWriter.WriteEndElement();
                }
                else if (gameObject is StaticObstacle)
                {
                    gameObjectType = "StaticObstacle";
                }
                else if (gameObject is SlowingObstacle)
                {
                    gameObjectType = "SlowingObstacle";

                    xmlWriter.WriteStartElement("movingspeed");
                    xmlWriter.WriteValue(((SlowingObstacle)gameObject).MovingSpeed); //Movingspeed
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("slowingspeed");
                    xmlWriter.WriteValue(((SlowingObstacle)gameObject).SlowingSpeed); //Movingspeed
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("smart");
                    xmlWriter.WriteValue(((SlowingObstacle)gameObject).IsSmart); //Movingspeed
                    xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteStartElement("type");
                xmlWriter.WriteValue(gameObjectType); //Type
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("x");
                xmlWriter.WriteValue(gameObject.Location.X); //X positie
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("y"); 
                xmlWriter.WriteValue(gameObject.Location.Y); //Y positie
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("height");
                xmlWriter.WriteValue(gameObject.Height); //Hoogte
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("width");
                xmlWriter.WriteValue(gameObject.Width); //Breedte
                xmlWriter.WriteEndElement();

                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();

            if (gameHighscores != null)
            {
                xmlWriter.WriteStartElement("highscores");

                foreach (GameHighscore gameHighscore in gameHighscores)
                {
                    xmlWriter.WriteStartElement("highscore");

                    xmlWriter.WriteStartElement("name");
                    xmlWriter.WriteValue(gameHighscore.name); //Name
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("datetime");
                    xmlWriter.WriteValue(gameHighscore.dateTime); //DateTime
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("score");
                    xmlWriter.WriteValue(gameHighscore.score); //Score
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteEndElement();
            }
            try { 
                xmlWriter.WriteEndDocument();
                xmlWriter.Close(); //Sluiten van XMLWriter, dit is ook het moment dat het bestand wordt weggeschreven
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
