using HtmlAgilityPack;
using Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TMS.Libraries.OutlookMailSegmenter
{
    public class OutlookEmail : EmailReplay, IOutlookEntity
    {

        #region Init

        private MailItem _COMEmail;
        internal OutlookEmail(OutlookFolder parentFolder, MailItem COMEmail)
        {

            _COMEmail = COMEmail;
            ID = Guid.NewGuid();
            Folder = parentFolder;

            OutlookEntryID = COMEmail.EntryID;
            OutlookConversationID = COMEmail.ConversationID;
            OutlookConversationIndex = COMEmail.ConversationIndex;

            this.AttachmentsCount = COMEmail.Attachments.Count;
            //COMEmail.GetConversation().GetChildren

            if (Outlook.ProcessHeaders)
                this.Header = new EmailHeader(COMEmail);

            CreateEmail(COMEmail.HTMLBody);


        }

        #endregion

        #region Properties

        public string OutlookEntryID { get; private set; }

        public string OutlookConversationID { get; private set; }

        public string OutlookConversationIndex { get; private set; }

        public int AttachmentsCount { get; private set; }

        public OutlookFolder Folder { get; private set; }

        // Hide this property
        private new OutlookEmail ParentEmail { get; set; }

        public List<EmailReplay> Repleys { get; private set; }

        List<OutlookEmail> _Conversations;

        /// <summary>
        /// Returns a list of emails that part of the current conversation as per Outlook.
        /// <para><strong>Note:</strong></para>
        /// <para>The returned emails can be located across different stores and folders.</para>
        /// </summary>
        public List<OutlookEmail> Conversations
        {
            get
            {
                if (_Conversations == null)
                {

                    _Conversations = new List<OutlookEmail>();

                    foreach (object comItem in _COMEmail.GetConversation()?.GetRootItems())
                    {
                        if (comItem is MailItem)
                        {
                            var comMail = comItem as MailItem;

                            var mail = Outlook.GetEmailByOutlookEntryID(comMail.EntryID);

                            if (mail != null)
                                _Conversations.Add(mail);

                        }
                    }
                }

                return _Conversations;
            }
        }

        #endregion

        #region Help Methods

        private void CreateEmail(string html)
        {

            // --- find replays
            var replaysHTML = SplitToReplays(html);

            // --- set the main email

            var doc = new HtmlDocument();
            doc.LoadHtml(replaysHTML[0]);

            // first we find and strip out the signature
            if (Outlook.ProcessSignatures)
                CreateSignature(doc);

            // get main body
            this.Body = new EmailChunk(doc.DocumentNode.OuterHtml, this);

            if (Outlook.ProcessAllReplays)
            {
                // --- processes the rest of replays
                for (int i = 1; i < replaysHTML.Count; i++)
                {
                    if (Repleys is null)
                        Repleys = new List<EmailReplay>();

                    Repleys.Add(new EmailReplay(replaysHTML[i], this));
                }
            }

        }


        private List<string> SplitToReplays(string html)
        {
            List<string> conversations = new List<string>();
            List<HtmlNode> res = null;

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            // we do not need whole html file, scope to body
            HtmlNode bodyND = FindBody(doc);

            // sometimes, outlook dose not return full html document! then whole msg is a body
            string bodyHTML = (bodyND == null) ? html : bodyND.InnerHtml;

            // re-create doc from body
            doc.LoadHtml(bodyHTML);

            // find separators
            res = FindReplaySeparators(doc);

            // if no replays are detected, the whole message is main message
            if (res == null || res.Count == 0)
                conversations.Add(bodyHTML);
            else
                // add the main message to conversations list
                conversations.Add(bodyHTML.Substring(0, res[0].OuterStartIndex));

            // generate replay list
            if (Outlook.ProcessAllReplays)
            {
                for (int i = 0; i < res?.Count; i++)
                {
                    if (i == res.Count - 1)
                        conversations.Add(bodyHTML.Substring(res[i].OuterStartIndex));
                    else
                        conversations.Add(bodyHTML.Substring(res[i].OuterStartIndex, res[i + 1].OuterStartIndex - res[i].OuterStartIndex));
                }
            }

            return conversations;
        }

        /// <summary>
        /// Finds all replay dividers.
        /// <para><strong>Note:</strong></para>
        /// <para>It will search for all separators, even if we set ProcessAllReplaes=False, because we do not know which criteria corresponds to first separator.</para>
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        private List<HtmlNode> FindReplaySeparators(HtmlDocument doc)
        {

            var separators = new List<HtmlNode>();

            var res = doc.DocumentNode.SelectNodes("//div/div[contains(@style,'border-top:solid')]/p/b/span[text()='From:']")?.ToList();

            // we go up in DOM tree to get top most separator tag
            if (res != null)
                separators.AddRange(res?.Select(n => n.ParentNode?.ParentNode?.ParentNode?.ParentNode).ToList());

            // another possibility
            res = doc.DocumentNode.SelectNodes("//div/div[contains(@style,'border-top:solid')]/p/b[text()='From:']")?.ToList();

            if (res != null)
                separators.AddRange(res?.Select(n => n.ParentNode?.ParentNode?.ParentNode).ToList());

            // another possibility
            res = doc.DocumentNode.SelectNodes("//div/span/hr")?.Where(n => n.ParentNode?.ParentNode?.NextSibling?.SelectSingleNode("//div/p/strong/span[text()='From:']") != null)?.ToList();

            if (res != null)
                separators.AddRange(res?.Select(n => n.ParentNode?.ParentNode).ToList());

            // another possibility, commented replay
            res = doc.DocumentNode.SelectNodes("//comment()[contains(.,'originalMessage')]")?.ToList();
            if (res != null)
                separators.AddRange(res?.Select(n => n.ParentNode?.ParentNode).ToList());

            // since we splinting the replays sequentially, we need to make sure that our separators comes in order
            separators = separators.OrderBy(s => s.OuterStartIndex).ToList();

            return separators;
        }


        private HtmlNode FindBody(HtmlDocument doc)
        {
            return doc.DocumentNode.SelectSingleNode("/html/body");
        }

        #endregion

    }
}
