using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace WindesHeim_Game
{
    public class XML
    {
        private String path;

        public XML(String path)
        {
            this.path = path;
        }

        public void Read()
        {
            XDocument doc = XDocument.Load("../levels/0.xml");

            var properties = from r in doc.Descendants("properties")
                             select new
                             {
                                 Title = r.Element("title").Value,
                                 Difficulty = r.Element("difficulty").Value
                             };

            foreach (var r in properties)
            {
                Console.WriteLine("title: " + r.Title + " difficulty: " + r.Difficulty);
            }

        }

        public void Write()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;
            settings.NewLineOnAttributes = false;
            settings.IndentChars = "     ";

            XmlWriter xmlWriter = XmlWriter.Create("level1.xml", settings);

            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("level");

            xmlWriter.WriteStartElement("properties");

            xmlWriter.WriteStartElement("title");
            xmlWriter.WriteValue("Level 1");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("difficulty");
            xmlWriter.WriteValue("hard");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("items");

            for (int i = 0; i < 10; i++)
            {
                xmlWriter.WriteStartElement("object");

                xmlWriter.WriteStartElement("type");
                xmlWriter.WriteValue("Player");
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("x");
                xmlWriter.WriteValue("10");
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("y");
                xmlWriter.WriteValue("10");
                xmlWriter.WriteEndElement();

                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("highscores");

            xmlWriter.WriteStartElement("highscore");

            xmlWriter.WriteStartElement("name");
            xmlWriter.WriteValue("Jonathan");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("datetime");
            xmlWriter.WriteValue(DateTime.Now.ToString("yyyyMMddHHmmss"));
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("score");
            xmlWriter.WriteValue("120");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
            Console.WriteLine("test");
        }
    }
}
