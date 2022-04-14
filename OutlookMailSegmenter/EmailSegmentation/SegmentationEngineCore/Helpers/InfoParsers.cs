using HtmlAgilityPack;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TMS.Libraries.EmailSegmentation.SegmentationEngineCore.Helpers
{
    public class InfoParsers
    {

        const string emailAddressPattern = @"([a-z0-9]+(?:[._-][a-z0-9]+)*)@([a-z0-9]+(?:[.-][a-z0-9]+)*\.[a-z]{2,})";

        private static Regex emailAddressRegex = new Regex(emailAddressPattern,
                                    RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

        public static List<string> ParseEmailAddresses(string text)
        {
            List<string> res = new();

            foreach (Match m in emailAddressRegex.Matches(text.ToLower().Trim()))
            {
                string email = m.Value.ToLower().Trim();

                if (!res.Contains(email))
                    res.Add(email);

            }


            return (res.Count > 0) ? res.Distinct().ToList() : null;
        }

        const string internationalPhonesPattern = @"(\+|0)\d{1,4}(\s*\(\s*\d{1,4}\s*\)\s*)*((\s|\-){0,2}\d{1,4}){7,15}";

        private static Regex phonesRegex = new Regex(internationalPhonesPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

        public static List<string> ParseInternationalPhones(string text)
        {
            List<string> res = new();

            foreach (Match m in phonesRegex.Matches(text.Trim()))
            {
                string phone = m
                                .Value
                                .Replace(" ", string.Empty)
                                .Replace("-", string.Empty)
                                .Replace("(", string.Empty)
                                .Replace(")", string.Empty)
                                .Trim();

                if (!res.Contains(phone))
                    res.Add(phone);

            }

            return (res.Count > 0) ? res.Distinct().ToList() : null;
        }

        /// <summary>
        /// Returns a dictionary of the available language tags in the html, along with how many times each language is mentioned/repeated.
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static Dictionary<string, int> ParseLangauges(string html)
        {
            var langs = new Dictionary<string, int>();

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            foreach (HtmlNode n in doc.DocumentNode.SelectNodes("//*[@lang]"))
            {
                var ln = n.GetAttributeValue("lang", null)?.ToLower().Trim();

                if (!string.IsNullOrWhiteSpace(ln))
                {
                    if (langs.ContainsKey(ln))
                        langs[ln]++;
                    else
                        langs.Add(ln, 1);
                }
            }

            return langs;
        }

        public struct HeaderInfo
        {
            public string Subject;
            public DateTime? Date;

            public List<string> To;
            public List<string> CC;

            public string From;

        }

        // header shared regex object
        private static Regex headerRegex;


        public static HeaderInfo ParseReplayHeader(string headerText)
        {
            // among many other possible ways to implement this, the below approach showed to be the most relabel

            if (headerRegex == null)
            {
                string patt = @"\b" +
                              string.Format("({0}|{1}|{2}|{3}|{4}|{5})",
                              KeyWords.fromKW,
                              KeyWords.toKW,
                              KeyWords.ccKW,
                              KeyWords.subjectKW,
                              KeyWords.sentKW,
                              KeyWords.importantKW)
                              + @"\b\s*\:";

                headerRegex = new Regex(patt,
                    RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
            }

            var res = headerRegex.Split(headerText);

            var h = new HeaderInfo();

            for (int i = 0; i < res.Count() - 1; i++)
            {
                if (KeyWords.fromKW.Contains(res[i], StringComparison.InvariantCultureIgnoreCase))
                    h.From = InfoParsers.ParseEmailAddresses(res[i + 1])?[0];


                if (KeyWords.toKW.Contains(res[i], StringComparison.InvariantCultureIgnoreCase))
                    h.To = InfoParsers.ParseEmailAddresses(res[i + 1]);


                if (KeyWords.ccKW.Contains(res[i], StringComparison.InvariantCultureIgnoreCase))
                    h.CC = InfoParsers.ParseEmailAddresses(res[i + 1]);

                if (KeyWords.sentKW.Contains(res[i], StringComparison.InvariantCultureIgnoreCase))
                {
                    DateTime dt;

                    if (
                        DateTime.TryParse(
                        Cleaners.FixBadCharacters(Cleaners.StripTextFromHTML(res[i + 1]))
                        , out dt))

                        h.Date = dt;
                }



                if (KeyWords.subjectKW.Contains(res[i], StringComparison.InvariantCultureIgnoreCase))
                    h.Subject = Cleaners.FixBadCharacters(Cleaners.StripTextFromHTML(res[i + 1]));
            }

            return h;

        }


    }
}
