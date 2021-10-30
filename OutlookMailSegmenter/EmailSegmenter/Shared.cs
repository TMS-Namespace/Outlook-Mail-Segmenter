using HtmlAgilityPack;
using Wmhelp.XPath2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.Libraries.EmailSegmenter
{
    internal static class Shared
    {

        // From, to etc... keywords in different languages
        public static string froms = string.Join("|", new List<string>() { "from", "от", "من" });
        public static string tos = string.Join("|", new List<string>() { "to", "Кому", "إلى" });
        public static string ccs = string.Join("|", new List<string>() { "cc", "Копия", "نسخة" });
        public static string subjects = string.Join("|", new List<string>() { "subject", "Тема", "الموضوع" });
        public static string sents = string.Join("|", new List<string>() { "sent", "date", "Отправлено", "التاريخ" });
        public static string importances = string.Join("|", new List<string>() { "importance", "Важность", "من" });



        public static List<HtmlNode> GetXPath2Nodes(HtmlDocument doc, string query)
        {
            return doc
                        .CreateNavigator()
                        .XPath2SelectNodes(query)
                        .Cast<HtmlNodeNavigator>()
                        .ToList()
                        .Select(n => n.CurrentNode).ToList();
        }

    }
}
