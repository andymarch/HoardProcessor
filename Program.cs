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
                    if (id.ParentNode.SelectSingleNode("/UserScreenName") == ids[id.InnerText].SelectSingleNode("/UserScreenName"))
                    {
                        if (id.ParentNode.SelectSingleNode("/CreatedDate") == ids[id.InnerText].SelectSingleNode("/CreatedDate"))
                        {
                            document.DocumentElement.RemoveChild(id.ParentNode);
                        }
                    }
                }
                else
                {
                    ids.Add(id.InnerText,id.ParentNode);
                }
            }
            double count = ids.Count;
           #endregion

            List<string> gb = FindAllMessages(document, new Regex(".*(teamgb|team gb).*"), "Team GB");
            List<string> torch = FindAllMessages(document, new Regex(".*(torch|cauldron).*"), "The torch & cauldron");
            List<string> redgrave = FindAllMessages(document, new Regex(".*(steve|redgrave).*"), "Steve Redgrave");
            List<string> macca = FindAllMessages(document, new Regex(".*(paul|mccartney|beatles|hey jude|macca).*"), "Paul McCartney");
            List<string> wiggins = FindAllMessages(document, new Regex(".*(bradley|wiggins|wiggo).*"), "Wiggins");
            List<string> queen = FindAllMessages(document, new Regex(".*(queen|her majesty).*"), "The Queen");
            List<string> bond = FindAllMessages(document, new Regex(".*(007|bond|james bond).*"), "Bond");
            List<string> cm = FindAllMessages(document, new Regex(".*(#criticalmass).*"), "#criticalmass");
            List<string> bean = FindAllMessages(document, new Regex(".*(bean|rowan|atkinson|chariots of fire ).*"), "Mr Bean");
            List<string> lotr = FindAllMessages(document, new Regex(".*(frodo|hobbit|lord of the rings|one ring|lotr|sauron|samwise|tolkien|gandalf|boromir|gimli|legolas|followship of the ring|two towers|return of the king| shire ).*"), "LOTR");

            Dictionary<string, XmlNode> RemainingIds = ids;
            RemainingIds = RemoveMatches(RemainingIds,gb);
            RemainingIds = RemoveMatches(RemainingIds,torch);
            RemainingIds = RemoveMatches(RemainingIds,redgrave);
            RemainingIds = RemoveMatches(RemainingIds,macca);
            RemainingIds = RemoveMatches(RemainingIds,wiggins);
            RemainingIds = RemoveMatches(RemainingIds,queen);
            RemainingIds = RemoveMatches(RemainingIds,bond);
            RemainingIds = RemoveMatches(RemainingIds,cm);
            RemainingIds = RemoveMatches(RemainingIds,bean);
            RemainingIds = RemoveMatches(RemainingIds,lotr);

            Console.WriteLine("Unmatched count: " + RemainingIds.Count);
            Console.WriteLine("As percentage: " + (RemainingIds.Count / count) * 100 + "%");
        }

        private static List<string> FindAllMessages(XmlDocument document, Regex matchPattern, string SearchName)
        {
            List<string> returnObj = new List<string>();
            double count = 0;
            foreach (XmlNode item in document.SelectNodes("Root/Status/Text"))
            {
                if (matchPattern.Match(item.InnerText.ToLower()).Success)
                {
                    returnObj.Add(item.ParentNode.SelectSingleNode("ID").InnerText);
                    count++;
                }
            }
            Console.WriteLine(SearchName+" count: " + count);
            Console.WriteLine("As percentage: " + (count / document.DocumentElement.ChildNodes.Count) * 100 + "%");
            return returnObj;
        }

        private static Dictionary<string, XmlNode> RemoveMatches(Dictionary<string, XmlNode> idList, List<string> matches)
        {
            foreach (string item in matches)
            {
                if(idList.ContainsKey(item))
                {
                    idList.Remove(item);
                }
            }
            return idList;
        }
    }
}
