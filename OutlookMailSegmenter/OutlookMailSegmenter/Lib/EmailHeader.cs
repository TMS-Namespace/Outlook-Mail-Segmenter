using HtmlAgilityPack;
using Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TMS.Libraries.OutlookMailSegmenter
{
    public class EmailHeader
    {

        #region Init

        internal EmailHeader(HtmlDocument doc)
        {
            this.ID = Guid.NewGuid();
            this.CreateHeader(doc);
        }

        internal EmailHeader(MailItem COMEmail)
        {
            this.ID = Guid.NewGuid();

            Subject = new EmailChunk(COMEmail.Subject?.Trim()).Text;
            Date = COMEmail.SentOn.ToUniversalTime();

            From = COMEmail.SenderEmailAddress.Trim();

            // outlook may return contact name in CC & To properties, to get email address we use the following:
            // from https://stackoverflow.com/questions/25758491/getting-an-email-address-instead-of-a-name

            foreach (Recipient recip in COMEmail.Recipients)
            {
                switch (recip.Type)
                {
                    case (int)OlMailRecipientType.olTo:
                        {
                            if (To == null)
                                To = new List<string>();
                            To.Add(recip.Address);
                            break;
                        }
                    case (int)OlMailRecipientType.olCC:
                        {
                            if (CC == null)
                                CC = new List<string>();
                            CC.Add(recip.Address);
                            break;
                        }
                };
            }

        }

        #endregion

        #region Properties

        public Guid ID { get; private set; }

        /// <summary>
        /// The Body object of the Header.
        /// <para><strong>Note:</strong></para>
        /// <para>In some cases the body is null, for example when it represents the header of main part of email.</para>
        /// </summary>
        public EmailChunk Body { get; private set; }

        public string Subject { get; internal set; }

        public string From { get; internal set; }

        /// <summary>
        /// Email addresses of "To" receivers.
        /// <para><strong>Note:</strong></para>
        /// <para>Emails are not always explicitly mentioned in the replay header, they can be replaced by receiver's contact name, so some receivers could be missed.</para>
        /// </summary>
        public List<string> To { get; internal set; }

        public List<string> CC { get; internal set; }

        public DateTime Date { get; internal set; }

        #endregion

        #region Help Methods

        private HtmlNode FindReplayHeader(HtmlDocument doc)
        {
            return doc.DocumentNode.SelectSingleNode("//div/div")?.ParentNode;
        }

        private void CreateHeader(HtmlDocument doc)
        {
            var header = FindReplayHeader(doc);

            if (header != null)
            {
                // we need cleaned text
                this.Body = new EmailChunk(header.InnerHtml, true);

                var headInfo = ParseReplayHeader(Body.Text);

                this.CC = headInfo.CC;
                this.From = headInfo.From;
                this.To = headInfo.To;
                this.Date = headInfo.Date;
                this.Subject = headInfo.Subject;

                // before removing header, we will remove everything above it (like lines)
                header.PreviousSibling?.PreviousSibling?.PreviousSibling?.Remove();
                header.PreviousSibling?.PreviousSibling?.Remove();
                header.PreviousSibling?.Remove();

                header.Remove(); // strip out the header
            }
        }

        private struct HeaderInfo
        {
            public string Subject;
            public DateTime Date;

            public List<string> To;
            public List<string> CC;

            public string From;

        }


        // header shared regex object
        private static Regex headerRegex;

        private HeaderInfo ParseReplayHeader(string headerText)
        {
            // prepare regex
            if (headerRegex == null)
            {
                // From, to etc... keywords in different languages
                List<string> froms = new List<string>() { "from", "от", "من" };
                List<string> tos = new List<string>() { "to", "Кому", "إلى" };
                List<string> ccs = new List<string>() { "cc", "Копия", "نسخة" };
                List<string> subjects = new List<string>() { "subject", "Тема", "الموضوع" };
                List<string> sents = new List<string>() { "sent", "Отправлено", "التاريخ" };
                List<string> importances = new List<string>() { "importance", "Важность", "من" };

                // the usual pattern for email's headers after they got replayed.
                // sometimes "importance" also in it, we do not need it, but it should be accounted
                // otherwise, it will become part of subject. Also, some times cc is just missed.
                string headerPatern = string.Format(@"(({0})\s*\:)(?<from>(.|\n)*?)(({1})\s*\:)(?<sent>(.|\n)*?)(({2})\s*\:)(?<to>(.|\n)*?)((({3})\s*\:)(?<cc>(.|\n)*?))*(({4})\s*\:)(?<subject>(.|\n)*)((({5})\s*\:)(?<importance>(.|\n)*))*",
                            string.Join("|", froms),
                            string.Join("|", sents),
                            string.Join("|", tos),
                            string.Join("|", ccs),
                            string.Join("|", subjects),
                            string.Join("|", importances));

                headerRegex = new Regex(headerPatern, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);
            }

            //headerText = WebUtility.HtmlDecode(headerText);

            var match = headerRegex.Match(headerText);

            var res = new HeaderInfo();

            res.Subject = match.Groups["subject"].Value?.Trim();
            DateTime.TryParse(match.Groups["sent"].Value?.Trim(), out res.Date);

            if (match.Groups["from"].Success)
                res.From = new EmailChunk(match.Groups["from"].Value).EmailAddresses?[0];

            if (match.Groups["cc"].Success)
                res.CC = new EmailChunk(match.Groups["cc"].Value).EmailAddresses;

            if (match.Groups["to"].Success)
                res.To = new EmailChunk(match.Groups["to"].Value).EmailAddresses;

            return res;

        }

        #endregion

    }
}
