using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace HoardProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            string hoardFile = "";
            while (string.IsNullOrEmpty(hoardFile)||!File.Exists(hoardFile))
            {
                Console.WriteLine("Enter source hoard file");
                hoardFile = Console.ReadLine();
            }

            XmlDocument document = new XmlDocument();
            document.Load(hoardFile);

            Console.WriteLine("Total status count: " + document.DocumentElement.ChildNodes.Count);

           #region get rid of duplicates
            Dictionary<string, XmlNode> ids = new Dictionary<string, XmlNode>();
            XmlNodeList StatusIDs = document.SelectNodes("Root/Status/ID");
            foreach (XmlNode id in StatusIDs)
            {
                if (ids.ContainsKey(id.InnerText))
                {
                    Console.WriteLine("Found duplicate id: "+id.InnerText);
                    if (id.ParentNode.SelectSingleNode("/UserScreenName") == ids[id.InnerText].SelectSingleNode("/UserScreenName"))
                    {
                        if (id.ParentNode.SelectSingleNode("/CreatedDate") == ids[id.InnerText].SelectSingleNode("/CreatedDate"))
                        {
                            Console.WriteLine("Removing....");
                            document.DocumentElement.RemoveChild(id.ParentNode);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Found id: " + id.InnerText);
                    ids.Add(id.InnerText,id.ParentNode);
                }
            }
           #endregion

            FindAllMessages(document, new Regex(".*(teamgb|team gb).*"), "Team GB");
            FindAllMessages(document, new Regex(".*(#criticalmass).*"), "#criticalmass");
            FindAllMessages(document, new Regex(".*(frodo|hobbit|lord of the rings|one ring|lotr|sauron|samwise|tolkien|gandalf|boromir|gimli|legolas|followship of the ring|two towers|return of the king| shire ).*"), "LOTR");
        }

        private static void FindAllMessages(XmlDocument document, Regex matchPattern, string SearchName)
        {
            double count = 0;
            foreach (XmlNode item in document.SelectNodes("Root/Status/Text"))
            {
                if (matchPattern.Match(item.InnerText.ToLower()).Success)
                {
                    count++;
                }
            }
            Console.WriteLine(SearchName+" count: " + count);
            Console.WriteLine("As percentage: " + (count / document.DocumentElement.ChildNodes.Count) * 100 + "%");
        }
    }
}
