using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace TMS.Libraries.OutlookMailSegmenter
{
    public class EmailChunk
    {
        #region Init

        /// <summary>
        /// A help initializer, used for virtual chunks, i.e not real parts of emails, needed just to exploit this class' methods like cleaning and striping.
        /// </summary>
        /// <param name="chunk">The text or HTML code.</param>
        /// <param name="isHTML">Determines if the passed string is text or HTML, so it will be treated differently.</param>
        internal EmailChunk(string chunk, bool isHTML = false)
        {
            HTML = (isHTML) ? CleanHTML(chunk) : chunk;
            Text = (isHTML) ? StripHTML(HTML) : FixBadCharacters(HTML);
        }

        // the collection that will hold unique base chunks, that will be referenced by any repeated email part, like repeated signatures
        internal static List<EmailChunk> BaseMailChunks = new List<EmailChunk>();

        // for locking previous static collection
        private static Mutex mutex = new Mutex();
        internal EmailChunk(string html, object parent)
        {
            Parent = parent;
            ID = Guid.NewGuid();

            // clean html and text
            var cleandHTMl = CleanHTML(html);
            var text = StripHTML(cleandHTMl);

            // In what below, we may modify same collection in parallel, so we need to mutex-lock it to prevent errors
            if (Outlook.CheckForIdenticalChunks & Outlook.ProcessInParallel)
                mutex.WaitOne();

            if (Outlook.CheckForIdenticalChunks)
            {
                // calc hash
                SHA256 shaHash = SHA256.Create();
                var hash = GetSha256Hash(shaHash, text);

                // look if this chunk obtained before
                this.BaseChunk = BaseMailChunks.SingleOrDefault(c => c.Hash == hash);

                // if this is a new unique chunk, save hash
                if (this.BaseChunk == null)
                    Hash = hash;
            }


            // if this is a unique chunk, or we are not checking for identical chunks
            if (this.BaseChunk == null)
            {
                HTML = cleandHTMl;
                Text = text;
                BaseMailChunks.Add(this);
            }

            // unlock
            if (Outlook.CheckForIdenticalChunks & Outlook.ProcessInParallel)
                mutex.ReleaseMutex();

        }

        #endregion

        #region Properties

        public Object Parent { get; private set; }

        internal string Hash { get; private set; }

        public EmailChunk BaseChunk { get; private set; }

        public Guid ID { get; private set; }

        public string HTML { get; internal set; }

        public string Text { get; private set; }


        List<string> _EmailAdresses;
        public List<string> EmailAddresses
        {
            get
            {
                if (_EmailAdresses == null)
                    _EmailAdresses = ParseEmailAddresses(this.HTML);

                return _EmailAdresses;
            }
        }

        #endregion

        #region Help Methods


        const string emailAddressPattern = @"([a-z0-9]+(?:[._-][a-z0-9]+)*)@([a-z0-9]+(?:[.-][a-z0-9]+)*\.[a-z]{2,})";

        private static Regex emailAddressRegex = new Regex(emailAddressPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

        private List<string> ParseEmailAddresses(string text)
        {
            List<string> res = null;

            foreach (Match m in emailAddressRegex.Matches(text.ToLower().Trim()))
            {
                if (res is null)
                    res = new List<string>(0);

                string email = m.Value.ToLower().Trim();

                if (!res.Contains(email))
                    res.Add(email);

            }

            return res;
        }

        private string GetSha256Hash(SHA256 shaHash, string input)
        {
            // Convert the input string to a byte array and compute the hash.
            byte[] data = shaHash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }


        // shared regexes for html cleaning
        private static Regex closedTagsRegx = new Regex(@"<(\w+)(?:\s+\w+=''[^'']+(?:''\$[^'']+'[^'']+)?'')*>\s*<\/\1>".Replace("'", "\""), RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static Regex nonBreakingParagraphRegex = new Regex(@"(\<p\>\&nbsp\;\<\/p\>){2,}", RegexOptions.IgnoreCase | RegexOptions.Compiled);


        private string CleanHTML(string html)
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
            res = closedTagsRegx.Replace(doc.DocumentNode.InnerHtml, string.Empty);

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
        private string RemoveUnwantedHtmlTags(string html, List<string> unwantedTags)
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

        private string StripHTML(string HTML)
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

            res = FixBadCharacters(res);

            return res.Trim();

        }



        private string FixBadCharacters(string txt)
        {
            if (string.IsNullOrWhiteSpace(txt))
                return string.Empty;

            // replace non breaking spaces
            var res = Regex.Replace(txt, @"\u00A0", " ");

            // replace bad left/right quotations marks
            res = Regex.Replace(res, @"\u2019\s*\u2018", "\"");
            res = Regex.Replace(res, @"\u2018\s*\u2019", "\"");

            res = Regex.Replace(res, @"\u2019", "'");
            res = Regex.Replace(res, @"\u2018", "'");

            // bad un-visible character
            res = Regex.Replace(res, @"\u200b", string.Empty);

            // another bad space
            res = Regex.Replace(res, @"\u00a0", " ");

            // bad dash
            res = Regex.Replace(res, @"\u2013", "-");

            return res;
        }

        #endregion

    }
}
