using HtmlAgilityPack;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace TMS.Libraries.EmailSegmenter
{
    public class BodySegment : BaseSegment
    {

        #region Init

        private bool _IsHTML = true;


        /// <summary>
        /// A help initializer, used for virtual chunks, i.e not real parts of emails, needed just to exploit this class' methods like cleaning and striping.
        /// </summary>
        /// <param name="chunk">The text or HTML code.</param>
        /// <param name="isHTML">Determines if the passed string is text or HTML, so it will be treated differently.</param>
        public BodySegment(string chunk, bool isHTML = false) : base(chunk, null) { _IsHTML = isHTML; }

        internal BodySegment(HtmlDocument doc, BaseSegment parent) : base(doc, parent) { }


        internal BodySegment(string html, BaseSegment parent) : base(html, parent) { }

        #endregion

        #region Properties

        private string _HTML;
        public string HTML
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_HTML))
                    _HTML = (_IsHTML) ? CleanHTML(OriginalHTML) : OriginalHTML;

                return _HTML;
            }
        }

        private string _Text;
        public string Text
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_Text))
                    _Text = (_IsHTML) ? StripHTML(HTML) : FixBadCharacters(HTML);

                return _Text;
            }
        }

        // hide body from base
        private new BodySegment Body { get; set; }

        List<string> _EmailAddresses;
        public List<string> EmailAddresses
        {
            get
            {
                if (_EmailAddresses == null)
                    _EmailAddresses = ParseEmailAddresses(this.HTML);

                return _EmailAddresses;
            }
        }

        List<string> _Phones;
        public List<string> Phones
        {
            get
            {
                if (_Phones == null)
                    _Phones = ParsePhones(this.HTML);

                return _Phones;
            }
        }



        #endregion

        #region Help Methods

        #region Information Parsers

        const string emailAddressPattern = @"([a-z0-9]+(?:[._-][a-z0-9]+)*)@([a-z0-9]+(?:[.-][a-z0-9]+)*\.[a-z]{2,})";

        private static Regex emailAddressRegex = new Regex(emailAddressPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

        private List<string> ParseEmailAddresses(string text)
        {
            List<string> res = null;

            foreach (Match m in emailAddressRegex.Matches(text.ToLower().Trim()))
            {
                if (res is null)
                    res = new List<string>();

                string email = m.Value.ToLower().Trim();

                if (!res.Contains(email))
                    res.Add(email);

            }

            return res;
        }

        const string internationalPhonesPattern = @"(\+|0)\d{1,4}(\s*\(\s*\d{1,4}\s*\)\s*)*((\s|\-){0,2}\d{1,4}){7,15}";

        private static Regex phonesRegex = new Regex(internationalPhonesPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

        private List<string> ParsePhones(string text)
        {
            List<string> res = null;

            foreach (Match m in emailAddressRegex.Matches(text.ToLower().Trim()))
            {
                if (res is null)
                    res = new List<string>();

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

            return res;
        }

        #endregion

        #region Cleaning HTML

        // shared regexes for html cleaning
        private static Regex closedTagsRegex = new Regex(@"<(\w+)(?:\s+\w+=''[^'']+(?:''\$[^'']+'[^'']+)?'')*>\s*<\/\1>".Replace("'", "\""), RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static Regex nonBreakingParagraphRegex = new Regex(@"(\<p\>\&nbsp\;\<\/p\>){2,}", RegexOptions.IgnoreCase | RegexOptions.Compiled);



        public static string CleanHTML(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
                return string.Empty;

            // remove unneeded tags
            var notNeededTags = new List<string>() { "a", "span", "o:p", "b", "i", "u", "style" };
            var res = RemoveUnwantedHtmlTags(html, notNeededTags);

            // remove repeated un-breaking spaces
            for (int i = 0; i < 5; i++)
                res = Regex.Replace(res, @"(\&nbsp\;){2,}", @"&nbsp;");

            // remove bad characters
            res = FixBadCharacters(res);

            // remove repeated <p>&nbsp;</p>
            string nonBreakingParagraph = @"<p>&nbsp;</p>";
            for (int i = 0; i < 5; i++)
                res = nonBreakingParagraphRegex.Replace(res, nonBreakingParagraph);


            // remove usual endings
            res = Regex.Replace(res, @"(best(\s|\n)*regards)|(sincerely(\s|\n)*yours)", string.Empty, RegexOptions.IgnoreCase);


            // Remove all attributes
            // from: https://html-agility-pack.net/knowledge-base/37700985/how-to-remove-all-attributes-in-html-tags

            var doc = new HtmlDocument();
            doc.LoadHtml(res);

            foreach (var eachNode in doc.DocumentNode.Descendants().Where(x => x.NodeType == HtmlNodeType.Element))
            {
                eachNode.Attributes.RemoveAll();
            }

            // Remove any comments
            doc.DocumentNode.Descendants()
                             .OfType<HtmlCommentNode>()
                             .ToList()
                             .ForEach(n => n.Remove());


            // Remove empty tags
            // from https://stackoverflow.com/questions/26226277/remove-unused-empty-html-tags
            res = closedTagsRegex.Replace(doc.DocumentNode.InnerHtml, string.Empty);

            // remove <p>&nbsp;</p> at start and end
            if (res.StartsWith(nonBreakingParagraph))
                res = res.Substring(nonBreakingParagraph.Length);

            // <p>&nbsp;</p> may appear again at the end, multiple times
            for (int i = 0; i < 10; i++)
                if (res.EndsWith(nonBreakingParagraph))
                    res = res.Substring(0, res.Length - nonBreakingParagraph.Length);

            // remove break lines
            for (int i = 0; i < 5; i++)
                res = Regex.Replace(res, @"(\r\n)", string.Empty);

            return res.Trim();

        }


        // from: https://stackoverflow.com/questions/12787449/html-agility-pack-removing-unwanted-tags-without-removing-content/12836974#12836974
        private static string RemoveUnwantedHtmlTags(string html, List<string> unwantedTags)
        {
            if (String.IsNullOrEmpty(html))
                return html;

            var document = new HtmlDocument();
            document.LoadHtml(html);

            HtmlNodeCollection tryGetNodes = document.DocumentNode.SelectNodes("./*|./text()");

            if (tryGetNodes == null || !tryGetNodes.Any())
                return html;

            var nodes = new Queue<HtmlNode>(tryGetNodes);

            while (nodes.Count > 0)
            {
                var node = nodes.Dequeue();
                var parentNode = node.ParentNode;

                var childNodes = node.SelectNodes("./*|./text()");

                if (childNodes != null)
                    foreach (var child in childNodes)
                        nodes.Enqueue(child);


                if (unwantedTags.Any(tag => tag == node.Name))
                {
                    if (childNodes != null)
                        foreach (var child in childNodes)
                            parentNode.InsertBefore(child, node);

                    parentNode.RemoveChild(node);
                }
            }

            return document.DocumentNode.InnerHtml;
        }

        #endregion

        #region Cleaning Text

        public static string FixBadCharacters(string txt)
        {
            if (string.IsNullOrWhiteSpace(txt))
                return string.Empty;

            // replace non breaking spaces, and other spaces
            // see https://en.wikipedia.org/wiki/Non-breaking_space
            var res = Regex.Replace(txt, @"(\u00A0|\u00a0|\u2007|\u202F|\u2060)", " ");


            // replace bad left/right quotations marks
            res = Regex.Replace(res, @"((\u2019\s*\u2018)|(\u2018\s*\u2019))", "\"");

            res = Regex.Replace(res, @"(\u2019|\u2018)", "'");

            // bad un-visible character
            res = Regex.Replace(res, @"\u200b", string.Empty);


            // bad dash
            res = Regex.Replace(res, @"\u2013", "-");

            return res.Trim();
        }


        public static string StripHTML(string HTML)
        {

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(HTML);


            StringBuilder sb = new StringBuilder();
            IEnumerable<HtmlNode> nodes = doc.DocumentNode.Descendants().Where(n =>
               n.NodeType == HtmlNodeType.Text &&
               n.ParentNode.Name != "script" &&
               n.ParentNode.Name != "style");

            foreach (HtmlNode node in nodes)
            {
                string text = node.InnerText;
                if (!string.IsNullOrEmpty(text))
                    sb.AppendLine(text.Trim());
            }

            string res = sb.ToString();

            // decode special characters
            res = WebUtility.HtmlDecode(res).Trim();

            // remove double lines
            for (int i = 0; i < 5; i++)
                res = Regex.Replace(res, @"(\r\n\s*){2,}", "\r\n");


            // resolve encoded tags, that may be part of the text by now
            res = Regex.Replace(res, @"&lt;", "<");
            res = Regex.Replace(res, @"&gt;", ">");
            res = Regex.Replace(res, @"&amp;", "&");
            res = Regex.Replace(res, @"&quot;", "\"");
            res = Regex.Replace(res, @"&apos;", "'");

            res = Regex.Replace(res, @"[^\r]\n", Environment.NewLine);

            res = FixBadCharacters(res);

            return res.Trim();

        }

        #endregion

        #endregion

    }
}
